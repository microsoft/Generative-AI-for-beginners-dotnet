# Ejecución de modelos de IA localmente con AI Toolkit y Docker

En esta lección, aprenderá cómo ejecutar modelos de IA generativa localmente en su dispositivo o en su entorno de nube utilizando AI Toolkit y Docker.

---

## Introducción

La capacidad de ejecutar modelos de IA localmente ofrece múltiples ventajas como privacidad, reducción de costos y control total sobre la ejecución del modelo. En esta lección, aprenderá a ejecutar varios modelos con Microsoft AI Toolkit y Docker.

## Microsoft AI Toolkit

Microsoft AI Toolkit es un conjunto de herramientas y bibliotecas que le permiten integrar modelos de IA locales en aplicaciones .NET. Es compatible con una variedad de tipos de modelos y tareas.

### Uso de AI Toolkit con .NET

Aquí hay un ejemplo de cómo puede utilizar AI Toolkit en una aplicación .NET:

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
chat.AddUserMessage("Explícame la computación cuántica en términos simples");
var response = await chatCompletion.GetChatMessageContentAsync(chat);
Console.WriteLine(response.Content);
```

## Docker para modelos de IA

Docker es una plataforma que le permite ejecutar aplicaciones en contenedores aislados. Es una herramienta poderosa para ejecutar modelos de IA en un entorno consistente.

### Ejecución de modelos con Docker

Puede ejecutar varios modelos de IA a través de Docker:

```bash
docker run -d --gpus all -p 8080:8080 ghcr.io/microsoft/phi3:latest
```

### Aplicaciones de ejemplo

En los ejemplos [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat) y [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat), hemos implementado aplicaciones que utilizan modelos locales tanto con Semantic Kernel como con Microsoft.Extensions.AI.

## Resumen

La ejecución de modelos localmente le brinda mayor control sobre sus aplicaciones de IA y puede reducir costos. Con Microsoft AI Toolkit y Docker, puede implementar una amplia gama de modelos en sus aplicaciones .NET.

**Descargo de responsabilidad**:  
Este documento ha sido traducido utilizando servicios de traducción automática basados en inteligencia artificial. Si bien nos esforzamos por garantizar la precisión, tenga en cuenta que las traducciones automáticas pueden contener errores o imprecisiones. El documento original en su idioma nativo debe considerarse como la fuente autorizada. Para información crítica, se recomienda una traducción profesional realizada por humanos. No nos hacemos responsables de malentendidos o interpretaciones erróneas que puedan surgir del uso de esta traducción.
