# Exécution de modèles d'IA localement avec AI Toolkit et Docker

Dans cette leçon, vous apprendrez comment exécuter des modèles d'IA générative localement sur votre appareil ou dans votre environnement cloud en utilisant AI Toolkit et Docker.

---

## Introduction

La capacité d'exécuter des modèles d'IA localement offre plusieurs avantages comme la confidentialité, la réduction des coûts et le contrôle total sur l'exécution du modèle. Dans cette leçon, vous apprendrez à exécuter différents modèles avec Microsoft AI Toolkit et Docker.

## Microsoft AI Toolkit

Microsoft AI Toolkit est un ensemble d'outils et de bibliothèques qui vous permettent d'intégrer des modèles d'IA locaux dans les applications .NET. Il prend en charge une variété de types de modèles et de tâches.

### Utilisation d'AI Toolkit avec .NET

Voici un exemple d'utilisation d'AI Toolkit dans une application .NET :

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
chat.AddUserMessage("Expliquez-moi l'informatique quantique en termes simples");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## Docker pour les modèles d'IA

Docker est une plateforme qui vous permet d'exécuter des applications dans des conteneurs isolés. C'est un outil puissant pour exécuter des modèles d'IA dans un environnement cohérent.

### Exécution de modèles avec Docker

Vous pouvez exécuter différents modèles d'IA via Docker :

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### Applications exemples

Dans les exemples [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat) et [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat), nous avons implémenté des applications qui utilisent des modèles locaux avec Semantic Kernel et Microsoft.Extensions.AI.

## Résumé

L'exécution locale de modèles vous donne plus de contrôle sur vos applications d'IA et peut réduire les coûts. Avec Microsoft AI Toolkit et Docker, vous pouvez déployer une large gamme de modèles dans vos applications .NET.

**Avertissement** :  
Ce document a été traduit à l'aide de services de traduction automatisée basés sur l'intelligence artificielle. Bien que nous fassions de notre mieux pour garantir l'exactitude, veuillez noter que les traductions automatiques peuvent contenir des erreurs ou des inexactitudes. Le document original dans sa langue d'origine doit être considéré comme la source faisant autorité. Pour des informations critiques, il est recommandé de recourir à une traduction humaine professionnelle. Nous déclinons toute responsabilité en cas de malentendus ou d'interprétations erronées résultant de l'utilisation de cette traduction.
