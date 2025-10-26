# Was ist neu in Generative KI für Anfänger .NET

Diese Seite verfolgt die Geschichte neuer Funktionen, Tools und Modelle, die dem Kurs hinzugefügt wurden. Schauen Sie regelmäßig nach Updates!

## Juni 2025

### Neu: Azure OpenAI Sora Video-Generierungs-Demo

- **Neues Lektion 3 Beispiel: Azure Sora Video-Generierung**
- Lektion 3 bietet jetzt eine praktische Demonstration, die zeigt, wie man Videos aus Text-Prompts mit dem neuen [Sora Video-Generierungsmodell](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) in Azure OpenAI generiert.
- Das Beispiel demonstriert, wie man:
  - Einen Video-Generierungsauftrag mit einem kreativen Prompt einreicht.
  - Den Auftragsstatus abfragt und die resultierende Videodatei automatisch herunterlädt.
  - Das generierte Video auf dem Desktop zur einfachen Anzeige speichert.
- Siehe die offizielle Dokumentation: [Azure OpenAI Sora Video-Generierung](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- Finden Sie das Beispiel unter [Lektion 3: Grundlegende Generative KI-Techniken /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

### Neues eShopLite Szenario: Parallele Agent-Orchestrierung (Juni 2025)

- **Neues Szenario: Parallele Agent-Orchestrierung**
- Das [eShopLite Repository](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) bietet jetzt ein Szenario, das parallele Agent-Orchestrierung mit Semantic Kernel demonstriert.
- Dieses Szenario zeigt, wie mehrere Agenten parallel arbeiten können, um Benutzeranfragen zu analysieren und wertvolle Erkenntnisse für zukünftige Analysen zu liefern.

## Mai 2025

### Azure OpenAI Bild-Generierungsmodell: gpt-image-1

- **Neues Lektion 3 Beispiel: Azure OpenAI Bild-Generierung**
  - Lektion 3 umfasst jetzt Codebeispiele und Erklärungen für die Verwendung des neuen Azure OpenAI Bild-Generierungsmodells: `gpt-image-1`.
  - Lernen Sie, wie Sie Bilder mit den neuesten Azure OpenAI-Funktionen direkt aus .NET generieren.
  - Siehe die [offizielle Azure OpenAI DALL·E Dokumentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) und den [openai-dotnet Bild-Generierungsleitfaden](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) für weitere Details.
  - Finden Sie das Beispiel unter [Lektion 3: Grundlegende Generative KI-Techniken](../../03-CoreGenerativeAITechniques/).

### Lokale Modelle mit AI Toolkit und Docker ausführen

- **Neu: Lokale Modelle mit AI Toolkit und Docker ausführen**: Entdecken Sie neue Beispiele für das lokale Ausführen von Modellen mit [AI Toolkit für Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview) und [Docker Model Runner](https://docs.docker.com/model-runner/). Der Quellcode befindet sich in [../../../samples/CoreSamples/](../../../samples/CoreSamples/) und zeigt, wie man Semantic Kernel und Microsoft Extensions für AI verwendet, um mit diesen Modellen zu interagieren.

## März 2025

### MCP-Bibliothek Integration

- **Model Context Protocol für .NET**: Wir haben Unterstützung für das [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) hinzugefügt, das eine standardisierte Möglichkeit bietet, mit KI-Modellen verschiedener Anbieter zu kommunizieren.
- Diese Integration ermöglicht konsistentere Interaktionen mit Modellen und reduziert gleichzeitig die Anbieterabhängigkeit.
- Schauen Sie sich unsere neuen Beispiele an, die MCP-Integration im Bereich [Grundlegende Generative KI-Techniken](../../03-CoreGenerativeAITechniques/) demonstrieren.

### eShopLite Szenario-Repository

- **Neues eShopLite Repository**: Alle eShopLite-Szenarien sind jetzt in einem einzigen Repository verfügbar: [https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- Das neue Repository umfasst:
  - Produktkatalog-Navigation
  - Warenkorb-Verwaltung
  - Bestellabwicklung und -verfolgung
  - Benutzerauthentifizierung und Profile
  - Integration mit Generativer KI für Empfehlungen und Chat
  - Administrator-Dashboard für Produkt- und Bestellverwaltung