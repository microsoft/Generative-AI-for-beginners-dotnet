# AGENTS.md

## Project Overview

This repository contains **Generative AI for Beginners .NET** - a hands-on course teaching .NET developers how to build Generative AI applications. The course focuses on practical, runnable code samples and real-world applications.

### Architecture

- **Lesson-based structure**: Organized in numbered folders (01-10) containing lessons, documentation, and code samples
- **Multi-language support**: Course materials translated into 8 languages (located in `translations/`)
- **Sample applications**: Full-featured AI-powered apps demonstrating practical GenAI integration
- **Key technologies**: .NET 9+, Microsoft.Extensions.AI (MEAI), Semantic Kernel, Azure OpenAI, Ollama, GitHub Models

### Project Structure

```
.
├── 01-IntroToGenAI/              # Introduction to Generative AI
├── 02-SetupDevEnvironment/       # Environment setup guides
├── 03-CoreGenerativeAITechniques/ # Core AI techniques and samples
├── 04-PracticalSamples/          # Practical AI application samples
├── 05-AppCreatedWithGenAI/       # Apps built using GenAI tools
│   ├── SpaceAINet/              # AI-powered Space Battle game
│   ├── HFMCP.GenImage/          # Image generation app
│   └── ConsoleGpuViewer/        # GPU viewer console app
├── 06-AgentFx/                   # Microsoft Agent Framework examples
├── 09-ResponsibleGenAI/          # Responsible AI practices
├── 10-WhatsNew/                  # Latest updates and features
└── translations/                 # Localized course materials

Each lesson folder contains:
- readme.md: Lesson documentation
- src/: Source code and samples
- images/: Visual assets
```

## Setup Commands

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later
- Access to at least one AI provider:
  - **GitHub Models** (recommended for beginners - free tier available)
  - **Azure OpenAI** (requires Azure subscription)
  - **Ollama** (for local model execution)

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/microsoft/Generative-AI-for-beginners-dotnet.git
cd Generative-AI-for-beginners-dotnet

# Verify .NET installation
dotnet --version  # Should be 9.0 or higher
```

### AI Provider Setup

#### GitHub Models (Recommended)
1. Create a Personal Access Token at https://github.com/settings/tokens
2. Set as environment variable or user secret (see lesson 02)

#### Azure OpenAI
```bash
# Navigate to a project and set user secrets
cd <project-path>
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "<your-model-name>"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"
```

#### Ollama (Local Models)
```bash
# Install Ollama from https://ollama.com/download

# Pull required models (example for lesson 02)
ollama pull phi4-mini

# Pull models for RAG samples (lesson 03)
ollama pull phi4-mini
ollama pull all-minilm

# Verify Ollama is running (default: http://localhost:11434)
curl http://localhost:11434/api/version
```

### Installing Dependencies

```bash
# Restore dependencies for a specific solution
dotnet restore <path-to-solution>.sln

# Example: Restore setup samples
dotnet restore 02-SetupDevEnvironment/src/GettingReadySamples.sln

# Restore all projects in a lesson
dotnet restore 03-CoreGenerativeAITechniques/src/CoreGenerativeAITechniques.sln
```

## Development Workflow

### GitHub Codespaces (Recommended)

1. Fork this repository to your GitHub account
2. Create a new Codespace from your fork
3. Select the appropriate dev container:
   - Default: .NET 9 with Azure CLI and GitHub CLI
   - Ollama: Includes Ollama for local model execution
4. The postCreateCommand will automatically set up .NET workloads and trust dev certificates

### Local Development

```bash
# Start a sample project
cd <project-directory>
dotnet run

# Example: Run basic chat sample with MEAI
cd 02-SetupDevEnvironment/src/BasicChat-01MEAI
dotnet run

# Example: Run with Ollama
cd 02-SetupDevEnvironment/src/BasicChat-03Ollama
dotnet run
```

### Working with Sample Applications

#### SpaceAINet (AI-powered game)
```bash
cd 05-AppCreatedWithGenAI/SpaceAINet/SpaceAINet.Console
dotnet build
dotnet run

