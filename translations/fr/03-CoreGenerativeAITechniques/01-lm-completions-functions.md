# Bases d'une application de chat

Dans cette leçon, nous allons explorer les bases de la création d'applications de chat en utilisant les complétions de modèles de langage et les fonctions dans .NET. Nous découvrirons également comment utiliser Microsoft Extensions AI (MEAI) pour créer des chatbots, ainsi que comment utiliser créer des fonctions ou des fonctionnalités appelées par le chatbot en fonction des entrées de l'utilisateur.

---

## Complétions de texte et chat

[![Vidéo sur les complétions de texte et le chat](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️Cliquez sur l'image pour regarder la vidéo⬆️_

Les complétions de texte peuvent être considérées comme la forme la plus basique d'interaction avec un modèle de langage dans une application d'IA. Une complétion de texte est une réponse unique générée par le modèle en fonction de l'entrée, ou "prompt", qui lui est donnée.

Une complétion de texte en elle-même n'est pas une application de chat, c'est une interaction ponctuelle. Vous pourriez utiliser les complétions de texte pour des tâches comme le résumé de contenu ou l'analyse de sentiment.

### Complétions de texte

Voyons comment utiliser les complétions de texte avec la bibliothèque **Microsoft.Extensions.AI** dans .NET.

> 🧑‍💻**Exemple de code** : [Voici un exemple fonctionnel de cette application](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI) que vous pouvez suivre.

```csharp

// this example illustrates using a model hosted on Azure OpenAI
IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-5-mini")
    .AsIChatClient();

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the repsonse
Console.WriteLine(response.Message);

```

> 🗒️**Note :** Cet exemple montre les Azure OpenAI comme service d'hébergement. Si vous souhaitez utiliser Ollama, [consultez cet exemple](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama) (il instancie un autre `IChatClient`).
>
> Si vous voulez utiliser Microsoft Foundry, vous pouvez utiliser le même code, mais il faudra changer le point de terminaison et les informations d'identification.

> 🙋 **Besoin d'aide ?** : Si vous rencontrez des problèmes, [ouvrez une issue dans le dépôt](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

### Applications de chat

Construire une application de chat est un peu plus complexe. Il s'agit d'une conversation avec le modèle, où l'utilisateur peut envoyer des prompts et le modèle répond. Et comme dans toute conversation, vous devrez conserver le contexte, ou l'historique, de la conversation pour que tout ait du sens.

#### Différents types de rôles dans le chat

Lors d'une conversation avec le modèle, les messages envoyés peuvent avoir différents types. Voici quelques exemples :

* **Système** : Le message système guide le comportement des réponses du modèle. Il sert d'instruction initiale ou de prompt qui définit le contexte, le ton et les limites de la conversation. L'utilisateur final de ce chat ne voit généralement pas ce message, mais il est crucial pour orienter la conversation.
* **Utilisateur** : Le message utilisateur est l'entrée ou le prompt de l'utilisateur final. Cela peut être une question, une affirmation ou une commande. Le modèle utilise ce message pour générer une réponse.
* **Assistant** : Le message assistant est la réponse générée par le modèle. Ces messages sont basés sur les messages système et utilisateur et sont générés par le modèle. L'utilisateur final voit ces messages.

#### Gestion de l'historique du chat

Lors d'une conversation avec le modèle, vous devrez suivre l'historique du chat. Cela est important car le modèle génère des réponses en se basant sur le message système, ainsi que sur tous les échanges entre les messages utilisateur et assistant. Chaque message supplémentaire ajoute du contexte que le modèle utilise pour générer la réponse suivante.

Voyons comment construire une application de chat en utilisant MEAI.

```csharp

// assume IChatClient is instantiated as before

List<ChatMessage> conversation =
[
    new (ChatRole.System, "You are a product review assistant. Your job is to help people write great product reviews. Keep asking questions on the person's experience with the product until you have enough information to write a review. Then write the review for them and ask if they are happy with it.")
];

Console.Write("Start typing a review (type 'q' to quit): ");

// Loop to read messages from the console
while (true)
{    
    string message = Console.ReadLine();

    if (message.ToLower() == "q")
    {
        break;
    }

    conversation.Add(new ChatMessage(ChatRole.User, message));

    // Process the message with the chat client (example)
    var response = await client.GetResponseAsync(conversation);
    conversation.Add(response.Message);
    
    Console.WriteLine(response.Message.Text);    
}

```


> 🙋 **Besoin d'aide ?** : Si vous rencontrez des problèmes, [ouvrez une issue dans le dépôt](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Appels de fonctions

[![Vidéo explicative sur les fonctions](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️Cliquez sur l'image pour regarder la vidéo⬆️_

Lors de la création d'applications d'IA, vous n'êtes pas limité aux interactions basées sur du texte. Il est possible d'étendre les fonctionnalités du chatbot en appelant des fonctions prédéfinies dans votre code en fonction des entrées utilisateur. En d'autres termes, les appels de fonctions servent de pont entre le modèle et les systèmes externes.

> 🧑‍💻**Exemple de code** : [Voici un exemple fonctionnel de cette application](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions) que vous pouvez suivre.

### Appels de fonctions dans les applications de chat

Il y a quelques étapes de configuration nécessaires pour appeler des fonctions avec MEAI.

1. Tout d'abord, bien sûr, définissez la fonction que vous voulez que le chatbot puisse appeler. Dans cet exemple, nous allons récupérer les prévisions météo.

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

```

2. Ensuite, nous allons créer un objet `ChatOptions` qui indiquera à MEAI quelles fonctions lui sont accessibles.

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

```

3. Lorsque nous instancions l'objet `IChatClient`, nous préciserons que nous utiliserons l'invocation de fonctions.

    ```csharp
    IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-5-mini")
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

4. Enfin, lorsque nous interagissons avec le modèle, nous enverrons l'objet `ChatOptions` qui spécifie la fonction que le modèle peut appeler s'il a besoin d'obtenir les informations météo.

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **Besoin d'aide ?** : Si vous rencontrez des problèmes, [ouvrez une issue dans le dépôt](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Résumé

Dans cette leçon, nous avons appris à utiliser les complétions de texte, à démarrer et gérer une conversation de chat, et à appeler des fonctions dans des applications de chat.

Dans la prochaine leçon, vous verrez comment commencer à discuter avec des données et construire ce qu'on appelle un chatbot basé sur le modèle RAG (Retrieval Augmented Generation) - et travailler avec la vision et l'audio dans une application d'IA !

## Ressources supplémentaires

* [Créer une application de chat IA avec .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [Exécuter une fonction .NET locale](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [Discuter avec un modèle IA local](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## À venir

👉 [Construisons une application RAG !](./02-retrieval-augmented-generation.md)

**Avertissement** :  
Ce document a été traduit à l'aide de services de traduction automatique basés sur l'intelligence artificielle. Bien que nous fassions de notre mieux pour garantir l'exactitude, veuillez noter que les traductions automatiques peuvent contenir des erreurs ou des inexactitudes. Le document original dans sa langue d'origine doit être considéré comme la source faisant autorité. Pour des informations critiques, il est recommandé de faire appel à une traduction humaine professionnelle. Nous déclinons toute responsabilité en cas de malentendus ou de mauvaises interprétations résultant de l'utilisation de cette traduction.
