# Novedades en IA Generativa para Principiantes .NET

Esta página rastrea el historial de nuevas características, herramientas y modelos añadidos al curso. ¡Vuelve regularmente para ver las actualizaciones!

## Junio 2025

### Nuevo: Demo de Generación de Video Azure OpenAI Sora

- **Nuevo Ejemplo de Lección 3: Generación de Video Azure Sora**
- La Lección 3 ahora presenta una demostración práctica que muestra cómo generar videos a partir de prompts de texto usando el nuevo [modelo de generación de video Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) en Azure OpenAI.
- El ejemplo demuestra cómo:
  - Enviar un trabajo de generación de video con un prompt creativo.
  - Consultar el estado del trabajo y descargar automáticamente el archivo de video resultante.
  - Guardar el video generado en tu escritorio para una visualización fácil.
- Ve la documentación oficial: [Generación de video Azure OpenAI Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- Encuentra el ejemplo en [Lección 3: Técnicas de IA Generativa Fundamentales /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

### Nuevo Escenario eShopLite: Orquestación de Agentes Concurrentes (Junio 2025)

- **Nuevo Escenario: Orquestación de Agentes Concurrentes**
- El [repositorio eShopLite](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) ahora presenta un escenario que demuestra la orquestación de agentes concurrentes usando Microsoft Agent Framework.
- Este escenario muestra cómo múltiples agentes pueden trabajar en paralelo para analizar consultas de usuarios y proporcionar insights valiosos para análisis futuros.

## Mayo 2025

### Modelo de Generación de Imágenes Azure OpenAI: gpt-image-1

- **Nuevo Ejemplo de Lección 3: Generación de Imágenes Azure OpenAI**
  - La Lección 3 ahora incluye ejemplos de código y explicaciones para usar el nuevo modelo de generación de imágenes de Azure OpenAI: `gpt-image-1`.
  - Aprende cómo generar imágenes usando las últimas capacidades de Azure OpenAI directamente desde .NET.
  - Ve la [documentación oficial de Azure OpenAI DALL·E](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) y la [guía de generación de imágenes openai-dotnet](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) para más detalles.
  - Encuentra el ejemplo en [Lección 3: Técnicas de IA Generativa Fundamentales](../../../03-CoreGenerativeAITechniques/).

### Ejecutar Modelos Locales con AI Toolkit y Docker

- **Nuevo: Ejecutar Modelos Locales con AI Toolkit y Docker**: Explora nuevos ejemplos para ejecutar modelos localmente usando [AI Toolkit para Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview) y [Docker Model Runner](https://docs.docker.com/model-runner/). El código fuente está en [../../../samples/CoreSamples/](../../../samples/CoreSamples/) y demuestra cómo usar Microsoft.Extensions.AI para interactuar con estos modelos.

## Marzo 2025

### Integración de Biblioteca MCP

- **Protocolo de Contexto de Modelo para .NET**: Hemos añadido soporte para el [SDK MCP C#](https://github.com/modelcontextprotocol/csharp-sdk), que proporciona una forma estandarizada de comunicarse con modelos de IA a través de diferentes proveedores.
- Esta integración permite interacciones más consistentes con modelos mientras reduce el vendor lock-in.
- Echa un vistazo a nuestros nuevos ejemplos que demuestran la integración MCP en la sección [Técnicas de IA Generativa Fundamentales](../../../03-CoreGenerativeAITechniques/).

### Repositorio de Escenarios eShopLite

- **Nuevo Repositorio eShopLite**: Todos los escenarios de eShopLite están ahora disponibles en un solo repositorio: [https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- El nuevo repositorio incluye:
  - Navegación del catálogo de productos
  - Gestión del carrito de compras
  - Colocación y seguimiento de pedidos
  - Autenticación y perfiles de usuario
  - Integración con IA Generativa para recomendaciones y chat
  - Panel de administración para gestión de productos y pedidos