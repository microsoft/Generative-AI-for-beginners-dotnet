# AI ToolkitとDockerを使用したモデルのローカル実行

このレッスンでは、AI ToolkitとDockerを使用して生成AIモデルをデバイスやクラウド環境でローカルに実行する方法について学びます。

---

## はじめに

AIモデルをローカルで実行する能力は、プライバシー、コスト削減、モデル実行の完全な制御など、複数の利点を提供します。このレッスンでは、Microsoft AI ToolkitとDockerを使用してさまざまなモデルを実行する方法を学びます。

## Microsoft AI Toolkit

Microsoft AI Toolkitは、ローカルのAIモデルを.NETアプリケーションに統合できるようにするツールとライブラリのコレクションです。様々なモデルタイプやタスクをサポートしています。

### AI Toolkitと.NETの使用

以下は.NETアプリケーションでAI Toolkitを使用する例です：

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
chat.AddUserMessage("量子コンピューティングをわかりやすく説明してください");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## AIモデル用のDocker

Dockerは、アプリケーションを分離されたコンテナで実行できるプラットフォームです。一貫した環境でAIモデルを実行するための強力なツールです。

### Dockerを使ったモデルの実行

Dockerを通じてさまざまなAIモデルを実行することができます：

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### サンプルアプリケーション

[DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat)と[DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat)のサンプルでは、Semantic KernelとMicrosoft.Extensions.AIの両方を使用してローカルモデルを活用するアプリケーションを実装しています。

## まとめ

モデルをローカルで実行することで、AIアプリケーションの制御を強化し、コストを削減することができます。Microsoft AI ToolkitとDockerを使えば、.NETアプリケーションで幅広いモデルを展開することができます。

**免責事項**:  
この文書は、機械ベースのAI翻訳サービスを使用して翻訳されています。正確性を期すよう努めていますが、自動翻訳には誤りや不正確さが含まれる場合があります。元の言語で記載された原文が正式な情報源と見なされるべきです。重要な情報については、専門の人間による翻訳をお勧めします。この翻訳の使用に起因する誤解や解釈の誤りについて、当方は責任を負いません。