# In-game controls:
# A - Toggle Azure OpenAI mode
# O - Toggle Ollama mode
# F - Show FPS
# S - Save screenshot
```

#### Aspire MCP Sample
```bash
cd 04-PracticalSamples/src
dotnet run --project src/McpSample.AppHost/McpSample.AppHost.csproj
```

### Hot Reload and Watch Mode

```bash
# Run with hot reload (for web projects)
dotnet watch run

# Build in watch mode
dotnet watch build
```

## Testing Instructions

### Build Validation

The repository uses GitHub Actions for CI/CD validation. Before submitting changes:

```bash
# Restore dependencies
dotnet restore <solution-path>

# Build in Release mode (as CI does)
dotnet build <solution-path> --no-restore --configuration Release --verbosity minimal

# Example: Validate all setup samples
dotnet restore 02-SetupDevEnvironment/src/GettingReadySamples.sln
dotnet build 02-SetupDevEnvironment/src/GettingReadySamples.sln --no-restore --configuration Release
```

### Validated Solutions

The following solutions are validated in CI:
- `02-SetupDevEnvironment/src/GettingReadySamples.sln`
- `03-CoreGenerativeAITechniques/src/CoreGenerativeAITechniques.sln`
- `04-PracticalSamples/src/Aspire.MCP.Sample.sln`
- `05-AppCreatedWithGenAI/HFMCP.GenImage/HFMCP.GenImage.sln`
- `05-AppCreatedWithGenAI/SpaceAINet/SpaceAINet.sln`

### Manual Testing

```bash
# Test a specific project
cd <project-directory>
dotnet run

# Verify AI integration works with your provider
# Follow the lesson-specific README instructions for testing AI features
```

### Note on Test Coverage

This repository is primarily educational with sample applications. There are currently no automated unit or integration tests. Validation is done through:
- Build verification in CI
- Manual testing of sample applications
- Documentation review

## Code Style and Conventions

### Formatting

```bash
# Format code before committing (REQUIRED)
dotnet format

# Verify formatting without making changes
dotnet format --verify-no-changes

# Format a specific solution
dotnet format <path-to-solution>.sln
```

### C# Conventions

- Use modern .NET features (top-level statements, nullable reference types, file-scoped namespaces)
- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for locals)
- Use async/await patterns for I/O operations
- Prefer dependency injection over static instances
- Use `IChatClient` abstraction (Microsoft.Extensions.AI) for AI model integration

### AI Integration Patterns

- **Never hardcode API keys** - use user secrets or environment variables
- **Abstract AI providers** behind service classes for easy swapping
- **Use IChatClient** from Microsoft.Extensions.AI for provider-agnostic code
- **Semantic Kernel** for advanced orchestration and plugin integration
- Support multiple providers (Azure OpenAI, Ollama, GitHub Models) where applicable

### File Organization

- Place lesson code in `<lesson-number>/src/` directories
- Store images in `<lesson-number>/images/` or root `images/` folder
- Keep README files in lesson root directories
- Use descriptive file names with English characters, numbers, and dashes

### Documentation Standards

- All URLs must be wrapped in `[text](url)` format
- Relative links start with `./` (current dir) or `../` (parent dir)
- Add tracking IDs to Microsoft/GitHub URLs: `?wt.mc_id=` or `&wt.mc_id=`
- No country-specific locales in URLs (avoid `/en-us/`)
- Update relevant lesson README.md files when modifying code

## Build and Deployment

### Build Commands

```bash
# Standard build
dotnet build

# Release build (as used in CI)
dotnet build --configuration Release

# Build specific solution
dotnet build <path-to-solution>.sln
```

### .NET Workloads

The dev container automatically installs required workloads. For local development:

```bash
# Update workloads
dotnet workload update

# List installed workloads
dotnet workload list

# Trust development certificates
dotnet dev-certs https --trust
```

### Deployment Notes

- Sample projects are designed for local execution and learning
- For production deployment, follow Azure deployment best practices
- Aspire projects can be deployed to Azure Container Apps
- Refer to individual project READMEs for specific deployment instructions

## Pull Request Guidelines

### Before Submitting a PR

1. **Format your code**: Run `dotnet format` on all modified projects
2. **Build verification**: Ensure `dotnet build` succeeds for affected solutions
3. **Test manually**: Run and verify sample applications work as expected
4. **Update documentation**: Modify lesson READMEs if changing functionality
5. **Translation considerations**: For content changes, create separate PRs per language

### PR Requirements

- **Fork the repository** before making changes
- **One change per PR**: Separate bug fixes, features, and documentation updates
- **No partial translations**: Submit complete translations for all files in a language
- **Keep main branch updated**: Rebase if there are merge conflicts
- **CLA required**: Contributors must agree to Microsoft's Contributor License Agreement

### Commit Message Format

```
<type>: <brief description>

