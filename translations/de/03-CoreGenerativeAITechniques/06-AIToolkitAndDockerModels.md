# Ausführen von Modellen lokal mit AI Toolkit und Docker

In dieser Lektion lernen Sie, wie Sie generative KI-Modelle lokal auf Ihrem Gerät oder in Ihrer Cloud-Umgebung mit AI Toolkit und Docker ausführen können.

---

## Einführung

Die Möglichkeit, KI-Modelle lokal auszuführen, bietet mehrere Vorteile wie Privatsphäre, reduzierte Kosten und volle Kontrolle über die Modellausführung. In dieser Lektion lernen Sie, wie Sie verschiedene Modelle mit dem Microsoft AI Toolkit und Docker ausführen können.

## Microsoft AI Toolkit

Das Microsoft AI Toolkit ist eine Sammlung von Werkzeugen und Bibliotheken, mit denen Sie lokale KI-Modelle in .NET-Anwendungen integrieren können. Es unterstützt eine Vielzahl von Modelltypen und Aufgaben.

### Verwendung von AI Toolkit mit .NET

Hier ist ein Beispiel dafür, wie Sie AI Toolkit in einer .NET-Anwendung verwenden können:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAIToolkitChatCompletion(
    modelId: "models/phi3:latest", 
    endpoint: "http://localhost:8080/v1");
var kernel = kernelBuilder.Build();

var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
var chat = new ChatHistory();
chat.AddUserMessage("Erkläre mir Quantencomputer in einfachen Worten");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## Docker für KI-Modelle

Docker ist eine Plattform, mit der Sie Anwendungen in isolierten Containern ausführen können. Es ist ein leistungsstarkes Werkzeug zum Ausführen von KI-Modellen in einer konsistenten Umgebung.

### Ausführen von Modellen mit Docker

Sie können verschiedene KI-Modelle über Docker ausführen:

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### Beispielanwendungen

In den [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat) und [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat) Beispielen haben wir Anwendungen implementiert, die lokale Modelle sowohl mit Semantic Kernel als auch mit Microsoft.Extensions.AI verwenden.

## Zusammenfassung

Das Ausführen von Modellen lokal gibt Ihnen mehr Kontrolle über Ihre KI-Anwendungen und kann die Kosten reduzieren. Mit Microsoft AI Toolkit und Docker können Sie eine breite Palette von Modellen in Ihren .NET-Anwendungen einsetzen.

**Haftungsausschluss**:  
Dieses Dokument wurde mit KI-gestützten maschinellen Übersetzungsdiensten übersetzt. Obwohl wir uns um Genauigkeit bemühen, weisen wir darauf hin, dass automatisierte Übersetzungen Fehler oder Ungenauigkeiten enthalten können. Das Originaldokument in seiner ursprünglichen Sprache sollte als maßgebliche Quelle betrachtet werden. Für kritische Informationen wird eine professionelle menschliche Übersetzung empfohlen. Wir übernehmen keine Haftung für Missverständnisse oder Fehlinterpretationen, die sich aus der Nutzung dieser Übersetzung ergeben.
