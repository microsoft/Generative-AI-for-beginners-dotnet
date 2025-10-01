# Novidades em IA Generativa para Iniciantes .NET

Esta página acompanha o histórico de novas funcionalidades, ferramentas e modelos adicionados ao curso. Volte aqui regularmente para atualizações!

## Junho 2025

### Novo: Demo de Geração de Vídeo Azure OpenAI Sora

- **Novo Exemplo da Lição 3: Geração de Vídeo Azure Sora**
- A Lição 3 agora apresenta uma demonstração prática mostrando como gerar vídeos a partir de prompts de texto usando o novo [modelo de geração de vídeo Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) no Azure OpenAI.
- O exemplo demonstra como:
  - Enviar um trabalho de geração de vídeo com um prompt criativo.
  - Fazer polling do status do trabalho e baixar automaticamente o arquivo de vídeo resultante.
  - Salvar o vídeo gerado na sua área de trabalho para visualização fácil.
- Veja a documentação oficial: [Geração de vídeo Azure OpenAI Sora](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
- Encontre o exemplo em [Lição 3: Técnicas de IA Generativa Fundamentais /src/VideoGeneration-AzureSora-01/Program.cs](../../../samples/CoreGenerativeAITechniques/VideoGeneration-AzureSora-01/Program.cs)

### Novo Cenário eShopLite: Orquestração de Agentes Concorrentes (Junho 2025)

- **Novo Cenário: Orquestração de Agentes Concorrentes**
- O [repositório eShopLite](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) agora apresenta um cenário demonstrando orquestração de agentes concorrentes usando Semantic Kernel.
- Este cenário mostra como múltiplos agentes podem trabalhar em paralelo para analisar consultas de usuários e fornecer insights valiosos para análises futuras.

## Maio 2025

### Modelo de Geração de Imagens Azure OpenAI: gpt-image-1

- **Novo Exemplo da Lição 3: Geração de Imagens Azure OpenAI**
  - A Lição 3 agora inclui exemplos de código e explicações para usar o novo modelo de geração de imagens do Azure OpenAI: `gpt-image-1`.
  - Aprenda como gerar imagens usando as mais recentes capacidades do Azure OpenAI diretamente do .NET.
  - Veja a [documentação oficial do Azure OpenAI DALL·E](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) e o [guia de geração de imagens openai-dotnet](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) para mais detalhes.
  - Encontre o exemplo em [Lição 3: Técnicas de IA Generativa Fundamentais](../../../03-CoreGenerativeAITechniques/).

### Executar Modelos Locais com AI Toolkit e Docker

- **Novo: Executar Modelos Locais com AI Toolkit e Docker**: Explore novos exemplos para executar modelos localmente usando [AI Toolkit para Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview) e [Docker Model Runner](https://docs.docker.com/model-runner/). O código-fonte está em [./03-CoreGenerativeAITechniques/src/](./03-CoreGenerativeAITechniques/src/) e demonstra como usar Semantic Kernel e Microsoft Extensions para AI para interagir com esses modelos.

## Março 2025

### Integração da Biblioteca MCP

- **Protocolo de Contexto de Modelo para .NET**: Adicionamos suporte para o [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk), que fornece uma maneira padronizada de se comunicar com modelos de IA de diferentes provedores.
- Esta integração permite interações mais consistentes com modelos enquanto reduz o vendor lock-in.
- Confira nossos novos exemplos demonstrando integração MCP na seção [Técnicas de IA Generativa Fundamentais](../../../03-CoreGenerativeAITechniques/).

### Repositório de Cenários eShopLite

- **Novo Repositório eShopLite**: Todos os cenários do eShopLite estão agora disponíveis em um único repositório: [https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- O novo repositório inclui:
  - Navegação no catálogo de produtos
  - Gerenciamento do carrinho de compras
  - Colocação e rastreamento de pedidos
  - Autenticação e perfis de usuário
  - Integração com IA Generativa para recomendações e chat
  - Painel administrativo para gerenciamento de produtos e pedidos