Examples:
- docs: Update Azure OpenAI setup instructions
- feat: Add new RAG sample with vector search
- fix: Correct null reference in chat service
- style: Apply dotnet format to all projects
```

### PR Title Format

Use descriptive titles that clearly indicate the change:
- `docs: Update lesson 02 for .NET 9 compatibility`
- `feat: Add Agent Framework orchestration sample`
- `fix: Resolve build errors in SpaceAINet project`

## Additional Notes

### Common Issues and Solutions

#### Build Errors
- **Missing SDK**: Ensure .NET 9 SDK is installed (`dotnet --version`)
- **Workload issues**: Run `dotnet workload update`
- **Package restore fails**: Delete `bin/` and `obj/` folders, then `dotnet restore`

#### AI Provider Issues
- **GitHub Models**: Verify Personal Access Token is set correctly
- **Azure OpenAI**: Check user secrets are configured: `dotnet user-secrets list`
- **Ollama**: Ensure service is running and model is pulled: `ollama list`

#### Codespace-Specific
- **Port forwarding**: Ensure port 17057 is forwarded for web applications
- **Memory limits**: Codespace requires 16GB RAM for optimal performance
- **Ollama in Codespaces**: Use the Ollama dev container configuration

### Security Considerations

- **Never commit secrets** to source control
- **Use user secrets** for local development: `dotnet user-secrets set`
- **Environment variables** for production/Codespaces
- **Review .gitignore** before committing to exclude sensitive files

### Performance Tips

- **Local models (Ollama)**: Require significant CPU/GPU resources
- **Cloud models**: Faster but require network connectivity
- **Batch operations**: Use streaming responses for better UX
- **Caching**: Consider caching AI responses for repeated queries

### Multi-Language Support

Translations are maintained in `translations/<language-code>/`:
- Only contribute translations in languages where you are proficient
- Do not use machine translation
- Submit complete translations (all files) in a single PR
- Update the translation table in README.md with the date

### Project-Specific Context

#### Lesson 02: Setup
- Focus on environment configuration and AI provider access
- Three paths: GitHub Models, Azure OpenAI, Ollama
- Core samples: BasicChat-01MEAI, BasicChat-03Ollama

#### Lesson 03: Core Techniques
- LLM completions, chat flows, function calling
- RAG (Retrieval-Augmented Generation) samples
- Vision and audio AI applications
- Agent implementations

#### Lesson 04: Practical Samples
- Aspire MCP sample demonstrating Model Context Protocol
- Multi-project Aspire solution with Blazor chat UI
- Integration with external MCP servers

#### Lesson 05: Apps Created with GenAI
- **SpaceAINet**: AI-powered retro game (created with Copilot Agent)
- **HFMCP.GenImage**: Image generation Aspire app
- **ConsoleGpuViewer**: GPU monitoring console app

#### Lesson 06: Agent Framework
- Microsoft Agent Framework (AgentFx) examples
- Multi-agent orchestration patterns
- Integration with MCP for enhanced capabilities

### Contributing to Documentation

When updating documentation:
1. Maintain consistent formatting across all READMEs
2. Include code examples where helpful for beginners
3. Add links to official documentation for deeper learning
4. Keep explanations accessible to .NET developers new to AI

### Resource Links

- [.NET Documentation](https://learn.microsoft.com/dotnet/)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Semantic Kernel](https://learn.microsoft.com/semantic-kernel/)
- [Azure OpenAI](https://learn.microsoft.com/azure/ai-services/openai/)
- [Ollama Documentation](https://ollama.com/docs)
- [GitHub Models](https://docs.github.com/github-models)

---

**For more information**, refer to:
- [README.md](./README.md) - Course overview and lesson map
- [CONTRIBUTING.MD](./CONTRIBUTING.MD) - Detailed contribution guidelines
- Individual lesson READMEs for specific topics and samples
