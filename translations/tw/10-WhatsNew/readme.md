# 生成式 AI 初學者 .NET 最新消息

本頁追蹤課程中新增功能、工具和模型的歷史記錄。請定期回來查看更新！

## 2025年6月

### 新功能：Azure OpenAI Sora 影片產生示範

- **新增第3課範例：Azure Sora 影片產生**
- 第3課現在包含實作示範，展示如何使用新的 [Sora 影片產生模型](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) 在 Azure OpenAI 中從文字提示生成影片。
- 這個範例展示如何：
  - 使用創意提示提交影片產生工作。
  - 輪詢工作狀態並自動下載產生的影片檔案。
  - 將產生的影片儲存到桌面以便輕鬆檢視。
- 查看官方文件：[Azure OpenAI Sora 影片產生](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- 在這裡找到範例：[第3課：核心生成式 AI 技術 /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

### 新增 eShopLite 情境：並發代理協調（2025年6月）

- **新情境：並發代理協調**
- [eShopLite 存放庫](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) 現在提供一個情境，展示如何使用 Microsoft Agent Framework 進行並發代理協調。
- 這個情境展示了多個代理如何並行工作來分析使用者查詢並為未來分析提供有價值的洞察。

## 2025年5月

### Azure OpenAI 圖像產生模型：gpt-image-1

- **新增第3課範例：Azure OpenAI 圖像產生**
  - 第3課現在包含使用新的 Azure OpenAI 圖像產生模型 `gpt-image-1` 的程式碼範例和說明。
  - 學習如何直接從 .NET 使用最新的 Azure OpenAI 功能產生圖像。
  - 查看 [官方 Azure OpenAI DALL·E 文件](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) 和 [openai-dotnet 圖像產生指南](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) 了解更多詳情。
  - 在這裡找到範例：[第3課：核心生成式 AI 技術](../../../03-CoreGenerativeAITechniques/)。

### 使用 AI Toolkit 和 Docker 執行本地模型

- **新功能：使用 AI Toolkit 和 Docker 執行本地模型**：探索使用 [Visual Studio Code 的 AI Toolkit](https://code.visualstudio.com/docs/intelligentapps/overview) 和 [Docker Model Runner](https://docs.docker.com/model-runner/) 本地執行模型的新範例。原始碼位於 [../../../samples/CoreSamples/](../../../samples/CoreSamples/)，展示了如何使用 Microsoft.Extensions.AI 與這些模型進行互動。

## 2025年3月

### MCP 程式庫整合

- **適用於 .NET 的模型內容協定**：我們新增了對 [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) 的支援，它提供了一種標準化的方式來與不同提供者的 AI 模型進行通訊。
- 這種整合使模型互動更加一致，同時減少對供應商的依賴。
- 查看我們在 [核心生成式 AI 技術](../../../03-CoreGenerativeAITechniques/) 部分展示 MCP 整合的新範例。

### eShopLite 情境存放庫

- **新的 eShopLite 存放庫**：所有 eShopLite 情境現在都可以在單一存放庫中找到：[https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- 新存放庫包括：
  - 產品目錄瀏覽
  - 購物車管理
  - 訂單下達和追蹤
  - 使用者驗證和個人檔案
  - 與生成式 AI 整合的推薦和聊天功能
  - 用於產品和訂單管理的管理員儀表板