# 채팅 앱 기초

이 강의에서는 .NET에서 언어 모델 완성과 함수를 사용하여 채팅 애플리케이션을 구축하는 기본 사항을 살펴보겠습니다. 또한 Microsoft Extensions AI(MEAI)를 사용하여 챗봇을 만드는 방법과, Microsoft.Extensions.AI를 사용하여 사용자의 입력에 따라 챗봇이 호출하는 플러그인 또는 기능을 만드는 방법도 알아볼 것입니다.

---

## 텍스트 완성과 채팅

[![텍스트 완성과 채팅 비디오](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️이미지를 클릭하여 비디오를 시청하세요⬆️_

텍스트 완성은 AI 애플리케이션에서 언어 모델과 상호작용하는 가장 기본적인 형태일 수 있습니다. 텍스트 완성은 모델에 제공된 입력 또는 프롬프트를 기반으로 모델이 생성한 단일 응답입니다.

텍스트 완성 자체는 채팅 애플리케이션이 아닙니다. 단발성 상호작용입니다. 텍스트 완성은 콘텐츠 요약이나 감정 분석과 같은 작업에 사용할 수 있습니다.

### 텍스트 완성

.NET의 **Microsoft.Extensions.AI** 라이브러리를 사용하여 텍스트 완성을 사용하는 방법을 살펴보겠습니다.

> 🧑‍💻**샘플 코드**: [이 애플리케이션의 작동 예제](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI)를 따라해 보세요.

```csharp

// this example illustrates using a model hosted on Azure OpenAI
IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
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

> 🗒️**참고:** 이 예제는 Azure OpenAI를 호스팅 서비스로 사용하는 방법을 보여줍니다. Ollama를 사용하려면 [이 예제를 확인하세요](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama) (다른 `IChatClient`를 인스턴스화합니다).
>
> Microsoft Foundry를 사용하려면 동일한 코드를 사용할 수 있지만, 엔드포인트와 자격 증명을 변경해야 합니다.

> 🙋 **도움이 필요하신가요?**: 문제가 발생하면 [저장소에 이슈를 열어주세요](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

### 채팅 애플리케이션

채팅 애플리케이션을 구축하는 것은 조금 더 복잡합니다. 모델과의 대화에서 사용자가 프롬프트를 보내고 모델이 응답하는 형태가 됩니다. 그리고 대화가 자연스럽게 이어지도록 대화의 맥락, 즉 대화 기록을 유지해야 합니다.

#### 다양한 채팅 역할 유형

모델과의 채팅 중에 모델에 전송되는 메시지는 다양한 유형일 수 있습니다. 다음은 몇 가지 예입니다:

* **시스템**: 시스템 메시지는 모델 응답의 동작을 안내합니다. 대화의 맥락, 톤, 경계를 설정하는 초기 지침 또는 프롬프트 역할을 합니다. 이 메시지는 일반적으로 최종 사용자가 보지 못하지만, 대화의 방향을 결정하는 데 매우 중요합니다.
* **사용자**: 사용자 메시지는 최종 사용자가 입력한 질문, 진술 또는 명령입니다. 모델은 이 메시지를 사용하여 응답을 생성합니다.
* **어시스턴트**: 어시스턴트 메시지는 모델이 생성한 응답입니다. 이 메시지는 시스템 및 사용자 메시지를 기반으로 하며 모델에 의해 생성됩니다. 최종 사용자는 이 메시지를 보게 됩니다.

#### 채팅 기록 관리

모델과의 채팅 중에는 대화 기록을 추적해야 합니다. 이는 시스템 메시지와 사용자 및 어시스턴트 메시지 간의 모든 상호작용을 기반으로 모델이 응답을 생성하기 때문에 중요합니다. 추가되는 각 메시지는 모델이 다음 응답을 생성하는 데 사용하는 더 많은 맥락을 제공합니다.

MEAI를 사용하여 채팅 애플리케이션을 구축하는 방법을 살펴보겠습니다.

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


> 🙋 **도움이 필요하신가요?**: 문제가 발생하면 [저장소에 이슈를 열어주세요](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## 함수 호출

[![함수 설명 비디오](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️이미지를 클릭하여 비디오를 시청하세요⬆️_

AI 애플리케이션을 구축할 때 단순히 텍스트 기반 상호작용에만 제한되지 않습니다. 사용자의 입력을 기반으로 코드에서 미리 정의된 함수를 호출하여 챗봇의 기능을 확장할 수 있습니다. 즉, 함수 호출은 모델과 외부 시스템 간의 다리 역할을 합니다.

> 🧑‍💻**샘플 코드**: [이 애플리케이션의 작동 예제](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions)를 따라해 보세요.

### 채팅 애플리케이션에서 함수 호출

MEAI로 함수를 호출하려면 몇 가지 설정 단계가 필요합니다.

1. 먼저, 챗봇이 호출할 수 있는 함수를 정의합니다. 이 예제에서는 날씨 예보를 가져옵니다.

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

```

2. 다음으로, MEAI에 사용할 수 있는 함수를 알려주는 `ChatOptions` 객체를 생성합니다.

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

```

3. `IChatClient` 객체를 인스턴스화할 때 함수 호출을 사용할 것임을 지정합니다.

    ```csharp
    IChatClient client = new AzureOpenAIClient(
    new Uri(config["endpoint"]),
    new ApiKeyCredential(config["apikey"]))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

4. 마지막으로 모델과 상호작용할 때, 날씨 정보를 가져오는 함수가 필요할 경우 호출할 수 있도록 `ChatOptions` 객체를 전송합니다.

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **도움이 필요하신가요?**: 문제가 발생하면 [저장소에 이슈를 열어주세요](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

## 요약

이 강의에서는 텍스트 완성을 사용하는 방법, 채팅 대화를 시작하고 관리하는 방법, 그리고 채팅 애플리케이션에서 함수를 호출하는 방법을 배웠습니다.

다음 강의에서는 데이터를 활용한 채팅을 시작하고 Retrieval Augmented Generation (RAG) 모델 챗봇을 구축하는 방법, 그리고 AI 애플리케이션에서 비전 및 오디오를 다루는 방법을 배울 것입니다!

## 추가 자료

* [.NET으로 AI 채팅 앱 빌드하기](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [로컬 .NET 함수 실행하기](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [로컬 AI 모델과 채팅하기](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## 다음 강의

👉 [RAG 앱을 만들어봅시다!](./02-retrieval-augmented-generation.md)

**면책 조항**:  
이 문서는 기계 기반 AI 번역 서비스를 사용하여 번역되었습니다. 정확성을 위해 최선을 다하고 있지만, 자동 번역에는 오류나 부정확성이 포함될 수 있습니다. 원본 문서의 모국어 버전이 권위 있는 자료로 간주되어야 합니다. 중요한 정보의 경우, 전문적인 인간 번역을 권장합니다. 이 번역 사용으로 인해 발생하는 오해나 잘못된 해석에 대해 당사는 책임을 지지 않습니다.
