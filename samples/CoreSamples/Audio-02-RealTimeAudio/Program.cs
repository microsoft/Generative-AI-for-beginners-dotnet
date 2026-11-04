using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Realtime;
using System.ClientModel;

#pragma warning disable AOAI001, OPENAI002 

public class Program
{
    private static string finishedConversationToolName = "user_wants_to_finish_conversation";

    public static async Task Main(string[] args)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Microphone and speaker playback in this sample require Windows.");
        }
        // First, we create a client according to configured environment variables (see end of file) and then start
        // a new conversation session.
        RealtimeClient client = GetConfiguredClient();
        var realtimeModel = GetModel();
        using RealtimeSessionClient session = await client.StartConversationSessionAsync(realtimeModel);

        // Now we configure the session using the tool we created along with transcription options that enable input
        // audio transcription with whisper.
        var prompt = "you are a useful chat that helps the user.";
        RealtimeConversationSessionOptions conversationSessionOptions = CreateConversationSessionOptions(prompt);
        await session.ConfigureConversationSessionAsync(conversationSessionOptions);

        // For convenience, we'll proactively start playback to the speakers now. Nothing will play until it's enqueued.
        using SpeakerOutput speakerOutput = new();

        // With the session configured, we start processing commands received from the service.
        await foreach (RealtimeServerUpdate update in session.ReceiveUpdatesAsync())
        {
            // session.created is the very first command on a session and lets us know that connection was successful.
            if (update is RealtimeServerUpdateSessionCreated)
            {
                Console.WriteLine($" <<< Connected: session started");
                // This is a good time to start capturing microphone input and sending audio to the service. The
                // input stream will be chunked and sent asynchronously, so we don't need to await anything in the
                // processing loop.
                _ = Task.Run(async () =>
                {
                    using MicrophoneAudioStream microphoneInput = MicrophoneAudioStream.Start();
                    Console.WriteLine($" >>> Listening to microphone input");
                    Console.WriteLine($" >>> (Just tell the app you're done to finish)");
                    Console.WriteLine();
                    await session.SendInputAudioAsync(microphoneInput);
                });
            }

            // input_audio_buffer.speech_started tells us that the beginning of speech was detected in the input audio
            // we're sending from the microphone.
            if (update is RealtimeServerUpdateInputAudioBufferSpeechStarted speechStartedUpdate)
            {
                Console.WriteLine($" <<< Start of speech detected @ {speechStartedUpdate.AudioStartTime}");
                // Like any good listener, we can use the cue that the user started speaking as a hint that the app
                // should stop talking. Note that we could also track the playback position and truncate the response
                // item so that the model doesn't "remember things it didn't say" -- that's not demonstrated here.
                speakerOutput.ClearPlayback();
            }

            // input_audio_buffer.speech_stopped tells us that the end of speech was detected in the input audio sent
            // from the microphone. It'll automatically tell the model to start generating a response to reply back.
            if (update is RealtimeServerUpdateInputAudioBufferSpeechStopped speechFinishedUpdate)
            {
                Console.WriteLine($" <<< End of speech detected @ {speechFinishedUpdate.AudioEndTime}");
            }

            // conversation.item.input_audio_transcription.completed will only arrive if input transcription was
            // configured for the session. It provides a written representation of what the user said, which can
            // provide good feedback about what the model will use to respond.
            if (update is RealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted transcriptionFinishedUpdate)
            {
                Console.WriteLine($" >>> USER: {transcriptionFinishedUpdate.Transcript}");
            }

            // Item streaming delta updates provide a combined view into incremental item data including output
            // the audio response transcript, function arguments, and audio data.
            if (update is RealtimeServerUpdateResponseOutputAudioTranscriptDelta transcriptDelta)
            {
                Console.Write(transcriptDelta.Delta);
            }
            if (update is RealtimeServerUpdateResponseOutputTextDelta textDelta)
            {
                Console.Write(textDelta.Delta);
            }
            if (update is RealtimeServerUpdateResponseOutputAudioDelta audioDelta)
            {
                speakerOutput.EnqueueForPlayback(audioDelta.Delta);
            }

            // response.output_item.done tells us that a model-generated item with streaming content is completed.
            // That's a good signal to provide a visual break and perform final evaluation of tool calls.
            if (update is RealtimeServerUpdateResponseOutputItemDone itemFinishedUpdate)
            {
                Console.WriteLine();
                if (itemFinishedUpdate.Item is RealtimeFunctionCallItem { FunctionName: var name }
                    && name == finishedConversationToolName)
                {
                    Console.WriteLine($" <<< Finish tool invoked -- ending conversation!");
                    break;
                }
            }

            // error commands, as the name implies, are raised when something goes wrong.
            if (update is RealtimeServerUpdateError errorUpdate)
            {
                Console.WriteLine();
                Console.WriteLine();
                throw new InvalidOperationException($"Realtime service error: {errorUpdate.Error.Message}");
            }
        }
    }

    private static RealtimeConversationSessionOptions CreateConversationSessionOptions(string instructions)
    {
        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = instructions,
            AudioOptions = new()
            {
                InputAudioOptions = new()
                {
                    AudioFormat = new RealtimePcmAudioFormat { Rate = 24000 },
                    AudioTranscriptionOptions = new() { Model = "whisper-1" },
                    TurnDetection = new RealtimeServerVadTurnDetection()
                },
                OutputAudioOptions = new()
                {
                    AudioFormat = new RealtimePcmAudioFormat { Rate = 24000 },
                    Voice = RealtimeVoice.Alloy
                }
            }
        };

        // We'll add a simple function tool that enables the model to interpret user input to figure out when it
        // might be a good time to stop the interaction.
        RealtimeFunctionTool finishConversationTool = new(finishedConversationToolName)
        {
            FunctionDescription = "Invoked when the user says goodbye, expresses being finished, or otherwise seems to want to stop the interaction.",
            FunctionParameters = BinaryData.FromString("""{"type":"object","properties":{},"additionalProperties":false}""")
        };

        sessionOptions.Tools.Add(finishConversationTool);
        return sessionOptions;
    }

    #region Configuration

    private static string GetModel()
    {
        string? aoaiDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");


        if (string.IsNullOrEmpty(aoaiDeployment))
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            aoaiDeployment = config["AZURE_OPENAI_DEPLOYMENT"];
        }
        return string.IsNullOrEmpty(aoaiDeployment) ? "gpt-realtime" : aoaiDeployment;
    }

    private static RealtimeClient GetConfiguredClient()
    {
        string? aoaiEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        string? aoaiUseEntra = Environment.GetEnvironmentVariable("AZURE_OPENAI_USE_ENTRA");
        string? aoaiDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");
        string? aoaiApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
        string? oaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");


        if (string.IsNullOrEmpty(aoaiEndpoint))
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            aoaiEndpoint = config["AZURE_OPENAI_ENDPOINT"];
            aoaiUseEntra = config["AZURE_OPENAI_USE_ENTRA"];
            aoaiDeployment = config["AZURE_OPENAI_DEPLOYMENT"];
            aoaiApiKey = config["AZURE_OPENAI_API_KEY"];
            oaiApiKey = config["OPENAI_API_KEY"];
        }

        if (aoaiEndpoint is not null && bool.TryParse(aoaiUseEntra, out bool useEntra) && useEntra)
        {
            return GetConfiguredClientForAzureOpenAIWithEntra(aoaiEndpoint, aoaiDeployment);
        }
        else if (aoaiEndpoint is not null && aoaiApiKey is not null)
        {
            return GetConfiguredClientForAzureOpenAIWithKey(aoaiEndpoint, aoaiDeployment, aoaiApiKey);
        }
        else if (aoaiEndpoint is not null)
        {
            throw new InvalidOperationException(
                $"AZURE_OPENAI_ENDPOINT configured without AZURE_OPENAI_USE_ENTRA=true or AZURE_OPENAI_API_KEY.");
        }
        else if (oaiApiKey is not null)
        {
            return GetConfiguredClientForOpenAIWithKey(oaiApiKey);
        }
        else
        {
            throw new InvalidOperationException(
                $"No environment configuration present. Please provide one of:\n"
                    + " - AZURE_OPENAI_ENDPOINT with AZURE_OPENAI_USE_ENTRA=true or AZURE_OPENAI_API_KEY\n"
                    + " - OPENAI_API_KEY");
        }
    }

    private static RealtimeClient GetConfiguredClientForAzureOpenAIWithEntra(
        string aoaiEndpoint,
        string? aoaiDeployment)
    {
        Console.WriteLine($" * Connecting to Azure OpenAI endpoint (AZURE_OPENAI_ENDPOINT): {aoaiEndpoint}");
        Console.WriteLine($" * Using Entra token-based authentication (AZURE_OPENAI_USE_ENTRA)");
        Console.WriteLine(string.IsNullOrEmpty(aoaiDeployment)
            ? $" * Using no deployment (AZURE_OPENAI_DEPLOYMENT)"
            : $" * Using deployment (AZURE_OPENAI_DEPLOYMENT): {aoaiDeployment}");

        AzureOpenAIClient aoaiClient = new(new Uri(aoaiEndpoint), new DefaultAzureCredential());
        return aoaiClient.GetRealtimeClient();
    }

    private static RealtimeClient GetConfiguredClientForAzureOpenAIWithKey(
        string aoaiEndpoint,
        string? aoaiDeployment,
        string aoaiApiKey)
    {
        Console.WriteLine($" * Connecting to Azure OpenAI endpoint (AZURE_OPENAI_ENDPOINT): {aoaiEndpoint}");
        Console.WriteLine($" * Using API key (AZURE_OPENAI_API_KEY): {aoaiApiKey[..5]}**");
        Console.WriteLine(string.IsNullOrEmpty(aoaiDeployment)
            ? $" * Using no deployment (AZURE_OPENAI_DEPLOYMENT)"
            : $" * Using deployment (AZURE_OPENAI_DEPLOYMENT): {aoaiDeployment}");

        AzureOpenAIClient aoaiClient = new(new Uri(aoaiEndpoint), new ApiKeyCredential(aoaiApiKey));
        return aoaiClient.GetRealtimeClient();
    }

    private static RealtimeClient GetConfiguredClientForOpenAIWithKey(string oaiApiKey)
    {
        string oaiEndpoint = "https://api.openai.com/v1";
        Console.WriteLine($" * Connecting to OpenAI endpoint (OPENAI_ENDPOINT): {oaiEndpoint}");
        Console.WriteLine($" * Using API key (OPENAI_API_KEY): {oaiApiKey[..5]}**");

        OpenAIClient aoaiClient = new(new ApiKeyCredential(oaiApiKey));
        return aoaiClient.GetRealtimeClient();
    }
    #endregion
}
