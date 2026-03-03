# Copilot Agent Prompt for Lesson 03 Documentation

You are working in the `03-CoreGenerativeAITechniques` lesson of the Generative-AI-for-beginners-dotnet course. Your tasks are:

---

## 1. Analyze the Solution

- The .NET solution is at `03-CoreGenerativeAITechniques/src/CoreGenerativeAITechniques.sln`.
- For each project in the solution, analyze:
  - Project dependencies (NuGet packages, references, SDKs).
  - Main features and demo capabilities.
  - Any unique AI techniques, models, or integration patterns.
  - Key files or entry points.
  - Add a set of relevant tags (e.g., #Ollama, #AzureOpenAI, #RAG, #Vision, #Audio, #Chat, #Agents, etc.) for each project.

---

## 2. Documentation Generation

- Create a new folder named `docs` at the root of `03-CoreGenerativeAITechniques`.
- For each project, generate a documentation section that includes:
  - Project name and short description.
  - List of dependencies and what they are used for.
  - Main features and how to run/demo the project.
  - Screenshots (if possible, generate or reference them from running the sample).
  - Tags for quick reference.
- At the top of the documentation page, create an index with links to each project’s section.

---

## 3. README Update

- Update `03-CoreGenerativeAITechniques/readme.md` by adding a "Want to know more?" section that points to the new documentation page in `docs`.

---

## Formatting and Conventions

- Use clear markdown formatting.
- Use relative links for navigation.
- Ensure the documentation is concise but detailed enough for a new learner to understand each project’s purpose and how to try it.
- Do not hardcode any API keys or secrets; reference the lesson’s setup guides for configuration.
- Follow the repo’s conventions for abstraction and AI provider toggling.

---

## Output

- A single markdown documentation file in `03-CoreGenerativeAITechniques/docs/` with all project details and screenshots.
- An updated `03-CoreGenerativeAITechniques/readme.md` with a section linking to the new