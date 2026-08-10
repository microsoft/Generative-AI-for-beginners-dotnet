using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Extensions.Configuration;


// get key and region
string? speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
string? speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");
if (string.IsNullOrWhiteSpace(speechKey) || string.IsNullOrWhiteSpace(speechRegion))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    if (string.IsNullOrWhiteSpace(speechKey)) speechKey = config["SPEECH_KEY"];
    if (string.IsNullOrWhiteSpace(speechRegion)) speechRegion = config["SPEECH_REGION"];
}

// preflight: the Speech SDK fails with an opaque native error (0x5) when these are missing
if (string.IsNullOrWhiteSpace(speechKey) || string.IsNullOrWhiteSpace(speechRegion))
{
    Console.WriteLine("""
        Missing configuration: SPEECH_KEY and/or SPEECH_REGION

        This sample uses Azure AI Speech, which is a SEPARATE Azure resource from the
        Azure OpenAI / AI Foundry endpoint the other samples use. The values above do
        not come from AzureOpenAI:Endpoint.

        1. In the Azure portal (https://portal.azure.com), create (or open) a
           'Speech service' resource - or a multi-service 'Azure AI services' resource.
        2. Open 'Keys and Endpoint'. Copy KEY 1 and the Location/Region (for example: eastus2).
        3. Set them as environment variables, or in User Secrets:

           dotnet user-secrets set --id genai-beginners-dotnet "SPEECH_KEY" "<key>"
           dotnet user-secrets set --id genai-beginners-dotnet "SPEECH_REGION" "<region, e.g. eastus2>"

        Note: this sample also requires a working microphone. It listens on the
        default system input device and translates what you say from en-US to es-ES.
        """);
    return 1;
}

Console.WriteLine("This sample listens on your default microphone. Make sure one is connected and not muted.");

SpeechTranslationConfig speechTranslationConfig;
AudioConfig audioConfig;
TranslationRecognizer translationRecognizer;
try
{
    speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("es-ES");

    audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
}
catch (Exception ex)
{
    Console.WriteLine($"""
        Could not initialize the Azure AI Speech SDK.

        Common causes:
          - SPEECH_KEY is not a valid Azure AI Speech key (a key from Azure OpenAI will not work).
          - SPEECH_REGION does not match the region of the Speech resource (for example: eastus2).
          - No microphone is available, or the app has no permission to use it.

        Region currently in use: {speechRegion}
        Underlying error: {ex.Message}
        """);
    return 1;
}

using (audioConfig)
using (translationRecognizer)
{
    Console.WriteLine("Speak into your microphone.");
    var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
    OutputSpeechRecognitionResult(translationRecognitionResult);
}

return 0;

void OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
{
    switch (translationRecognitionResult.Reason)
    {
        case ResultReason.TranslatedSpeech:
            Console.WriteLine($"RECOGNIZED: Text={translationRecognitionResult.Text}");
            foreach (var element in translationRecognitionResult.Translations)
            {
                Console.WriteLine($"TRANSLATED into '{element.Key}': {element.Value}");
            }
            break;
        case ResultReason.NoMatch:
            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            break;
        case ResultReason.Canceled:
            var cancellation = CancellationDetails.FromResult(translationRecognitionResult);
            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

            if (cancellation.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
            }
            break;
    }
}