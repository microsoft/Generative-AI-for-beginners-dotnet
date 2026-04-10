# Generative AI para Principiantes .NET - Un Curso

### Lecciones prácticas que te enseñan cómo construir aplicaciones de IA Generativa en .NET

[![Licencia de GitHub](https://img.shields.io/github/license/microsoft/Generative-AI-For-beginners-dotnet.svg)](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/LICENSE)
[![Contribuidores de GitHub](https://img.shields.io/github/contributors/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/graphs/contributors/)
[![Problemas en GitHub](https://img.shields.io/github/issues/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/issues/)
[![Solicitudes de extracción en GitHub](https://img.shields.io/github/issues-pr/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/pulls/)
[![PRs Bienvenidos](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

[![Observadores de GitHub](https://img.shields.io/github/watchers/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Watch)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/watchers/)
[![Bifurcaciones en GitHub](https://img.shields.io/github/forks/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Fork)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/network/)
[![Estrellas en GitHub](https://img.shields.io/github/stars/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Star)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/stargazers/)

[![Azure AI Community Discord](https://img.shields.io/discord/1113626258182504448?label=Azure%20AI%20Community%20Discord)](https://aka.ms/ai-discord/dotnet)
[![Discusiones de Microsoft Foundry en GitHub](https://img.shields.io/badge/Discussions-Microsoft%20Foundry-blueviolet?logo=github&style=for-the-badge)](https://aka.ms/ai-discussions/dotnet)

![Logo de Generative AI para Principiantes .NET](../../translated_images/main-logo.5ac974278bc20b3520e631aaa6bf8799f2d219c5aec555da85555725546f25f8.es.jpg)

¡Bienvenido a **Generative AI para Principiantes .NET**, el curso práctico para desarrolladores .NET que quieren adentrarse en el mundo de la IA Generativa!

Este no es el típico curso de "aquí tienes algo de teoría, suerte". Este repositorio está enfocado en **aplicaciones del mundo real** y **código en vivo** para que los desarrolladores .NET puedan aprovechar al máximo la IA Generativa.

¡Es **práctico**, **orientado a la acción** y diseñado para ser **divertido**!

No olvides [darle una estrella (🌟) a este repositorio](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars) para encontrarlo fácilmente más adelante.

➡️ Obtén tu propia copia [bifurcando este repositorio](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork) y encuéntralo en tus propios repositorios.

## ✨ ¡Novedades

Estamos mejorando constantemente este curso con las últimas herramientas de IA, modelos y ejemplos prácticos:

- **🚀 Microsoft Agent Framework v1.0 GA (abril de 2026)**

  Todos los 28 ejemplos de MAF se han actualizado de preview a paquetes **estables v1.0**. Esto incluye un **cambio de ruptura:** `Microsoft.Agents.AI.AzureAI` ha sido renombrado a `Microsoft.Agents.AI.Foundry`.

  **2 nuevos escenarios de Hosted Agent** — Implementa agentes containerizados en Azure Foundry Agent Service:
  - [MAF-HostedAgent-01-TimeZone](samples/MAF/MAF-HostedAgent-01-TimeZone/) — Agente alojado simple con herramienta de zona horaria
  - [MAF-HostedAgent-02-MultiAgent](samples/MAF/MAF-HostedAgent-02-MultiAgent/) — Flujo de trabajo de Asistente de Investigación Multi-Agente

  Los flujos de trabajo multi-agente, streaming, persistencia y MCP están ahora listos para producción.
  
  👉 [Official GA Release](https://github.com/microsoft/agent-framework/releases/tag/dotnet-1.0.0) | [Foundry Agent Service GA](https://devblogs.microsoft.com/foundry/foundry-agent-service-ga/)

- **¡Nuevo: Demos de Foundry Local!**
  - La Lección 3 ahora presenta demostraciones prácticas para [modelos Foundry Local](https://github.com/microsoft/Foundry-Local/tree/main).
  - Ve la documentación oficial: [Documentación de Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
  - **Explicación completa y ejemplos de código están disponibles en [03-CoreGenerativeAITechniques/06-LocalModelRunners.md](../../03-CoreGenerativeAITechniques/06-LocalModelRunners.md)**

- **¡Nuevo: Demo de Generación de Video Azure OpenAI Sora!**
  - La Lección 3 ahora presenta una demostración práctica que muestra cómo generar videos a partir de prompts de texto usando el nuevo [modelo de generación de video Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) en Azure OpenAI.
  - El ejemplo demuestra cómo:
    - Enviar un trabajo de generación de video con un prompt creativo.
    - Consultar el estado del trabajo y descargar automáticamente el archivo de video resultante.
    - Guardar el video generado en tu escritorio para una visualización fácil.
  - Ve la documentación oficial: [Generación de video Azure OpenAI Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
  - Encuentra el ejemplo en [Lección 3: Técnicas de IA Generativa Fundamentales /src/VideoGeneration-AzureSora-01/Program.cs](../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

- **Nuevo: Modelo de Generación de Imágenes Azure OpenAI (`gpt-image-1`)**: La Lección 3 ahora presenta ejemplos de código para usar el nuevo modelo de generación de imágenes de Azure OpenAI, `gpt-image-1`. Aprende cómo generar imágenes desde .NET usando las últimas capacidades de Azure OpenAI.
  - Ve la documentación oficial: [Cómo usar modelos de generación de imágenes de Azure OpenAI](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) y la [guía de generación de imágenes openai-dotnet](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) para más detalles.
  - Encuentra el ejemplo en [Lección 3: Técnicas de IA Generativa Fundamentales .. /src/ImageGeneration-01.csproj](../../samples/CoreSamples/ImageGeneration-01/ImageGeneration-01.csproj).

- **Nuevo Escenario: Orquestación de Agentes Concurrentes en eShopLite**: El [repositorio eShopLite](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) ahora presenta un escenario que demuestra la orquestación de agentes concurrentes usando Microsoft Agent Framework. Este escenario muestra cómo múltiples agentes pueden trabajar en paralelo para analizar consultas de usuarios y proporcionar insights valiosos para análisis futuros.

[Ve todas las actualizaciones anteriores en nuestra sección de Novedades](./10-WhatsNew/readme.md)

## 🚀 Introducción

La IA Generativa está transformando el desarrollo de software, y .NET no es la excepción. Este curso tiene como objetivo simplificar el proceso ofreciendo:

- Videos cortos de 5 a 10 minutos para cada lección.
- Ejemplos de código funcionales en .NET que puedes ejecutar y explorar.
- Integración con herramientas como **GitHub Codespaces** y **Azure OpenAI** para una configuración rápida y sencilla. Pero si prefieres ejecutar los ejemplos localmente con tus propios modelos, también puedes hacerlo.

Aprenderás cómo implementar IA Generativa en proyectos .NET, desde generación básica de texto hasta la creación de soluciones completas utilizando **Azure OpenAI**, **Azure OpenAI Services** y modelos locales con Ollama.

## 📦 Cada Lección Incluye

- **Video Corto**: Una visión general rápida de la lección (5-10 minutos).
- **Ejemplos de Código Completos**: Totalmente funcionales y listos para ejecutar.
- **Guía Paso a Paso**: Instrucciones simples para ayudarte a aprender e implementar los conceptos.
- **Referencias Detalladas**: Este curso se centra en la implementación práctica de la IA Generativa, pero también proporcionamos enlaces a explicaciones teóricas en [Generative AI for Beginners - A Course](https://github.com/microsoft/generative-ai-for-beginners) cuando sea necesario.

## 🗃️ Lecciones

| #   | **Enlace a la Lección** | **Descripción** |
| --- | --- | --- |
| 01  | [**Introducción a los Fundamentos de IA Generativa para Desarrolladores .NET**](./01-IntroToGenAI/readme.md) | <ul><li>Visión general de los modelos generativos y sus aplicaciones en .NET</li></ul> |
| 02  | [**Configurando el Entorno de Desarrollo para IA Generativa en .NET**](./02-SetupDevEnvironment/readme.md) | <ul><li>Uso de bibliotecas como **Microsoft.Extensions.AI** y **Microsoft Agent Framework**.</li><li>Configuración de proveedores como Azure OpenAI, Microsoft Foundry y desarrollo local con Ollama.</li></ul> |
| 03  | [**Técnicas Básicas de IA Generativa con .NET**](./03-CoreGenerativeAITechniques/readme.md) | <ul><li>Generación de texto y flujos conversacionales.</li><li>Capacidades multimodales (visión y audio).</li><li>Agentes</li></ul> |
| 04  | [**Ejemplos Prácticos de IA Generativa en .NET**](./04-PracticalSamples/readme.md) | <ul><li>Ejemplos completos que demuestran IA Generativa en escenarios de la vida real.</li><li>Aplicaciones de búsqueda semántica.</li><li>Aplicaciones con múltiples agentes.</li></ul> |
| 05  | [**Uso Responsable de IA Generativa en Aplicaciones .NET**](./05-ResponsibleGenAI/readme.md) | <ul><li>Consideraciones éticas, mitigación de sesgos e implementaciones seguras.</li></ul> |

## 🌐 Soporte Multilenguaje

| Idioma               | Código | Enlace al README Traducido                            | Última Actualización |
|----------------------|--------|------------------------------------------------------|-----------------------|
| Chino (Simplificado) | zh     | [Traducción al Chino](../zh/README.md)   | 2025-06-11           |
| Chino (Tradicional)  | tw     | [Traducción al Chino](../tw/README.md)   | 2025-06-11           |
| Francés              | fr     | [Traducción al Francés](../fr/README.md) | 2025-06-11           |
| Japonés              | ja     | [Traducción al Japonés](../ja/README.md) | 2025-06-11           |
| Coreano              | ko     | [Traducción al Coreano](../ko/README.md) | 2025-06-11           |
| Portugués            | pt     | [Traducción al Portugués](../pt/README.md) | 2025-06-11         |
| Español              | es     | [Traducción al Español](./README.md) | 2025-06-11           |
| Alemán               | de     | [Traducción al Alemán](../de/README.md)  | 2025-06-11           |

## 🛠️ Lo Que Necesitas

Para comenzar, necesitarás:

1. Una **cuenta de GitHub** (la gratuita funciona perfectamente) para [bifurcar este repositorio completo](https://github.com/microsoft/generative-ai-for-beginners-dotnet/fork) en tu propia cuenta de GitHub.

1. **GitHub Codespaces habilitado** para entornos de codificación instantáneos. Puedes habilitar GitHub Codespaces en la configuración de tu repositorio. Aprende más sobre GitHub Codespaces [aquí](https://docs.github.com/en/codespaces).

1. Crea tu copia [bifurcando este repositorio](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork), o utiliza el `Fork` en la parte superior de esta página.

1. Un conocimiento básico de **desarrollo .NET**. Aprende más sobre .NET [aquí](https://dotnet.microsoft.com/learn/dotnet/what-is-dotnet).

Y eso es todo.

Hemos diseñado este curso para que sea lo más sencillo posible. Utilizamos lo siguiente para ayudarte a empezar rápidamente:

- **Ejecutar en GitHub Codespaces**: Con un solo clic, obtendrás un entorno preconfigurado para probar y explorar las lecciones.
- **Aprovecha los Azure OpenAI**: Prueba demostraciones impulsadas por IA alojadas directamente en este repositorio. Explicamos más en las lecciones a medida que avanzamos. *(Si quieres saber más sobre los Azure OpenAI, haz clic [aquí](https://docs.github.com/github-models))*

Cuando estés listo para ampliar, también tenemos guías para:

- Actualizar a **Azure OpenAI Services** para soluciones escalables y preparadas para empresas.
- Usar **Ollama** para ejecutar modelos localmente en tu hardware, mejorando la privacidad y el control.

## 🤝 ¿Quieres Ayudar?

¡Las contribuciones son bienvenidas! Aquí tienes cómo puedes colaborar:

- [Reporta problemas](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new) o errores en el repositorio.

- Mejora los ejemplos de código existentes o añade nuevos. ¡Haz un fork de este repositorio y propone algunos cambios!
- Sugiere lecciones adicionales o mejoras.
- ¿Tienes sugerencias o encontraste errores ortográficos o en el código? [Crea un pull request](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/compare)

Consulta el archivo [CONTRIBUTING.md](CONTRIBUTING.md) para más detalles sobre cómo participar.

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT. Consulta el archivo [LICENSE](../../LICENSE) para más detalles.

## 🌐 Otros Cursos

Tenemos mucho más contenido para ayudarte en tu proceso de aprendizaje. Échale un vistazo a:

- [Generative AI for Beginners](https://aka.ms/genai-beginners)
- [Generative AI for Beginners .NET](https://aka.ms/genainet)
- [Generative AI with JavaScript](https://aka.ms/genai-js-course)
- [AI for Beginners](https://aka.ms/ai-beginners)
- [AI Agents for Beginners - A Course](https://aka.ms/ai-agents-beginners)
- [Data Science for Beginners](https://aka.ms/datascience-beginners)
- [ML for Beginners](https://aka.ms/ml-beginners)
- [Cybersecurity for Beginners](https://github.com/microsoft/Security-101)
- [Web Dev for Beginners](https://aka.ms/webdev-beginners)
- [IoT for Beginners](https://aka.ms/iot-beginners)
- [XR Development for Beginners](https://github.com/microsoft/xr-development-for-beginners)
- [Mastering GitHub Copilot for Paired Programming](https://github.com/microsoft/Mastering-GitHub-Copilot-for-Paired-Programming)
- [Mastering GitHub Copilot for C#/.NET Developers](https://github.com/microsoft/mastering-github-copilot-for-dotnet-csharp-developers)
- [Choose Your Own Copilot Adventure](https://github.com/microsoft/CopilotAdventures)
- [Phi Cookbook: Ejemplos Prácticos con los Modelos Phi de Microsoft](https://aka.ms/phicookbook)

[¡Empecemos a aprender sobre Generative AI y .NET!](02-SetupDevEnvironment/readme.md) 🚀

**Descargo de responsabilidad**:  
Este documento ha sido traducido utilizando servicios de traducción automática basados en inteligencia artificial. Si bien nos esforzamos por lograr precisión, tenga en cuenta que las traducciones automatizadas pueden contener errores o inexactitudes. El documento original en su idioma nativo debe considerarse como la fuente autorizada. Para información crítica, se recomienda una traducción profesional realizada por humanos. No nos hacemos responsables por malentendidos o interpretaciones erróneas que surjan del uso de esta traducción.
