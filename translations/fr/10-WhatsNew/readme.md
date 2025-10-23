# Quoi de neuf dans Générative AI pour Débutants .NET

Cette page suit l'historique des nouvelles fonctionnalités, outils et modèles ajoutés au cours. Revenez régulièrement pour les mises à jour !

## Juin 2025

### Nouveau : Démo de génération vidéo Azure OpenAI Sora

- **Nouvel exemple Leçon 3 : Génération vidéo Azure Sora**
- La leçon 3 propose désormais une démo pratique montrant comment générer des vidéos à partir de prompts textuels en utilisant le nouveau [modèle de génération vidéo Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) dans Azure OpenAI.
- L'exemple démontre comment :
  - Soumettre un travail de génération vidéo avec un prompt créatif.
  - Interroger le statut du travail et télécharger automatiquement le fichier vidéo résultant.
  - Sauvegarder la vidéo générée sur votre bureau pour un visionnage facile.
- Voir la documentation officielle : [Génération vidéo Azure OpenAI Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- Trouvez l'exemple dans [Leçon 3 : Techniques IA génératives fondamentales /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

### Nouveau scénario eShopLite : Orchestration d'agents concurrents (Juin 2025)

- **Nouveau scénario : Orchestration d'agents concurrents**
- Le [dépôt eShopLite](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) propose maintenant un scénario démontrant l'orchestration d'agents concurrents en utilisant Semantic Kernel.
- Ce scénario montre comment plusieurs agents peuvent travailler en parallèle pour analyser les requêtes utilisateur et fournir des insights précieux pour l'analyse future.

## Mai 2025

### Modèle de génération d'images Azure OpenAI : gpt-image-1

- **Nouvel exemple Leçon 3 : Génération d'images Azure OpenAI**
  - La leçon 3 inclut maintenant des exemples de code et des explications pour utiliser le nouveau modèle de génération d'images Azure OpenAI : `gpt-image-1`.
  - Apprenez comment générer des images en utilisant les dernières capacités d'Azure OpenAI directement depuis .NET.
  - Voir la [documentation officielle Azure OpenAI DALL·E](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) et le [guide de génération d'images openai-dotnet](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) pour plus de détails.
  - Trouvez l'exemple dans [Leçon 3 : Techniques IA génératives fondamentales](../../../03-CoreGenerativeAITechniques/).

### Exécuter des modèles locaux avec AI Toolkit et Docker

- **Nouveau : Exécuter des modèles locaux avec AI Toolkit et Docker** : Explorez de nouveaux exemples pour exécuter des modèles localement en utilisant [AI Toolkit pour Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview) et [Docker Model Runner](https://docs.docker.com/model-runner/). Le code source se trouve dans [../../../samples/CoreSamples/](../../../samples/CoreSamples/) et démontre comment utiliser Semantic Kernel et Microsoft Extensions pour AI pour interagir avec ces modèles.

## Mars 2025

### Intégration de la bibliothèque MCP

- **Protocole de contexte de modèle pour .NET** : Nous avons ajouté le support pour le [SDK MCP C#](https://github.com/modelcontextprotocol/csharp-sdk), qui fournit un moyen standardisé de communiquer avec les modèles IA à travers différents fournisseurs.
- Cette intégration permet des interactions plus cohérentes avec les modèles tout en réduisant la dépendance aux fournisseurs.
- Découvrez nos nouveaux exemples démontrant l'intégration MCP dans la section [Techniques IA génératives fondamentales](../../../03-CoreGenerativeAITechniques/).

### Dépôt de scénarios eShopLite

- **Nouveau dépôt eShopLite** : Tous les scénarios eShopLite sont maintenant disponibles dans un seul dépôt : [https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- Le nouveau dépôt inclut :
  - Navigation du catalogue de produits
  - Gestion du panier d'achat
  - Placement et suivi des commandes
  - Authentification et profils utilisateur
  - Intégration avec l'IA générative pour les recommandations et le chat
  - Tableau de bord administrateur pour la gestion des produits et commandes