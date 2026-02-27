# Ollamaを使った開発環境のセットアップ

このコースでローカルモデルを実行するためにOllamaを使用したい場合は、このガイドの手順に従ってください。

Azure OpenAIを使用したくないですか？

👉 [Azure OpenAIを使用する場合はこちらのガイドをご覧ください](README.md)  
👉 [Ollamaを使用する手順はこちら](getting-started-ollama.md)

## GitHub Codespaceの作成

このコースを通して開発を行うためのGitHub Codespaceを作成しましょう。

1. [こちらを右クリック](https://github.com/microsoft/Generative-AI-for-beginners-dotnet)して、コンテキストメニューから**新しいウィンドウで開く**を選択して、このリポジトリのメインページを新しいウィンドウで開きます。  
1. ページの右上にある**Fork**ボタンをクリックして、このリポジトリを自分のGitHubアカウントにフォークします。  
1. **Code**ドロップダウンボタンをクリックし、**Codespaces**タブを選択します。  
1. **...**オプション（三点リーダー）を選択し、**New with options...**をクリックします。

![カスタムオプションでCodespaceを作成する](../../../translated_images/creating-codespace.0e7334f85cf4c8d0e080a0d5b4c76c24c5bbe6bddf48dcd1403e092ea0d9bce9.ja.png)

### 開発用コンテナの選択

**Dev container configuration**ドロップダウンから以下のいずれかのオプションを選択してください：

**オプション1: C# (.NET)** : Azure OpenAIまたはAzure OpenAIを使用する予定の場合、このオプションを選択してください。このコースを進める上で推奨される方法です。必要な.NET開発ツールがすべて含まれており、起動も高速です。

**オプション2: C# (.NET) - Ollama** : Ollamaを使ってローカルでモデルを実行したい場合はこちらを選択してください。必要な.NET開発ツールに加えてOllamaも含まれていますが、起動に平均5分程度かかります。[こちらのガイド](getting-started-ollama.md)に従ってOllamaを使用してください。

他の設定はそのままにしておいて問題ありません。**Create codespace**ボタンをクリックして、Codespaceの作成プロセスを開始します。

![開発用コンテナの設定を選択する](../../../translated_images/select-container-codespace.9b8ca34b6ff8b4cb80973924cbc1894cf7672d233b0055b47f702db60c4c6221.ja.png)

## CodespaceがOllamaで正しく動作していることを確認する

Codespaceが完全に読み込まれ、設定が完了したら、サンプルアプリを実行してすべてが正常に動作していることを確認しましょう：

1. ターミナルを開きます。macOSでは**Ctrl+\`** (backtick) on Windows or **Cmd+`**を押してターミナルウィンドウを開くことができます。

1. 次のコマンドを実行して適切なディレクトリに移動します：

    ```bash
    cd 02-SetupDevEnvironment/src/BasicChat-03Ollama/
    ```

1. 次に、以下のコマンドでアプリケーションを実行します：

    ```bash
    dotnet run
    ```

1. 数秒かかる場合がありますが、最終的には次のようなメッセージが出力されるはずです：

    ```bash
    AI, or Artificial Intelligence, refers to the development of computer systems that can perform tasks that typically require human intelligence, such as:

    1. Learning: AI systems can learn from data and improve their performance over time.
    2. Reasoning: AI systems can draw conclusions and make decisions based on the data they have been trained on.
    
    ...
    ```

> 🙋 **助けが必要ですか？**: 何かがうまくいかない場合は、[こちらから問題を報告してください](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue)。サポートします。

## Ollamaでモデルを切り替える

Ollamaの便利な点の1つは、モデルを簡単に切り替えられることです。現在のアプリは"**llama3.2**"モデルを使用しています。次に、"**phi3.5**"モデルを試してみましょう。

1. ターミナルで次のコマンドを実行してPhi3.5モデルをダウンロードします：

    ```bash
    ollama pull phi3.5
    ```

    [Phi3.5](https://ollama.com/library/phi3.5)や他の利用可能なモデルについては、[Ollamaライブラリ](https://ollama.com/library/)をご覧ください。

1. エディタで `02-SetupDevEnvironment/src/BasicChat-03Ollama/Program.cs` を開きます。

    6-7行目を変更して、新しいモデルを使用するようにチャットクライアントの初期化を更新します：

    ```csharp
    IChatClient client =
        new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3.5");
    ```

1. 最後に、以下のコマンドでアプリケーションを実行します：

    ```bash
    dotnet run
    ```

1. 新しいモデルに切り替わりました。応答がより長く、詳細になっていることに気付くでしょう。

    ```bash
    Artificial Intelligence (AI) refers to the simulation of human intelligence processes by machines, especially computer systems. These processes include learning (the acquisition of information and accumulation of knowledge), reasoning (using the acquired knowledge to make deductions or decisions), and self-correction. AI can manifest in various forms:

    1. **Narrow AI** – Designed for specific tasks, such as facial recognition software, voice assistants like Siri or Alexa, autonomous vehicles, etc., which operate under a limited preprogrammed set of behaviors and rules but excel within their domain when compared to humans in these specialized areas.

    2. **General AI** – Capable of understanding, learning, and applying intelligence broadly across various domains like human beings do (natural language processing, problem-solving at a high level). General AIs are still largely theoretical as we haven't yet achieved this form to the extent necessary for practical applications beyond narrow tasks.
    
    ...
    ```

> 🙋 **助けが必要ですか？**: 何かがうまくいかない場合は、[こちらから問題を報告してください](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue)。サポートします。

## まとめ

このレッスンでは、コース全体で使用する開発環境をセットアップする方法を学びました。GitHub Codespaceを作成し、Ollamaを使用するように設定しました。また、サンプルコードを更新して簡単にモデルを変更する方法も学びました。

### 追加リソース

- [Ollama Models](https://ollama.com/search)  
- [GitHub Codespacesの使用方法](https://docs.github.com/en/codespaces/getting-started)  
- [Microsoft Extensions for AI ドキュメント](https://learn.microsoft.com/dotnet/)  

## 次のステップ

次に、最初のAIアプリケーションを作成する方法を学びます！ 🚀

👉 [コア生成AIテクニック](../03-CoreGenerativeAITechniques/readme.md)

**免責事項**:  
この文書は、機械ベースのAI翻訳サービスを使用して翻訳されています。正確性を追求していますが、自動翻訳には誤りや不正確さが含まれる場合があります。元の言語で作成された原文が、公式な情報源として考慮されるべきです。重要な情報については、専門の人間による翻訳をお勧めします。本翻訳の使用に起因する誤解や解釈の誤りについて、当社は一切の責任を負いません。