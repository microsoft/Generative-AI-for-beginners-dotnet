# Einrichten der Entwicklungsumgebung mit Ollama

Wenn du Ollama verwenden möchtest, um lokale Modelle für diesen Kurs auszuführen, folge den Schritten in dieser Anleitung.

Möchtest du Azure OpenAI nicht verwenden?

👉 [Hier ist die Anleitung zur Nutzung von Azure OpenAIn](README.md)  
👉 [Hier sind die Schritte für Ollama](getting-started-ollama.md)

## Erstellen eines GitHub Codespace

Lass uns einen GitHub Codespace erstellen, um die Entwicklung für den Rest dieses Kurses vorzubereiten.

1. Öffne die Hauptseite dieses Repositorys in einem neuen Fenster, indem du [hier mit der rechten Maustaste klickst](https://github.com/microsoft/Generative-AI-for-beginners-dotnet) und **In neuem Fenster öffnen** aus dem Kontextmenü auswählst.
1. Forke dieses Repository in deinen GitHub-Account, indem du auf die **Fork**-Schaltfläche oben rechts auf der Seite klickst.
1. Klicke auf den **Code**-Dropdown-Button und wähle dann den Reiter **Codespaces** aus.
1. Wähle die Option **...** (die drei Punkte) und klicke auf **Neu mit Optionen...**.

![Erstellen eines Codespace mit benutzerdefinierten Optionen](../../../translated_images/creating-codespace.0e7334f85cf4c8d0e080a0d5b4c76c24c5bbe6bddf48dcd1403e092ea0d9bce9.de.png)

### Auswahl deines Entwicklungskonfigurationscontainers

Wähle im Dropdown-Menü **Dev container configuration** eine der folgenden Optionen aus:

**Option 1: C# (.NET)**: Dies ist die Option, die du wählen solltest, wenn du Azure OpenAI oder Azure OpenAI verwenden möchtest. Es ist die empfohlene Methode, um diesen Kurs abzuschließen. Sie enthält alle grundlegenden .NET-Entwicklungstools, die für den Kurs benötigt werden, und hat eine schnelle Startzeit.

**Option 2: C# (.NET) - Ollama**: Diese Option ist ideal, wenn du Modelle lokal mit Ollama ausführen möchtest. Sie enthält alle grundlegenden .NET-Entwicklungstools sowie Ollama, hat jedoch eine langsamere Startzeit, im Durchschnitt etwa fünf Minuten. [Folge dieser Anleitung](getting-started-ollama.md), wenn du Ollama verwenden möchtest.

Du kannst die restlichen Einstellungen so belassen, wie sie sind. Klicke auf die Schaltfläche **Create codespace**, um den Erstellungsprozess des Codespace zu starten.

![Auswahl deiner Entwicklungscontainer-Konfiguration](../../../translated_images/select-container-codespace.9b8ca34b6ff8b4cb80973924cbc1894cf7672d233b0055b47f702db60c4c6221.de.png)

## Überprüfen, ob dein Codespace mit Ollama korrekt läuft

Sobald dein Codespace vollständig geladen und konfiguriert ist, lass uns eine Beispielanwendung ausführen, um sicherzustellen, dass alles korrekt funktioniert:

1. Öffne das Terminal. Du kannst ein Terminalfenster öffnen, indem du **Ctrl+\`** (backtick) on Windows or **Cmd+`** auf macOS eingibst.

1. Wechsel in das richtige Verzeichnis, indem du den folgenden Befehl ausführst:

    ```bash
    cd 02-SetupDevEnvironment/src/BasicChat-03Ollama/
    ```

1. Führe dann die Anwendung mit folgendem Befehl aus:

    ```bash
    dotnet run
    ```

1. Es kann ein paar Sekunden dauern, aber schließlich sollte die Anwendung eine Nachricht ähnlich der folgenden ausgeben:

    ```bash
    AI, or Artificial Intelligence, refers to the development of computer systems that can perform tasks that typically require human intelligence, such as:

    1. Learning: AI systems can learn from data and improve their performance over time.
    2. Reasoning: AI systems can draw conclusions and make decisions based on the data they have been trained on.
    
    ...
    ```

> 🙋 **Hilfe benötigt?**: Funktioniert etwas nicht? [Erstelle ein Issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue), und wir helfen dir weiter.

## Das Modell in Ollama austauschen

Eine der tollen Eigenschaften von Ollama ist, dass es einfach ist, Modelle zu wechseln. Die aktuelle App verwendet das "**llama3.2**"-Modell. Lass uns stattdessen das "**phi3.5**"-Modell ausprobieren.

1. Lade das Phi3.5-Modell herunter, indem du den folgenden Befehl im Terminal ausführst:

    ```bash
    ollama pull phi3.5
    ```

    Du kannst mehr über das [Phi3.5](https://ollama.com/library/phi3.5) und andere verfügbare Modelle in der [Ollama-Bibliothek](https://ollama.com/library/) erfahren.

1. Öffne `02-SetupDevEnvironment/src/BasicChat-03Ollama/Program.cs` in deinem Editor.

    Aktualisiere die Chat-Client-Initialisierung, um das neue Modell zu verwenden, indem du die Zeilen 6-7 änderst:

    ```csharp
    IChatClient client =
        new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3.5");
    ```

1. Führe abschließend die Anwendung mit folgendem Befehl aus:

    ```bash
    dotnet run
    ```

1. Du hast gerade zu einem neuen Modell gewechselt. Beachte, wie die Antwort länger und detaillierter ist.

    ```bash
    Artificial Intelligence (AI) refers to the simulation of human intelligence processes by machines, especially computer systems. These processes include learning (the acquisition of information and accumulation of knowledge), reasoning (using the acquired knowledge to make deductions or decisions), and self-correction. AI can manifest in various forms:

    1. **Narrow AI** – Designed for specific tasks, such as facial recognition software, voice assistants like Siri or Alexa, autonomous vehicles, etc., which operate under a limited preprogrammed set of behaviors and rules but excel within their domain when compared to humans in these specialized areas.

    2. **General AI** – Capable of understanding, learning, and applying intelligence broadly across various domains like human beings do (natural language processing, problem-solving at a high level). General AIs are still largely theoretical as we haven't yet achieved this form to the extent necessary for practical applications beyond narrow tasks.
    
    ...
    ```

> 🙋 **Hilfe benötigt?**: Funktioniert etwas nicht? [Erstelle ein Issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue), und wir helfen dir weiter.

## Zusammenfassung

In dieser Lektion hast du gelernt, wie du deine Entwicklungsumgebung für den Rest des Kurses einrichtest. Du hast einen GitHub Codespace erstellt und ihn so konfiguriert, dass er Ollama verwendet. Außerdem hast du den Beispielcode aktualisiert, um Modelle problemlos auszutauschen.

### Zusätzliche Ressourcen

- [Ollama-Modelle](https://ollama.com/search)  
- [Arbeiten mit GitHub Codespaces](https://docs.github.com/en/codespaces/getting-started)  
- [Microsoft Extensions für AI-Dokumentation](https://learn.microsoft.com/dotnet/)

## Nächste Schritte

Als Nächstes werden wir erkunden, wie du deine erste KI-Anwendung erstellst! 🚀

👉 [Kerntechniken der Generativen KI](../03-CoreGenerativeAITechniques/readme.md)

**Haftungsausschluss**:  
Dieses Dokument wurde mithilfe von KI-basierten maschinellen Übersetzungsdiensten übersetzt. Obwohl wir uns um Genauigkeit bemühen, weisen wir darauf hin, dass automatisierte Übersetzungen Fehler oder Ungenauigkeiten enthalten können. Das Originaldokument in seiner ursprünglichen Sprache sollte als maßgebliche Quelle betrachtet werden. Für kritische Informationen wird eine professionelle menschliche Übersetzung empfohlen. Wir übernehmen keine Haftung für Missverständnisse oder Fehlinterpretationen, die aus der Nutzung dieser Übersetzung entstehen.