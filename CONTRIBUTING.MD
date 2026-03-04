# Contributing

This project welcomes contributions and suggestions. Most contributions require you to
agree to a Contributor License Agreement (CLA) declaring that you have the right to,
and actually do, grant us the rights to use your contribution. For details, visit
<https://cla.microsoft.com>.

> Important: when translating text in this repo, please ensure that you do not use machine translation. We will verify translations via the community, so please only volunteer for translations in languages where you are proficient.

When you submit a pull request, a CLA-bot will automatically determine whether you need
to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the
instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

## Volunteer Responsibilities

All volunteers who contribute to this project have important responsibilities to maintain the quality and consistency of the repository:

### Translation and Content Updates

**Volunteers are responsible for reviewing and updating translations** for any major changes to the guide in their respective language(s). Major changes include:
- New lessons or sections added
- Significant updates to existing content (more than minor typo fixes)
- Changes to code samples or instructions
- Updates to documentation structure or navigation

When major updates are announced, volunteers will be pinged and are expected to review and update their translations within 14 days.

### Issue-First Approach

Before making any contribution, whether it's a bug fix, feature addition, documentation update, or translation change, you must:

1. **Open an issue** describing the intended update, change, or fix
2. Wait for discussion and approval from the maintainers
3. Reference the issue number in your pull request

### Critical Updates Process

When significant changes are required to the repository:

1. **Maintainers will ping relevant volunteers** via GitHub mentions in the related issue
2. **Volunteers should acknowledge the notification** and provide an estimated timeline for completing the required updates
3. **If a volunteer is unable to complete the work**, they should communicate this promptly so others can assist

By volunteering to contribute to this project, you agree to these responsibilities and commit to being responsive to critical update requests.

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/?WT.mc_id=academic-105485-koreyst).
For more information read the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/?WT.mc_id=academic-105485-koreyst) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Question or Problem?

Please do not open GitHub issues for general support questions as the GitHub list should be used for feature requests and bug reports. This way we can more easily track actual issues or bugs from the code and keep the general discussion separate from the actual code.

## Typos, Issues, Bugs and contributions

Whenever you are submitting any changes to the Generative AI for Beginners repository, please follow these recommendations.

* Always open an issue first before making any changes (see "Volunteer Responsibilities" section above)
* Always fork the repository to your own account before making your modifications
* Do not combine multiple changes to one pull request. For example, submit any bug fix and documentation updates using separate PRs
* If your pull request shows merge conflicts, make sure to update your local main to be a mirror of what's in the main repository before making your modifications
* If you are submitting a translation, please create one PR for all the translated files as we don't accept partial translations for the content
* If you are submitting a typo or documentation fix, you can combine modifications to a single PR where suitable
* All pull requests must reference the issue number that describes the change being made

## Code Standards for .NET Samples

When adding new code samples to this course, follow these conventions:

### Shared User Secrets Configuration

All samples use the **shared User Secrets ID** `genai-beginners-dotnet` for consistency. This allows learners to configure secrets once and use them across all samples.

**In your project file (.csproj):**
```xml
<PropertyGroup>
  <UserSecretsId>genai-beginners-dotnet</UserSecretsId>
</PropertyGroup>
```

**Never hardcode Azure OpenAI model names.** Reference the configured deployment:
```csharp
// Load from configuration (will use User Secrets automatically)
var config = new ConfigurationBuilder()
    .AddUserSecrets(userSecretsId: "genai-beginners-dotnet")
    .AddEnvironmentVariables()
    .Build();

string deployment = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";
string embeddingDeployment = config["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-3-small";
```

### File-Based Console Apps (.NET 10)

For simple console samples, use .NET 10 file-based programs to minimize boilerplate:

**Instead of a full project folder**, create a simple `.cs` file:
```powershell
# Users can run directly
dotnet run my-sample.cs

# Or compile and execute
dotnet compile my-sample.cs
./my-sample
```

This pattern is ideal for:
- Quick demonstrations
- Single-file utilities
- Lesson code walkthroughs

**Place file-based samples in `samples/` with clear naming:**
```
samples/
  ├── quick-completions.cs
  ├── basic-chat.cs
  └── embeddings-demo.cs
```

### Configuration & Secrets Best Practices

- **Always use User Secrets** for local development (never commit API keys)
- **Use environment variables** for CI/CD and containers
- **Support both** via ConfigurationBuilder (User Secrets take precedence)
- **Default to "gpt-5-mini"** for chat, **"text-embedding-3-small"** for embeddings
- **Reference docs:** [Azure Resource Setup Guide](./01-IntroductionToGenerativeAI/setup-azure-openai.md)

---

## General Guidance for Writing

- Ensure that all your URLs are wrapped in square brackets followed by a parenthesis with no extra spaces around them or inside them `[]()`.
- Ensure that any relative link (i.e. links to other files and folders in the repository) starts with a `./` referring to a file or a folder located in the current working directory or a `../` referring to a file or a folder located in a parent working directory.
- Ensure that any relative link (i.e. links to other files and folders in the repository) has a tracking ID (i.e. `?` or `&` then `wt.mc_id=` or `WT.mc_id=`) at the end of it.
- Ensure that any URL from the following domains _github.com, microsoft.com, visualstudio.com, aka.ms, and azure.com_ has a tracking ID (i.e. `?` or `&` then `wt.mc_id=` or `WT.mc_id=`) at the end of it.
- Ensure that your links don't have country specific locale in them (i.e. `/en-us/` or `/en/`).
- Ensure that all images are stored in the `./images` folder.
- Ensure that the images have descriptive names using English characters, numbers, and dashes in the name of your image.

Congratulations! We will get back to you as soon as possible with feedback about your contribution.
