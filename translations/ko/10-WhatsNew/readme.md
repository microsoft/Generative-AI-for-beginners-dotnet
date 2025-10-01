# 초보자를 위한 생성 AI .NET의 새로운 소식

이 페이지는 강좌에 추가된 새로운 기능, 도구 및 모델의 이력을 추적합니다. 업데이트를 확인하러 다시 방문해 주세요!

## 2025년 6월

### 새로운 기능: Azure OpenAI Sora 비디오 생성 데모

- **새로운 레슨 3 샘플: Azure Sora 비디오 생성**
- 레슨 3에서는 Azure OpenAI의 새로운 [Sora 비디오 생성 모델](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)을 사용하여 텍스트 프롬프트에서 비디오를 생성하는 방법을 보여주는 실습 데모를 제공합니다.
- 이 샘플은 다음 방법을 보여줍니다:
  - 창의적인 프롬프트로 비디오 생성 작업을 제출하기.
  - 작업 상태를 폴링하고 결과 비디오 파일을 자동으로 다운로드하기.
  - 생성된 비디오를 데스크톱에 저장하여 쉽게 보기.
- 공식 문서 보기: [Azure OpenAI Sora 비디오 생성](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- 샘플 찾기: [레슨 3: 핵심 생성 AI 기술 /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreGenerativeAITechniques/VideoGeneration-AzureSora-01/Program.cs)

### 새로운 eShopLite 시나리오: 동시 에이전트 오케스트레이션 (2025년 6월)

- **새로운 시나리오: 동시 에이전트 오케스트레이션**
- [eShopLite 저장소](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent)에서는 Semantic Kernel을 사용한 동시 에이전트 오케스트레이션을 보여주는 시나리오를 제공합니다.
- 이 시나리오는 여러 에이전트가 병렬로 작업하여 사용자 쿼리를 분석하고 향후 분석을 위한 귀중한 통찰력을 제공하는 방법을 보여줍니다.

## 2025년 5월

### Azure OpenAI 이미지 생성 모델: gpt-image-1

- **새로운 레슨 3 샘플: Azure OpenAI 이미지 생성**
  - 레슨 3에서는 새로운 Azure OpenAI 이미지 생성 모델인 `gpt-image-1`을 사용하는 코드 샘플과 설명을 포함합니다.
  - .NET에서 직접 최신 Azure OpenAI 기능을 사용하여 이미지를 생성하는 방법을 배워보세요.
  - [공식 Azure OpenAI DALL·E 문서](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) 및 [openai-dotnet 이미지 생성 가이드](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images)에서 자세한 내용을 확인하세요.
  - 샘플 찾기: [레슨 3: 핵심 생성 AI 기술](../../../03-CoreGenerativeAITechniques/).

### AI Toolkit과 Docker로 로컬 모델 실행

- **새로운 기능: AI Toolkit과 Docker로 로컬 모델 실행**: [Visual Studio Code용 AI Toolkit](https://code.visualstudio.com/docs/intelligentapps/overview)과 [Docker Model Runner](https://docs.docker.com/model-runner/)를 사용하여 로컬에서 모델을 실행하는 새로운 샘플을 탐색해 보세요. 소스 코드는 [./03-CoreGenerativeAITechniques/src/](./03-CoreGenerativeAITechniques/src/)에 있으며 Semantic Kernel과 Microsoft Extensions for AI를 사용하여 이러한 모델과 상호 작용하는 방법을 보여줍니다.

## 2025년 3월

### MCP 라이브러리 통합

- **.NET용 모델 컨텍스트 프로토콜**: 다양한 공급자의 AI 모델과 통신하는 표준화된 방법을 제공하는 [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)에 대한 지원을 추가했습니다.
- 이 통합을 통해 공급자 종속성을 줄이면서 모델과의 더 일관된 상호 작용이 가능합니다.
- [핵심 생성 AI 기술](../../../03-CoreGenerativeAITechniques/) 섹션에서 MCP 통합을 보여주는 새로운 샘플을 확인해 보세요.

### eShopLite 시나리오 저장소

- **새로운 eShopLite 저장소**: 모든 eShopLite 시나리오를 단일 저장소에서 이용할 수 있습니다: [https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- 새로운 저장소에는 다음이 포함됩니다:
  - 제품 카탈로그 탐색
  - 쇼핑 카트 관리
  - 주문 배치 및 추적
  - 사용자 인증 및 프로필
  - 추천 및 채팅을 위한 생성 AI 통합
  - 제품 및 주문 관리를 위한 관리자 대시보드
