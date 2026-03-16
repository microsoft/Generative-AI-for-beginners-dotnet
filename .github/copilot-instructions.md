
# Copilot Instructions for Generative-AI-for-beginners-dotnet

## Project Purpose & Structure
- This repo is a hands-on .NET course for Generative AI, focused on real-world, runnable code and live coding.
- Lessons are organized in numbered folders (`01-IntroductionToGenerativeAI/` through `05-ResponsibleAI/`), each with a `readme.md` for documentation.
- All code samples live in the centralized `samples/` directory, organized by category: `CoreSamples/`, `MAF/`, `AppsWithGenAI/`, `PracticalSamples/`.
- Key technologies: .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework, Azure OpenAI, Ollama (local models).
- Multi-language support: see `translations/` for localized docs.

## Architecture & Patterns
- Each lesson folder contains documentation; code samples are in `samples/` (e.g., `samples/AppsWithGenAI/SpaceAINet/` demonstrates full-stack AI integration).
- Samples are organized under `samples/` by category: `CoreSamples/`, `MAF/`, `AppsWithGenAI/`, `PracticalSamples/`.
- AI model calls are always abstracted behind service classes (e.g., `SpaceAINet.GameActionProcessor`), enabling easy provider swapping and testability.
- API keys and endpoints are never hardcoded—use user secrets or environment variables (see lesson READMEs for details).
- For SpaceAINet and similar apps:
  - AI providers (Ollama, Azure OpenAI) are toggled at runtime via key bindings.
  - Game state is sent to the AI model, which returns a JSON action and explanation.
  - Screenshots and FPS toggles are built-in for debugging and demonstration.

## Developer Workflows
- **Code Formatting (Required Before Each Commit):**
  - Run `dotnet format` before committing any changes to ensure proper C# code formatting and style
  - Ensure all code builds with `dotnet build` and passes tests with `dotnet test`
  - Verify formatting with `dotnet format --verify-no-changes` if linting is enabled
- **Build & Run:**
  - Use standard .NET CLI: `dotnet build`, `dotnet run` from the relevant project folder (e.g., `cd samples/AppsWithGenAI/SpaceAINet/SpaceAINet.Console`).
  - For local AI: ensure Ollama is running and the required model is pulled (e.g., `ollama pull phi4-mini`).
  - For Azure: set secrets via `dotnet user-secrets` or Codespaces secrets.
- **Switching AI Providers:**
  - Most samples support toggling between local (Ollama) and cloud (Azure OpenAI) models at runtime (see app key bindings and README tables).
- **Codespaces:**
  - Dev containers are pre-configured for .NET, Azure, and Ollama workflows. Choose the right container for your use case.
- **Testing:**
  - Run sample apps in the `samples/` directory to verify model integration and workflow.

## Code Standards & Best Practices
- Follow C# and .NET best practices (naming conventions, async/await patterns, nullable reference types)
- Use modern .NET features (top-level statements, file-scoped namespaces, pattern matching) where appropriate
- Write unit tests for new functionality when possible (use xUnit or MSTest)
- Document public APIs, sample usage, and complex logic
- Prefer dependency injection and configuration via `appsettings.json` for new .NET projects
- Keep code and documentation accessible for beginners—add comments and links to docs where helpful
- For AI integrations (Azure OpenAI, Ollama, etc.), follow official SDK and security guidelines

## Project-Specific Conventions
- All lessons include a short video, code sample, and step-by-step guide.
- The code samples can be found at the `samples` folder
- Use `01-IntroductionToGenerativeAI/setup-azure-openai.md` and `01-IntroductionToGenerativeAI/setup-local-ollama.md` for setup.
- AI integration is always via service abstraction (never direct model calls in UI or game logic).
- Use key bindings in sample apps to toggle AI modes, save screenshots, and display FPS (see app README for details).
- For translations, update the corresponding `translations/<lang>/README.md`.
- Maintain existing code structure and organization—place new samples in the correct `samples/` subfolder

## Integration Points & External Dependencies
- **Azure OpenAI:** Requires endpoint, model name, and API key (see `01-IntroductionToGenerativeAI/setup-azure-openai.md`).
- **Ollama:** Requires local server running and model pulled (see `getting-started-ollama.md`).


## Key Files & Directories
- `README.md` (root): Course overview, lesson map, and links to translations.
- `01-IntroductionToGenerativeAI/` through `05-ResponsibleAI/`: Lesson folders with documentation and guides.
- `samples/CoreSamples/`: Core technique samples (chat, vision, RAG, functions, etc.).
- `samples/MAF/`: Microsoft Agent Framework samples (25+ projects).
- `samples/AppsWithGenAI/SpaceAINet/`: AI-powered Space Battle game (see `README.md` inside for architecture).
- `samples/AppsWithGenAI/HFMCP.GenImage/`: Image generation Aspire app.
- `samples/PracticalSamples/`: Aspire MCP sample.
- `translations/`: Localized documentation.
- `CONTRIBUTING.MD`: Contribution and PR guidelines.

## Example: Running SpaceAINet
1. Pull Ollama model: `ollama pull phi4-mini`
2. Set Azure secrets (if using cloud):
   - `dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"`
   - `dotnet user-secrets set "AZURE_OPENAI_MODEL" "<your-model-name>"`
   - `dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"`
3. Build & run:
   - `cd samples/AppsWithGenAI/SpaceAINet/SpaceAINet.Console`
   - `dotnet build && dotnet run`
4. Use key bindings to toggle AI modes, save screenshots, and display FPS during gameplay.

## Contribution
- See `CONTRIBUTING.MD` for PR and issue guidelines.
- Report issues or suggest improvements via GitHub Issues.

---
For more, see lesson READMEs and the root `README.md`.
