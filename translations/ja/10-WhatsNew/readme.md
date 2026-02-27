# 初心者向け生成AI .NETの最新情報

このページでは、コースに追加された新機能、ツール、モデルの履歴を追跡しています。定期的に更新をチェックしてください！

## 2025年6月

### 新機能：Azure OpenAI Sora動画生成デモ

- **新しいレッスン3サンプル：Azure Sora動画生成**
- レッスン3では、Azure OpenAIの新しい[Sora動画生成モデル](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)を使用してテキストプロンプトから動画を生成する実践的なデモを提供しています。
- このサンプルでは以下の方法を学習できます：
  - 創造的なプロンプトで動画生成ジョブを送信する。
  - ジョブの状態をポーリングして生成された動画ファイルを自動的にダウンロードする。
  - 生成された動画をデスクトップに保存して簡単に表示する。
- 公式ドキュメントを確認：[Azure OpenAI Sora動画生成](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- サンプルはこちら：[レッスン3：コア生成AI技術 /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

### 新しいeShopLiteシナリオ：並行エージェントオーケストレーション（2025年6月）

- **新シナリオ：並行エージェントオーケストレーション**
- [eShopLiteリポジトリ](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent)では、Microsoft Agent Frameworkを使用した並行エージェントオーケストレーションを示すシナリオを提供しています。
- このシナリオでは、複数のエージェントが並行して動作してユーザークエリを分析し、将来の分析に役立つ貴重な洞察を提供する方法を紹介しています。

## 2025年5月

### Azure OpenAI画像生成モデル：gpt-image-1

- **新しいレッスン3サンプル：Azure OpenAI画像生成**
  - レッスン3では、新しいAzure OpenAI画像生成モデル`gpt-image-1`を使用するコードサンプルと説明を含んでいます。
  - .NETから最新のAzure OpenAI機能を使用して画像を生成する方法を学習できます。
  - [公式Azure OpenAI DALL·Eドキュメント](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1)と[openai-dotnet画像生成ガイド](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images)で詳細を確認してください。
  - サンプルはこちら：[レッスン3：コア生成AI技術](../../../03-CoreGenerativeAITechniques/)。

### AI ToolkitとDockerを使用してローカルモデルを実行

- **新機能：AI ToolkitとDockerを使用してローカルモデルを実行**：[Visual Studio Code用AI Toolkit](https://code.visualstudio.com/docs/intelligentapps/overview)と[Docker Model Runner](https://docs.docker.com/model-runner/)を使用してローカルでモデルを実行する新しいサンプルを探索してください。ソースコードは[../../../samples/CoreSamples/](../../../samples/CoreSamples/)にあり、Microsoft.Extensions.AIを使用してこれらのモデルと相互作用する方法を示しています。

## 2025年3月

### MCPライブラリ統合

- **.NET用モデルコンテキストプロトコル**：異なるプロバイダーのAIモデルと通信するための標準化された方法を提供する[MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)のサポートを追加しました。
- この統合により、プロバイダーロックインを削減しながら、モデルとのより一貫した相互作用が可能になります。
- [コア生成AI技術](../../../03-CoreGenerativeAITechniques/)セクションでMCP統合を示す新しいサンプルをご確認ください。

### eShopLiteシナリオリポジトリ

- **新しいeShopLiteリポジトリ**：すべてのeShopLiteシナリオが単一のリポジトリで利用可能になりました：[https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- 新しいリポジトリには以下が含まれます：
  - 製品カタログブラウジング
  - ショッピングカート管理
  - 注文配置と追跡
  - ユーザー認証とプロファイル
  - 推奨とチャット用の生成AI統合
  - 製品と注文管理用の管理者ダッシュボード