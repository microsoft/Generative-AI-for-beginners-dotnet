# Executando Modelos de IA Localmente com AI Toolkit e Docker

Nesta lição, você aprenderá como executar modelos de IA generativa localmente em seu dispositivo ou em seu ambiente de nuvem usando AI Toolkit e Docker.

---

## Introdução

A capacidade de executar modelos de IA localmente oferece várias vantagens como privacidade, redução de custos e controle total sobre a execução do modelo. Nesta lição, você aprenderá como executar vários modelos com o Microsoft AI Toolkit e Docker.

## Microsoft AI Toolkit

Microsoft AI Toolkit é uma coleção de ferramentas e bibliotecas que permitem integrar modelos de IA locais em aplicações .NET. Ele suporta uma variedade de tipos de modelos e tarefas.

### Usando AI Toolkit com .NET

Aqui está um exemplo de como você pode usar o AI Toolkit em uma aplicação .NET:

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
chat.AddUserMessage("Explique computação quântica em termos simples");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## Docker para Modelos de IA

Docker é uma plataforma que permite executar aplicações em contêineres isolados. É uma ferramenta poderosa para executar modelos de IA em um ambiente consistente.

### Executando Modelos com Docker

Você pode executar vários modelos de IA através do Docker:

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### Aplicações de Exemplo

Nos exemplos [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat) e [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat), implementamos aplicações que utilizam modelos locais tanto com o Semantic Kernel quanto com o Microsoft.Extensions.AI.

## Resumo

Executar modelos localmente fornece mais controle sobre suas aplicações de IA e pode reduzir custos. Com o Microsoft AI Toolkit e Docker, você pode implantar uma ampla gama de modelos em suas aplicações .NET.

**Aviso Legal**:  
Este documento foi traduzido utilizando serviços de tradução baseados em IA. Embora nos esforcemos para garantir a precisão, esteja ciente de que traduções automatizadas podem conter erros ou imprecisões. O documento original em seu idioma nativo deve ser considerado a fonte oficial. Para informações críticas, recomenda-se a tradução profissional humana. Não nos responsabilizamos por quaisquer mal-entendidos ou interpretações equivocadas decorrentes do uso desta tradução.
