# Agents IA

Dans cette leçon, apprenez à créer une entité IA qui... prend des décisions et exécute des actions sans interaction humaine continue ? C'est exact, les agents IA peuvent accomplir des tâches spécifiques de manière autonome.

---

[![Vidéo explicative sur les agents](https://img.youtube.com/vi/Btkmw1Bosh0/0.jpg)](https://youtu.be/Btkmw1Bosh0?feature=shared)

_⬆️Cliquez sur l'image pour regarder la vidéo⬆️_

Les agents IA permettent aux LLM (modèles de langage) d'évoluer, passant du rôle d'assistants à celui d'entités capables d'agir au nom des utilisateurs. Les agents peuvent même interagir avec d'autres agents pour réaliser des tâches. Parmi les attributs clés d'un agent, on retrouve un certain degré d'**autonomie**, permettant à l'agent d'initier des actions selon sa programmation, ce qui conduit à une capacité de **prise de décision** basée sur des objectifs prédéfinis. Ils sont également **adaptables**, car ils apprennent et s'ajustent pour améliorer leurs performances au fil du temps.

Un point essentiel à garder à l'esprit lors de la création d'agents est qu'ils doivent se concentrer sur une seule tâche. Vous devez définir leur objectif de manière aussi précise que possible.

> 🧑‍🏫**En savoir plus** : Découvrez les bases des agents IA dans [Generative AI for Beginners: AI Agents](https://github.com/microsoft/generative-ai-for-beginners/tree/main/17-ai-agents).

## Créer un agent IA

Nous allons explorer de nouveaux concepts pour construire un agent IA en .NET. Cela nécessitera l'utilisation d'un nouveau SDK et une configuration supplémentaire dans Microsoft Foundry pour démarrer.

> 🧑‍💻**Code d'exemple** : Nous travaillerons avec l'exemple [AgentLabs-01-Simple](../../../03-CoreGenerativeAITechniques/src/AgentLabs-01-Simple) pour cette leçon.
>
> Nous avons également inclus des exemples plus avancés dans le dossier `/src/`. Vous pouvez consulter les README de [AgentLabs-02-Functions](../../../03-CoreGenerativeAITechniques/src/AgentLabs-02-Functions), [AgentLabs-03-OpenAPIs](../../../03-CoreGenerativeAITechniques/src/AgentLabs-03-OpenAPIs) ou [AgentLabs-03-PythonParksInformationServer](../../../03-CoreGenerativeAITechniques/src/AgentLabs-03-PythonParksInformationServer) pour plus d'informations.

### Service Azure AI Agent

Nous allons introduire un nouveau service Azure qui nous aidera à créer des agents : le [Service Azure AI Agent](https://learn.microsoft.com/azure/ai-services/agents/overview).

Pour exécuter les exemples de code inclus dans cette leçon, vous devrez effectuer une configuration supplémentaire dans Microsoft Foundry. Suivez [ces instructions pour configurer un **agent de base**](https://learn.microsoft.com/azure/ai-services/agents/quickstart?pivots=programming-language-csharp).

### Bibliothèque Azure AI Projects

Les agents se composent de 3 parties principales : le **LLM** ou modèle, l'**état** ou contexte (similaire à une conversation) qui guide les décisions basées sur les résultats précédents, et les **outils**, qui sont comme [les fonctions que nous avons vues précédemment](./01-lm-completions-functions.md#function-calling), servant de passerelle entre le modèle et les systèmes externes.

En théorie, vous pourriez construire des agents IA avec les connaissances déjà acquises. Cependant, la bibliothèque **Azure AI Projects pour .NET** simplifie le développement d'agents en fournissant une API qui automatise de nombreuses tâches courantes.

Voici quelques concepts (qui correspondent à des classes) à comprendre lors de l'utilisation de la bibliothèque Azure AI Projects :

- `AgentClient` : Le client principal qui crée et héberge les agents, gère les threads sur lesquels ils s'exécutent, et gère la connexion au cloud.
- `Agent` : L'agent qui contient les instructions sur ce qu'il doit faire ainsi que les définitions des outils auxquels il a accès.
- `ThreadMessage` : Ce sont des messages - presque comme les invites que nous avons vues précédemment - qui sont transmis à l'agent. Les agents créent également des objets `ThreadMessage` pour communiquer.
- `ThreadRun` : Un thread sur lequel les messages sont transmis à l'agent. Le thread est démarré, peut recevoir des instructions supplémentaires, et son état peut être interrogé.

Voyons un exemple simple de tout cela en action !

### Construire un agent de mathématiques

Nous allons créer un agent dédié à une seule tâche : agir comme un tuteur pour les étudiants en mathématiques. Son unique objectif est de résoudre et d'expliquer les problèmes mathématiques posés par l'utilisateur.

1. Pour commencer, nous devons créer un objet `AgentsClient` responsable de la gestion de la connexion à Azure, de l'agent lui-même, des threads, des messages, etc.

    ```csharp
    string projectConnectionString = "< YOU GET THIS FROM THE PROJECT IN AI FOUNDRY >";
    AgentsClient client = new(projectConnectionString, new DefaultAzureCredential());
    ```

    Vous pouvez trouver la chaîne de connexion du projet dans AI Foundry en ouvrant le Hub que vous avez créé, puis le projet. Elle se trouve sur le côté droit.

    ![Capture d'écran de la page d'accueil du projet dans AI Foundry avec la chaîne de connexion du projet mise en évidence en rouge](../../../translated_images/project-connection-string.e9005630f6251f18a89cb8c08f54b33bc83e0765f4c4e4d694af2ff447c4dfef.fr.png)

1. Ensuite, nous voulons créer l'agent tuteur. Rappelez-vous, il doit être concentré sur une seule tâche.

    ```csharp
    Agent tutorAgent = (await client.CreateAgentAsync(
    model: "gpt-5-mini",
    name: "Math Tutor",
    instructions: "You are a personal math tutor. Write and run code to answer math questions.",
    tools: [new CodeInterpreterToolDefinition()])).Value;
    ```

    Quelques points importants ici. Le premier est le `tools` parameter. We're creating a `CodeInterpreterToolDefinition` object (that is apart of the **Azure.AI.Projects** SDK) that will allow the agent to create and execute code.

    > 🗒️**Note**: You can create your own tools too. See the [Functions](../../../03-CoreGenerativeAITechniques/src/AgentLabs-02-Functions) to learn more.

    Second note the `instructions` that are being sent along. It's a prompt and we're limiting it to answer math questions. Then last creating the agent is an async operation. That's because it's creating an object within Microsoft Foundry Agents service. So we both `await` the `CreateAgentAsync` function and then grab the `Value` pour accéder à l'objet `Agent` réel. Vous verrez ce schéma se répéter souvent lors de la création d'objets avec le SDK **Azure.AI.Projects**.

1. Un `AgentThread` est un objet qui gère la communication entre les agents individuels, les utilisateurs, etc. Nous devons le créer pour pouvoir y ajouter un `ThreadMessage`. Dans ce cas, il s'agit de la première question de l'utilisateur.

    ```csharp
    AgentThread thread = (await client.CreateThreadAsync()).Value;

    // Creating the first user message to AN agent - notice how we're putting it on a thread
    ThreadMessage userMessage = (await client.CreateMessageAsync(
        thread.Id,
        MessageRole.User,
        "Hello, I need to solve the equation `3x + 11 = 14`. Can you help me?")
    ).Value;
    ```

    Notez que le `ThreadMessage` a un type `MessageRole.User`. Et remarquez que nous n'envoyons pas le message à un agent spécifique, mais que nous le plaçons simplement sur un thread.

1. Ensuite, nous allons demander à l'agent de fournir une réponse initiale, la placer sur le thread, puis démarrer ce dernier. Lorsque nous démarrons le thread, nous fournissons l'identifiant initial de l'agent à exécuter ainsi que des instructions supplémentaires.

    ```csharp
    ThreadMessage agentMessage =  await client.CreateMessageAsync(
        thread.Id,
        MessageRole.Agent,
        "Please address the user as their name. The user has a basic account, so just share the answer to the question.")
    ).Value;

    ThreadRun run = (await client.CreateRunAsync(
        thread.Id,
        assistantId: agentMathTutor.Id, 
        additionalInstructions: "You are working in FREE TIER EXPERIENCE mode`, every user has premium account for a short period of time. Explain detailed the steps to answer the user questions")
    ).Value;
    ```

1. Tout ce qui reste à faire est de vérifier l'état de l'exécution.

    ```csharp
    do
    {
        await Task.Delay(Timespan.FromMilliseconds(100));
        run = (await client.GetRunAsync(thread.Id, run.Id)).Value;

        Console.WriteLine($"Run Status: {run.Status}");
    }
    while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);
    ```

1. Et enfin, afficher les messages des résultats.

    ```csharp
    Response<PageableList<ThreadMessage>> afterRunMessagesResponse = await client.GetMessagesAsync(thread.Id);
    IReadOnlyList<ThreadMessage> messages = afterRunMessagesResponse.Value.Data;

    // sort by creation date
    messages = messages.OrderBy(m => m.CreatedAt).ToList();

    foreach (ThreadMessage msg in messages)
    {
        Console.Write($"{msg.CreatedAt:yyyy-MM-dd HH:mm:ss} - {msg.Role,10}: ");

        foreach (MessageContent contentItem in msg.ContentItems)
        {
            if (contentItem is MessageTextContent textItem)
                Console.Write(textItem.Text);
        }
        Console.WriteLine();
    }
    ```

> 🙋 **Besoin d'aide ?** : Si vous rencontrez des problèmes, [ouvrez un ticket dans le dépôt](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

L'étape logique suivante est d'utiliser plusieurs agents pour créer un système autonome. Une idée pourrait être d'avoir un agent qui vérifie si l'utilisateur dispose d'un compte premium ou non.

## Résumé

Les agents IA sont des entités autonomes qui vont au-delà des simples interactions par chat - ils peuvent :

- **Prendre des décisions indépendantes** : Exécuter des tâches sans intervention humaine constante
- **Maintenir un contexte** : Conserver un état et se souvenir des interactions précédentes
- **Utiliser des outils** : Accéder à des systèmes externes et des API pour accomplir des tâches
- **Collaborer** : Travailler avec d'autres agents pour résoudre des problèmes complexes

Et vous avez appris à utiliser le service **Azure AI Agents** avec le SDK **Azure AI Project** pour créer un agent rudimentaire.

Pensez aux agents comme des assistants IA dotés d'une autonomie - ils ne se contentent pas de répondre, ils agissent en fonction de leur programmation et de leurs objectifs.

## Ressources supplémentaires

- [Créer un agent minimal avec .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-assistants?pivots=openai)
- [Orchestration multi-agents](https://techcommunity.microsoft.com/blog/educatordeveloperblog/using-azure-ai-agent-service-with-autogen--semantic-kernel-to-build-a-multi-agen/4363121)
- [Agents IA - Série pour débutants sur l'IA générative](https://github.com/microsoft/generative-ai-for-beginners/tree/main/17-ai-agents)

## Prochaines étapes

Vous avez parcouru un long chemin ! Depuis l'apprentissage des complétions simples jusqu'à la création d'agents !

👉 [Dans la prochaine leçon, découvrez des exemples pratiques](../04-PracticalSamples/readme.md) d'utilisation de tout ce que vous avez appris.

**Avertissement** :  
Ce document a été traduit à l'aide de services de traduction automatique basés sur l'intelligence artificielle. Bien que nous nous efforcions d'assurer l'exactitude, veuillez noter que les traductions automatisées peuvent contenir des erreurs ou des inexactitudes. Le document original dans sa langue d'origine doit être considéré comme la source faisant autorité. Pour des informations critiques, il est recommandé de recourir à une traduction humaine professionnelle. Nous déclinons toute responsabilité en cas de malentendus ou de mauvaises interprétations résultant de l'utilisation de cette traduction.
