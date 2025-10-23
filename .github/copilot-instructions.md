
# Copilot Instructions for Generative-AI-for-beginners-dotnet

## Project Purpose & Structure
- This repo is a hands-on .NET course for Generative AI, focused on real-world, runnable code and live coding.
- Lessons are organized in numbered folders (e.g., `01-IntroToGenAI/`, `03-CoreGenerativeAITechniques/`), each with a `readme.md` and code in the root sample folder.
- Key technologies: .NET 9+, Microsoft.Extensions.AI (MEAI), Semantic Kernel, Azure OpenAI, Ollama (local models), GitHub Models.
- Multi-language support: see `translations/` for localized docs.

## Architecture & Patterns
- Each lesson is self-contained; (e.g., `05-AppCreatedWithGenAI/SpaceAINet/`) demonstrate full-stack AI integration.
- Samples are created inside the sample folder for organization.
- AI model calls are always abstracted behind service classes (e.g., `SpaceAINet.GameActionProcessor`), enabling easy provider swapping and testability.
- API keys and endpoints are never hardcoded—use user secrets or environment variables (see lesson READMEs for details).
- For SpaceAINet and similar apps:
  - AI providers (Ollama, Azure OpenAI) are toggled at runtime via key bindings.
  - Game state is sent to the AI model, which returns a JSON action and explanation.
  - Screenshots and FPS toggles are built-in for debugging and demonstration.

## Developer Workflows
- **Build & Run:**
  - Use standard .NET CLI: `dotnet build`, `dotnet run` from the relevant project folder (e.g., `cd 05-AppCreatedWithGenAI/SpaceAINet/SpaceAINet.Console`).
  - For local AI: ensure Ollama is running and the required model is pulled (e.g., `ollama pull phi4-mini`).
  - For Azure: set secrets via `dotnet user-secrets` or Codespaces secrets.
- **Switching AI Providers:**
  - Most samples support toggling between local (Ollama) and cloud (Azure OpenAI) models at runtime (see app key bindings and README tables).
- **Codespaces:**
  - Dev containers are pre-configured for .NET, Azure, and Ollama workflows. Choose the right container for your use case.
- **Testing:**
  - Run sample apps in the lesson `src/` folders to verify model integration and workflow.

## Project-Specific Conventions
- All lessons include a short video, code sample, and step-by-step guide.
- The code samples can be found at the `samples` folder
- Use `02-SetupDevEnvironment/getting-started-azure-openai.md` and `getting-started-ollama.md` for setup.
- AI integration is always via service abstraction (never direct model calls in UI or game logic).
- Use key bindings in sample apps to toggle AI modes, save screenshots, and display FPS (see app README for details).
- For translations, update the corresponding `translations/<lang>/README.md`.

## Integration Points & External Dependencies
- **Azure OpenAI:** Requires endpoint, model name, and API key (see `02-SetupDevEnvironment/getting-started-azure-openai.md`).
- **Ollama:** Requires local server running and model pulled (see `getting-started-ollama.md`).
- **GitHub Models:** Supported in some lessons; see lesson-specific instructions.
- **Semantic Kernel:** Used for agent orchestration and plugin integration in advanced samples.

## Key Files & Directories
- `README.md` (root): Course overview, lesson map, and links to translations.
- `01-IntroToGenAI/`, `02-SetupDevEnvironment/`, ...: Lesson folders with guides and code.
- `05-AppCreatedWithGenAI/SpaceAINet/`: Example of a full AI-powered .NET app (see `README.md` inside for architecture and usage).
- `samples/`: Contains all sample applications.
- `translations/`: Localized documentation.
- `CONTRIBUTING.MD`: Contribution and PR guidelines.

## Example: Running SpaceAINet
1. Pull Ollama model: `ollama pull phi4-mini`
2. Set Azure secrets (if using cloud):
   - `dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"`
   - `dotnet user-secrets set "AZURE_OPENAI_MODEL" "<your-model-name>"`
   - `dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"`
3. Build & run:
   - `cd 05-AppCreatedWithGenAI/SpaceAINet/SpaceAINet.Console`
   - `dotnet build && dotnet run`
4. Use key bindings to toggle AI modes, save screenshots, and display FPS during gameplay.

## Contribution
- See `CONTRIBUTING.MD` for PR and issue guidelines.
- Report issues or suggest improvements via GitHub Issues.

---
For more, see lesson READMEs and the root `README.md`.
