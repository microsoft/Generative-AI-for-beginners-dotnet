# チャットアプリの基本

このレッスンでは、.NETを使用して言語モデルの補完機能や関数を活用したチャットアプリケーションの基本を探ります。また、セマンティック カーネルやMicrosoft Extensions AI (MEAI) を使用してチャットボットを作成する方法についても学びます。さらに、セマンティック カーネルを使用してプラグイン（ユーザーの入力に基づいてチャットボットが呼び出す機能）を作成する方法も紹介します。

---

## テキスト補完とチャット

[![テキスト補完とチャットの動画](https://img.youtube.com/vi/Av1FCQf83QU/0.jpg)](https://youtu.be/Av1FCQf83QU?feature=shared)

_⬆️画像をクリックして動画を見る⬆️_

テキスト補完は、AIアプリケーションにおける言語モデルとの最も基本的な形のやり取りかもしれません。テキスト補完とは、モデルに与えられた入力（プロンプト）に基づいて生成される単一の応答のことです。

テキスト補完自体はチャットアプリケーションではなく、一回限りのやり取りです。例えば、コンテンツの要約や感情分析のようなタスクでテキスト補完を使用することがあります。

### テキスト補完

.NETの **Microsoft.Extensions.AI** ライブラリを使用して、テキスト補完をどのように実現するか見てみましょう。

> 🧑‍💻**サンプルコード**: [このアプリケーションの動作例はこちら](../../../03-CoreGenerativeAITechniques/src/BasicChat-01MEAI) を参考にしてください。

```csharp

// this example illustrates using a model hosted on GitHub Models
IChatClient client = new ChatCompletionsClient(
    endpoint: new Uri("https://models.github.ai/inference"),
    new AzureKeyCredential(githubToken)) // githubToken is retrieved from the environment variables
    .AsChatClient("gpt-4o-mini");

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

> 🗒️**Note:** この例ではGitHub Modelsをホスティングサービスとして使用しています。Ollamaを使用したい場合は、[こちらの例](../../../03-CoreGenerativeAITechniques/src/BasicChat-03Ollama) をご覧ください（異なる `IChatClient` をインスタンス化しています）。
>
> Microsoft Foundryを使用したい場合は、同じコードを使用できますが、エンドポイントと認証情報を変更する必要があります。

> 🙋 **Need help?**: 問題が発生した場合は、[リポジトリでIssueを開いてください](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

### チャットアプリケーション

チャットアプリケーションを構築するのは少し複雑です。ユーザーがプロンプトを送信し、モデルが応答するという会話が行われます。そして、どんな会話でもそうですが、会話のコンテキストや履歴を保持して、全体の流れが意味をなすようにする必要があります。

#### チャットの役割の種類

モデルとのチャット中に送信されるメッセージには、さまざまな種類があります。以下はいくつかの例です：

* **システム**: システムメッセージは、モデルの応答の振る舞いを導くものです。会話のコンテキスト、トーン、境界を設定する初期の指示やプロンプトとして機能します。このメッセージは通常、チャットのエンドユーザーには見えませんが、会話の方向性を形作る上で非常に重要です。
* **ユーザー**: ユーザーメッセージは、エンドユーザーからの入力やプロンプトです。質問、声明、またはコマンドである可能性があります。モデルはこのメッセージを基に応答を生成します。
* **アシスタント**: アシスタントメッセージは、モデルによって生成された応答です。これらのメッセージは、システムメッセージとユーザーメッセージを基にして生成され、エンドユーザーに表示されます。

#### チャット履歴の管理

モデルとのチャット中には、チャット履歴を追跡する必要があります。これは、モデルがシステムメッセージに基づいて応答を生成し、さらにユーザーとアシスタント間のやり取りをすべて考慮して次の応答を生成するために重要です。各メッセージが追加されることで、モデルが次の応答を生成するためのコンテキストが増えます。

MEAIを使用してチャットアプリケーションを構築する方法を見てみましょう。

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

> 🗒️**Note:** レガシー Semantic Kernel サンプルについては、[廃止予定のサンプル](../../../samples/deprecated/)フォルダーを参照してください。

> 🙋 **Need help?**: 問題が発生した場合は、[リポジトリでIssueを開いてください](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## 関数呼び出し

[![関数の解説動画](https://img.youtube.com/vi/i84GijmGlYU/0.jpg)](https://youtu.be/i84GijmGlYU?feature=shared)

_⬆️画像をクリックして動画を見る⬆️_

AIアプリケーションを構築する際、テキストベースのやり取りに限定される必要はありません。ユーザー入力に基づいてコード内の事前定義された関数を呼び出すことで、チャットボットの機能を拡張することが可能です。言い換えれば、関数呼び出しはモデルと外部システムをつなぐ橋渡しとして機能します。

> 🧑‍💻**サンプルコード**: [このアプリケーションの動作例はこちら](../../../03-CoreGenerativeAITechniques/src/MEAIFunctions) を参考にしてください。

### チャットアプリケーションでの関数呼び出し

MEAIを使用して関数を呼び出すには、いくつかの設定手順が必要です。

1. まず、チャットボットが呼び出せるようにする関数を定義します。この例では天気予報を取得します。

    ```csharp

    [Description("Get the weather")]
    static string GetTheWeather()
    {    
        var temperature = Random.Shared.Next(5, 20);

        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    }

    ```

2. 次に、MEAIに利用可能な関数を指定するための `ChatOptions` オブジェクトを作成します。

    ```csharp

    var chatOptions = new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(GetTheWeather)]
    };

    ```

3. `IChatClient` オブジェクトをインスタンス化する際に、関数呼び出しを使用することを指定します。

    ```csharp
    IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken)) // githubToken is retrieved from the environment variables
    .AsChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()  // here we're saying that we could be invoking functions!
    .Build();
    ```

4. 最後に、モデルとやり取りする際に、天気情報を取得するための関数をモデルが呼び出せるようにする `ChatOptions` オブジェクトを送信します。

    ```csharp
    var responseOne = await client.GetResponseAsync("What is today's date", chatOptions); // won't call the function

    var responseTwo = await client.GetResponseAsync("Should I bring an umbrella with me today?", chatOptions); // will call the function
    ```

> 🙋 **Need help?**: 問題が発生した場合は、[リポジトリでIssueを開いてください](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new)。

## まとめ

このレッスンでは、テキスト補完を使用する方法、チャット会話を開始して管理する方法、チャットアプリケーションで関数を呼び出す方法について学びました。

次のレッスンでは、データとのチャットを開始し、RAG（Retrieval Augmented Generation）モデルのチャットボットを構築する方法、そしてAIアプリケーションでビジョンやオーディオを活用する方法を学びます！

## 追加リソース

* [.NETでAIチャットアプリを構築する](https://learn.microsoft.com/dotnet/ai/quickstarts/get-started-openai?tabs=azd&pivots=openai)
* [ローカル.NET関数を実行する](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-azure-openai-tool?tabs=azd&pivots=openai)
* [ローカルAIモデルとチャットする](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-local-ai)

## 次に進む

👉 [RAGアプリを構築してみましょう！](./02-retrieval-augmented-generation.md)

**免責事項**:  
この文書は、機械翻訳AIサービスを使用して翻訳されています。正確性を追求していますが、自動翻訳には誤りや不正確な部分が含まれる可能性があります。原文（原言語で記載された文書）が正式な情報源として考慮されるべきです。重要な情報については、専門の人間による翻訳を推奨します。この翻訳の使用に起因する誤解や誤解釈について、当社は一切の責任を負いません。
