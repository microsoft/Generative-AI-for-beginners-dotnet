# AI Toolkit 및 Docker를 활용한 로컬 모델 실행

이 수업에서는 AI Toolkit과 Docker를 사용하여 생성형 AI 모델을 로컬 기기 또는 클라우드 환경에서 실행하는 방법에 대해 알아보겠습니다.

---

## 소개

AI 모델을 로컬에서 실행하는 기능은 개인정보 보호, 비용 절감, 모델 실행에 대한 완전한 제어와 같은 여러 이점을 제공합니다。이 수업에서는 Microsoft AI Toolkit과 Docker를 사용하여 다양한 모델을 실행하는 방법을 배웁니다。

## Microsoft AI Toolkit

Microsoft AI Toolkit은 로컬 AI 모델을 .NET 애플리케이션에 통합할 수 있게 해주는 도구와 라이브러리 모음입니다. 다양한 모델 유형과 작업을 지원합니다.

### .NET에서 AI Toolkit 사용하기

다음은 .NET 애플리케이션에서 AI Toolkit을 사용하는 방법의 예입니다:

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
chat.AddUserMessage("양자 컴퓨팅에 대해 쉽게 설명해주세요");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## AI 모델을 위한 Docker

Docker는 격리된 컨테이너에서 애플리케이션을 실행할 수 있는 플랫폼입니다. 일관된 환경에서 AI 모델을 실행하기 위한 강력한 도구입니다.

### Docker로 모델 실행하기

Docker를 통해 다양한 AI 모델을 실행할 수 있습니다:

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### 샘플 애플리케이션

[DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat)과 [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat) 예제에서는 Semantic Kernel과 Microsoft.Extensions.AI를 모두 사용하여 로컬 모델을 활용하는 애플리케이션을 구현했습니다.

## 요약

모델을 로컬에서 실행하면 AI 애플리케이션에 대한 더 많은 제어권을 갖게 되고 비용을 줄일 수 있습니다. Microsoft AI Toolkit과 Docker를 사용하면 .NET 애플리케이션에서 광범위한 모델을 배포할 수 있습니다.

**면책 조항**:  
이 문서는 기계 기반 AI 번역 서비스를 사용하여 번역되었습니다. 정확성을 위해 최선을 다하고 있지만, 자동 번역에는 오류나 부정확성이 포함될 수 있습니다. 원본 문서를 해당 언어로 작성된 상태에서 신뢰할 수 있는 권위 있는 자료로 간주해야 합니다. 중요한 정보의 경우, 전문적인 인간 번역을 권장합니다. 이 번역 사용으로 인해 발생하는 오해나 오역에 대해 당사는 책임을 지지 않습니다.
