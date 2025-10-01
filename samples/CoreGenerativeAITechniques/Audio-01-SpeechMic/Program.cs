using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Extensions.Configuration;


// get key and region
string? speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
string? speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");
if (string.IsNullOrEmpty(speechKey) || string.IsNullOrEmpty(speechRegion))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    speechKey = config["SPEECH_KEY"];
    speechRegion = config["SPEECH_REGION"];
}

var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
speechTranslationConfig.AddTargetLanguage("es-ES");

using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

Console.WriteLine("Speak into your microphone.");
var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
OutputSpeechRecognitionResult(translationRecognitionResult);

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