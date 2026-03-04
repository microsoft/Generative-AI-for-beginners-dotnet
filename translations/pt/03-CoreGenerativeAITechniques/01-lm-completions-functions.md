# Noções Básicas de Aplicativos de Chat


---

## Conclusões de texto e chat

[![Vídeo sobre conclusões de texto e chat](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️Clique na imagem para assistir ao vídeo⬆️_

As conclusões de texto podem ser a forma mais básica de interação com o modelo de linguagem em uma aplicação de IA. Uma conclusão de texto é uma única resposta gerada pelo modelo com base na entrada, ou prompt, fornecida a ele.

Por si só, uma conclusão de texto não é um aplicativo de chat; trata-se de uma interação pontual. Você pode usar conclusões de texto para tarefas como resumo de conteúdo ou análise de sentimentos.

### Conclusões de texto

Vamos ver como você pode usar conclusões de texto utilizando a biblioteca **Microsoft.Extensions.AI** no .NET.

> 🧑‍💻**Código de exemplo**: [Aqui está um exemplo funcional desta aplicação](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI) para você acompanhar.

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

> 🗒️**Nota:** Este exemplo utilizou os Azure OpenAI como serviço de hospedagem. Se você quiser usar o Ollama, [confira este exemplo](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama) (ele instancia um `IChatClient` diferente).
>
> Se você quiser usar o Microsoft Foundry, pode usar o mesmo código, mas será necessário alterar o endpoint e as credenciais.

> 🙋 **Precisa de ajuda?**: Se encontrar algum problema, [abra uma issue no repositório](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

### Aplicativos de chat

Construir um aplicativo de chat é um pouco mais complexo. Haverá uma conversa com o modelo, onde o usuário pode enviar prompts e o modelo responde. E, como em qualquer conversa, é necessário manter o contexto, ou histórico, da conversa para que tudo faça sentido.

#### Diferentes tipos de papéis no chat

Durante uma conversa com o modelo, as mensagens enviadas podem ter diferentes tipos. Aqui estão alguns exemplos:

* **Sistema**: A mensagem do sistema orienta o comportamento das respostas do modelo. Ela serve como a instrução inicial ou prompt que define o contexto, o tom e os limites da conversa. O usuário final geralmente não vê essa mensagem, mas ela é muito importante para moldar a interação.
* **Usuário**: A mensagem do usuário é a entrada ou prompt do usuário final. Pode ser uma pergunta, uma declaração ou um comando. O modelo usa essa mensagem para gerar uma resposta.
* **Assistente**: A mensagem do assistente é a resposta gerada pelo modelo. Essas mensagens são baseadas nas mensagens de sistema e usuário e são geradas pelo modelo. O usuário final vê essas mensagens.

#### Gerenciando o histórico do chat

Durante a conversa com o modelo, será necessário acompanhar o histórico do chat. Isso é importante porque o modelo gera respostas com base na mensagem do sistema e em todas as interações entre as mensagens do usuário e do assistente. Cada mensagem adicional adiciona mais contexto que o modelo usa para gerar a próxima resposta.

Vamos dar uma olhada em como construir um aplicativo de chat usando o MEAI.

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


> 🙋 **Precisa de ajuda?**: Se encontrar algum problema, [abra uma issue no repositório](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Chamadas de função

[![Vídeo explicativo sobre funções](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️Clique na imagem para assistir ao vídeo⬆️_

Ao construir aplicações de IA, você não está limitado apenas a interações baseadas em texto. É possível estender a funcionalidade do chatbot chamando funções pré-definidas no seu código com base na entrada do usuário. Em outras palavras, as chamadas de função servem como uma ponte entre o modelo e sistemas externos.

> 🧑‍💻**Código de exemplo**: [Aqui está um exemplo funcional desta aplicação](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions) para você acompanhar.

### Chamadas de função em aplicativos de chat

Existem alguns passos de configuração que você precisa seguir para chamar funções com o MEAI.

1. Primeiro, é claro, defina a função que você quer que o chatbot seja capaz de chamar. Neste exemplo, vamos obter a previsão do tempo.

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

```

1. Em seguida, criaremos um objeto `ChatOptions` que informará ao MEAI quais funções estão disponíveis para ele.

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

```

1. Ao instanciar o objeto `IChatClient`, especificaremos que usaremos a invocação de funções.

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

1. Por fim, ao interagir com o modelo, enviaremos o objeto `ChatOptions` que especifica a função que o modelo pode chamar caso precise obter informações sobre o clima.

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **Precisa de ajuda?**: Se encontrar algum problema, [abra uma issue no repositório](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## Resumo

Nesta lição, aprendemos a usar conclusões de texto, iniciar e gerenciar uma conversa de chat, e chamar funções em aplicativos de chat.

Na próxima lição, você verá como começar a conversar com dados e construir o que é conhecido como um modelo de chatbot de Recuperação e Geração Aumentada (RAG) - além de trabalhar com visão e áudio em uma aplicação de IA!

## Recursos adicionais

* [Crie um aplicativo de chat de IA com .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [Execute uma função local do .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [Converse com um modelo de IA local](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## A seguir

👉 [Vamos construir um aplicativo RAG!](./02-retrieval-augmented-generation.md)

**Aviso Legal**:  
Este documento foi traduzido utilizando serviços de tradução automática baseados em IA. Embora nos esforcemos para garantir a precisão, esteja ciente de que traduções automáticas podem conter erros ou imprecisões. O documento original em seu idioma nativo deve ser considerado a fonte oficial. Para informações críticas, recomenda-se a tradução profissional humana. Não nos responsabilizamos por quaisquer mal-entendidos ou interpretações equivocadas decorrentes do uso desta tradução.
