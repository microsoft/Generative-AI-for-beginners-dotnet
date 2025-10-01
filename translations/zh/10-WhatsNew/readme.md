# 生成式 AI 初学者 .NET 最新更新

本页面记录了课程中新增功能、工具和模型的历史。请定期查看更新！

## 2025年6月

### 新增：Azure OpenAI Sora 视频生成演示

- **新增第3课示例：Azure Sora 视频生成**
- 第3课现在提供实践演示，展示如何使用新的 [Sora 视频生成模型](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) 在 Azure OpenAI 中从文本提示生成视频。
- 该示例演示如何：
  - 使用创意提示提交视频生成任务。
  - 轮询任务状态并自动下载生成的视频文件。
  - 将生成的视频保存到桌面以便轻松查看。
- 查看官方文档：[Azure OpenAI Sora 视频生成](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- 在这里找到示例：[第3课：核心生成式 AI 技术 /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreGenerativeAITechniques/VideoGeneration-AzureSora-01/Program.cs)

### 新增 eShopLite 场景：并发代理编排（2025年6月）

- **新场景：并发代理编排**
- [eShopLite 仓库](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) 现在提供一个场景，演示如何使用 Semantic Kernel 进行并发代理编排。
- 此场景展示了多个代理如何并行工作来分析用户查询并为未来分析提供有价值的见解。

## 2025年5月

### Azure OpenAI 图像生成模型：gpt-image-1

- **新增第3课示例：Azure OpenAI 图像生成**
  - 第3课现在包含使用新的 Azure OpenAI 图像生成模型 `gpt-image-1` 的代码示例和说明。
  - 学习如何直接从 .NET 使用最新的 Azure OpenAI 功能生成图像。
  - 查看 [官方 Azure OpenAI DALL·E 文档](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) 和 [openai-dotnet 图像生成指南](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) 了解更多详情。
  - 在这里找到示例：[第3课：核心生成式 AI 技术](../../../03-CoreGenerativeAITechniques/)。

### 使用 AI Toolkit 和 Docker 运行本地模型

- **新增：使用 AI Toolkit 和 Docker 运行本地模型**：探索使用 [Visual Studio Code 的 AI Toolkit](https://code.visualstudio.com/docs/intelligentapps/overview) 和 [Docker Model Runner](https://docs.docker.com/model-runner/) 本地运行模型的新示例。源代码位于 [./03-CoreGenerativeAITechniques/src/](./03-CoreGenerativeAITechniques/src/)，演示了如何使用 Semantic Kernel 和 Microsoft Extensions for AI 与这些模型进行交互。

## 2025年3月

### MCP 库集成

- **适用于 .NET 的模型上下文协议**：我们添加了对 [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) 的支持，它提供了一种标准化的方式来与不同提供商的 AI 模型进行通信。
- 这种集成使模型交互更加一致，同时减少了对供应商的依赖。
- 查看我们在 [核心生成式 AI 技术](../../../03-CoreGenerativeAITechniques/) 部分展示 MCP 集成的新示例。

### eShopLite 场景仓库

- **新的 eShopLite 仓库**：所有 eShopLite 场景现在都可以在一个仓库中找到：[https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- 新仓库包括：
  - 产品目录浏览
  - 购物车管理
  - 订单下达和跟踪
  - 用户身份验证和配置文件
  - 与生成式 AI 集成的推荐和聊天功能
  - 用于产品和订单管理的管理员仪表板
