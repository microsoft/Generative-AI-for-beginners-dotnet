# Azure AI Foundry to Azure Foundry Rebrand - Change Documentation

## Executive Summary

This document outlines all necessary changes to rebrand "Azure AI Foundry" to "Azure Foundry" across the Generative-AI-for-beginners-dotnet repository. The rebrand simplifies the product name while maintaining clarity and consistency across documentation, code, and user-facing content.

**Scope**: This rebrand affects:

- Documentation files (markdown)
- Code comments
- Sample project names and namespaces
- Configuration keys and environment variables
- URLs and links (where applicable)
- Translated content (8 languages)

---

## Change Categories

### 1. Documentation Files (Markdown)

#### 1.1 Main English Documentation

**Files to Update:**

1. **`README.md`** (root)
   - Line 17: Badge URL text
   - Line 39: Provider list mention
   - Line 69: Lesson 02 description
   - Line 175: Discord badge text
   - Line 179: Developer Forum badge text

2. **`AGENTS.md`**
   - Multiple references in setup commands and workflow sections

3. **`02-SetupDevEnvironment/getting-started-azure-openai.md`**
   - Line 3: Introduction text
   - Line 10: Section heading "Create the Azure AI Foundry resources"
   - Line 12: Body text (2 occurrences)
   - Line 14: Subheading "Create a Hub and Project in Azure AI Foundry"
   - Line 16: Portal link text "Azure AI Foundry Portal"
   - Line 31: Success message
   - Line 33: Subheading "Deploy a Language Model in Azure AI Foundry"
   - Line 37: Portal navigation text
   - Line 46: Success message
   - Line 61: API key instruction
   - Line 153: Summary text
   - Line 157: Additional resources link
   - Line 159: Additional resources link

3. **`03-CoreGenerativeAITechniques/04-agents.md`**
   - Line 19: Introduction to agents
   - Line 29: Prerequisites setup
   - Line 53: Code comment
   - Line 57: Connection string location
   - Line 59: Image alt text
   - Line 75: Code comment explanation

4. **`03-CoreGenerativeAITechniques/01-lm-completions-functions.md`**
   - Line 49: Note about using alternative provider

5. **`03-CoreGenerativeAITechniques/06-LocalModelRunners.md`**
   - References in code samples

6. **`03-CoreGenerativeAITechniques/readme.md`**
   - References in sample descriptions

7. **`03-CoreGenerativeAITechniques/docs/troubleshooting-chat-api.md`**
   - References in examples

8. **`03-CoreGenerativeAITechniques/docs/QUICK-FIX-ChatMessageList.md`**
   - References in examples

9. **`04-PracticalSamples/readme.md`**
   - Line 33: Provider description
   - Multiple references throughout demos

10. **`06-AgentFx/readme.md`**
    - Line 11: Provider list
    - Line 82: Endpoint configuration
    - Line 109: Sample table (2 occurrences)
    - Line 122: Multi-agent sample description

11. **`10-WhatsNew/readme.md`**
    - Line 15: Sample description
    - Line 32: SpaceAINet description

12. **`samples/README.md`**
    - Line 35: Local model runners description
    - Line 105: Prerequisites list

13. **`samples/AgentFx/AgentFx-MultiAgents/README.md`**
    - Line 9: Agent description
    - Line 15: Multi-provider orchestration
    - Line 17: Persistent agents
    - Line 26: Architecture diagram
    - Line 42: Prerequisites
    - Line 72: Configuration section heading
    - Line 75: Environment variable comment
    - Line 119: Authentication explanation
    - Line 154: Deletion prompt
    - Line 161: Agent description
    - Line 165: Setup message
    - Line 182: Deletion prompt
    - Line 190: File structure comment
    - Line 199: Section heading
    - Line 201: Description
    - Line 204: Function description
    - Line 289: Troubleshooting
    - Line 297: Resources link

14. **`samples/AppsWithGenAI/SpaceAINet/README.md`**
    - Line 5: Description
    - Line 12: Library integration
    - Line 22: Prerequisites

#### 1.2 Translated Documentation (All Languages)

**Languages Affected:**

- Chinese (zh)
- Traditional Chinese (tw)
- Portuguese (pt)
- Korean (ko)
- Japanese (ja)
- Spanish (es)
- French (fr)
- German (de)

**Files per Language:**

- `translations/{lang}/README.md` - Badge and lesson descriptions
- `translations/{lang}/02-SetupDevEnvironment/getting-started-azure-openai.md` - All references
- `translations/{lang}/03-CoreGenerativeAITechniques/04-agents.md` - Agent setup
- `translations/{lang}/03-CoreGenerativeAITechniques/01-lm-completions-functions.md` - Notes
- `translations/{lang}/04-PracticalSamples/readme.md` - Demo descriptions

**Total Translated Files**: ~40 files across 8 languages

---

### 2. Code Files

#### 2.1 C# Source Code

**Variable/Configuration Names to Update:**

1. **Sample Projects with "AIFoundry" in Names:**
   - `samples/CoreSamples/AIFoundryLocal-01-MEAI-Chat/`
   - `samples/CoreSamples/AIFoundryLocal-01-SK-Chat/`
   - `samples/AgentFx/AgentFx-AIFoundry-01/`
   - `samples/AgentFx/AgentFx-AIFoundry-02/`
   - `samples/AgentFx/AgentFx-AIFoundryAgents-01/`

2. **Configuration Keys in Code:**

   ```csharp
   // Current:
   config["aifoundryproject_tenantid"]
   config["aifoundryproject_endpoint"]
   
   // Change to:
   config["foundryproject_tenantid"]
   config["foundryproject_endpoint"]
   ```

3. **Files Containing Config Keys:**
   - `samples/CoreSamples/AgentLabs-01-Simple/Program.cs`
   - `samples/CoreSamples/AgentLabs-02-Functions/Program.cs`
   - `samples/CoreSamples/AgentLabs-03-OpenAPIs/Program.cs`

4. **Namespace and Project Names:**
   - `BasicChat_05AIFoundryModels` → `BasicChat_05FoundryModels`
   - `AIFoundryLocal_01_SK_Chat` → `FoundryLocal_01_SK_Chat`
   - `AIFoundryLocal_01_MEAI_Chat` → `FoundryLocal_01_MEAI_Chat`
   - `AgentFx_AIFoundry_01` → `AgentFx_Foundry_01`
   - `AgentFx_AIFoundry_02` → `AgentFx_Foundry_02`
   - `AgentFx_AIFoundryAgents_01` → `AgentFx_FoundryAgents_01`

5. **Class and Provider Names:**

   ```csharp
   // Current:
   class AIFoundryAgentsProvider
   AIFoundryAgentsProvider.CreateAIAgent()
   AIFoundryAgentsProvider.DeleteAIAgentInAIFoundry()
   
   // Change to:
   class FoundryAgentsProvider
   FoundryAgentsProvider.CreateAIAgent()
   FoundryAgentsProvider.DeleteAgentInFoundry()
   ```

   - File: `samples/AgentFx/AgentFx-MultiAgents/AIFoundryAgentsProvider.cs`

6. **Connection String Variables:**

   ```csharp
   // Current in Aspire samples:
   var aifoundrymodels
   builder.AddAzureOpenAI("aifoundry")
   builder.AddConnectionString("aifoundry")
   
   // Change to:
   var foundrymodels
   builder.AddAzureOpenAI("foundry")
   builder.AddConnectionString("foundry")
   ```

   - File: `samples/AgentFx/AgentFx-AIWebChatApp-Persisting/AgentFx-AIWebChatApp-Persisting.AppHost/AppHost.cs`

#### 2.2 Solution and Project Files

**Files to Update:**

1. **Solution Files:**
   - `samples/CoreSamples/CoreGenerativeAITechniques.sln`
     - Update project references from `AIFoundryLocal-*` to `FoundryLocal-*`
     - Update project references from `BasicChat-05AIFoundryModels` to `BasicChat-05FoundryModels`

2. **Agent Demos Solution:**
   - `samples/AgentFx/AgentFx-Demos.slnx`
     - Update paths from `AgentFx-AIFoundry-01` to `AgentFx-Foundry-01`
     - Update paths from `AgentFx-AIFoundry-02` to `AgentFx-Foundry-02`
     - Update paths from `AgentFx-AIFoundryAgents-01` to `AgentFx-FoundryAgents-01`

3. **Project Files (.csproj):**
   - Update `RootNamespace` properties in all affected projects

#### 2.3 Code Comments

**Files with Comments to Update:**

1. `samples/CoreSamples/AgentLabs-01-Simple/Program.cs`
   - Line 9: `"aifoundryproject_endpoint"` comment

2. `samples/CoreSamples/AgentLabs-02-Functions/Program.cs`
   - Line 11: `"aifoundryproject_endpoint"` comment

3. `samples/CoreSamples/AgentLabs-03-OpenAPIs/Program.cs`
   - Line 10: `"aifoundryproject_endpoint"` comment

4. `samples/AgentFx/AgentFx-AIWebChatApp-Persisting/AgentFx-AIWebChatApp-Persisting.Web/Program.cs`
   - Line 26: Comment about Aspire resource key

---

### 3. Directory and File Names

**Directories to Rename:**

1. `samples/CoreSamples/AIFoundryLocal-01-MEAI-Chat/`
   → `samples/CoreSamples/FoundryLocal-01-MEAI-Chat/`

2. `samples/CoreSamples/AIFoundryLocal-01-SK-Chat/`
   → `samples/CoreSamples/FoundryLocal-01-SK-Chat/`

3. `samples/CoreSamples/BasicChat-05AIFoundryModels/`
   → `samples/CoreSamples/BasicChat-05FoundryModels/`

4. `samples/AgentFx/AgentFx-AIFoundry-01/`
   → `samples/AgentFx/AgentFx-Foundry-01/`

5. `samples/AgentFx/AgentFx-AIFoundry-02/`
   → `samples/AgentFx/AgentFx-Foundry-02/`

6. `samples/AgentFx/AgentFx-AIFoundryAgents-01/`
   → `samples/AgentFx/AgentFx-FoundryAgents-01/`

**Files to Rename:**

1. `samples/AgentFx/AgentFx-MultiAgents/AIFoundryAgentsProvider.cs`
   → `samples/AgentFx/AgentFx-MultiAgents/FoundryAgentsProvider.cs`

2. Project files within renamed directories (*.csproj)

---

### 4. Image Files and Alt Text

**Image Alt Text to Update:**

1. `03-CoreGenerativeAITechniques/04-agents.md`
   - Line 59: Alt text "AI Foundry project homepage" → "Foundry project homepage"

**Note**: Image files themselves (e.g., `project-connection-string.png`) do not need renaming as they are referenced by path, but if alt text contains "AI Foundry", update it.

---

### 5. URLs and External Links

**Portal URL** (No Change Required):

- `https://ai.azure.com/` remains the same
- Portal link text can be updated from "Azure AI Foundry Portal" to "Azure Foundry Portal"

**Badge URLs:**

```markdown
<!-- Current -->
[![Azure AI Foundry GitHub Discussions](https://img.shields.io/badge/Discussions-Azure%20AI%20Foundry-blueviolet?logo=github&style=for-the-badge)](https://aka.ms/ai-discussions/dotnet)

<!-- Change to -->
[![Azure Foundry GitHub Discussions](https://img.shields.io/badge/Discussions-Azure%20Foundry-blueviolet?logo=github&style=for-the-badge)](https://aka.ms/ai-discussions/dotnet)
```

**Discord Badge:**

```markdown
<!-- Current -->
[![Azure AI Foundry Discord](https://img.shields.io/badge/Discord-Azure_AI_Foundry_Community_Discord-blue?style=for-the-badge&logo=discord&color=5865f2&logoColor=fff)](https://aka.ms/foundry/discord)

<!-- Change to -->
[![Azure Foundry Discord](https://img.shields.io/badge/Discord-Azure_Foundry_Community_Discord-blue?style=for-the-badge&logo=discord&color=5865f2&logoColor=fff)](https://aka.ms/foundry/discord)
```

**Developer Forum Badge:**

```markdown
<!-- Current -->
[![Azure AI Foundry Developer Forum](https://img.shields.io/badge/GitHub-Azure_AI_Foundry_Developer_Forum-blue?style=for-the-badge&logo=github&color=000000&logoColor=fff)](https://aka.ms/foundry/forum)

<!-- Change to -->
[![Azure Foundry Developer Forum](https://img.shields.io/badge/GitHub-Azure_Foundry_Developer_Forum-blue?style=for-the-badge&logo=github&color=000000&logoColor=fff)](https://aka.ms/foundry/forum)
```

---

## Implementation Checklist

### Phase 1: Documentation Updates (English)

- [ ] Update `README.md` badges and references
- [ ] Update `AGENTS.md` references
- [ ] Update `02-SetupDevEnvironment/getting-started-azure-openai.md` (all sections)
- [ ] Update `03-CoreGenerativeAITechniques/04-agents.md`
- [ ] Update `03-CoreGenerativeAITechniques/01-lm-completions-functions.md`
- [ ] Update `03-CoreGenerativeAITechniques/06-LocalModelRunners.md`
- [ ] Update `03-CoreGenerativeAITechniques/readme.md`
- [ ] Update `03-CoreGenerativeAITechniques/docs/*` troubleshooting files
- [ ] Update `04-PracticalSamples/readme.md`
- [ ] Update `06-AgentFx/readme.md`
- [ ] Update `10-WhatsNew/readme.md`
- [ ] Update `samples/README.md`
- [ ] Update `samples/AgentFx/AgentFx-MultiAgents/README.md`
- [ ] Update `samples/AppsWithGenAI/SpaceAINet/README.md`

### Phase 2: Code Updates

- [ ] Rename directories (6 directories)
- [ ] Rename `AIFoundryAgentsProvider.cs` file
- [ ] Update configuration keys in AgentLabs samples (3 files)
- [ ] Update namespaces in all affected .csproj files
- [ ] Update class names and method names in provider files
- [ ] Update Aspire AppHost connection strings
- [ ] Update solution files (.sln, .slnx)
- [ ] Update code comments

### Phase 3: Translated Documentation

- [ ] Update Chinese (zh) translations (5 files)
- [ ] Update Traditional Chinese (tw) translations (5 files)
- [ ] Update Portuguese (pt) translations (5 files)
- [ ] Update Korean (ko) translations (5 files)
- [ ] Update Japanese (ja) translations (5 files)
- [ ] Update Spanish (es) translations (5 files)
- [ ] Update French (fr) translations (5 files)
- [ ] Update German (de) translations (5 files)

### Phase 4: Testing and Validation

- [ ] Run `dotnet build` on all affected solutions
- [ ] Run `dotnet format` on all code changes
- [ ] Test sample applications to ensure functionality
- [ ] Verify all documentation links work correctly
- [ ] Check that no broken references exist
- [ ] Validate translated content consistency

---

## Search and Replace Patterns

### Pattern 1: Full Product Name in Prose

```
Find:    Azure AI Foundry
Replace: Azure Foundry
```

**Context**: General documentation text, descriptions, explanations

### Pattern 2: Code Configuration Keys

```
Find:    aifoundryproject_
Replace: foundryproject_
```

**Context**: Configuration keys in code

### Pattern 3: Variable Names

```
Find:    aifoundrymodels
Replace: foundrymodels
```

**Context**: Aspire configuration variables

### Pattern 4: Connection Strings

```
Find:    "aifoundry"
Replace: "foundry"
```

**Context**: Aspire connection string names

### Pattern 5: Namespace Identifiers

```
Find:    AIFoundry
Replace: Foundry
```

**Context**: C# namespaces, class names, directory names

### Pattern 6: URL Badge Text

```
Find:    Azure%20AI%20Foundry
Replace: Azure%20Foundry
```

**Context**: GitHub badge URLs

---

## Breaking Changes and Migration Notes

### For End Users

1. **Configuration Keys Changed:**
   - Users with existing configurations must update:
     - `aifoundryproject_tenantid` → `foundryproject_tenantid`
     - `aifoundryproject_endpoint` → `foundryproject_endpoint`

2. **Aspire Connection Names Changed:**
   - Existing Aspire projects using `"aifoundry"` connection must update to `"foundry"`

3. **Sample Directory Paths Changed:**
   - Any scripts or documentation referencing old paths must be updated

### For Contributors

1. **Class and File Renames:**
   - `AIFoundryAgentsProvider` → `FoundryAgentsProvider`
   - File renames will affect imports and references

2. **Namespace Changes:**
   - Projects using the renamed samples must update their using statements

---

## Post-Implementation Verification

### Automated Checks

```powershell
# Search for remaining "AI Foundry" references
grep -r "AI Foundry" --include="*.md" --include="*.cs" --include="*.csproj"

# Search for old config keys
grep -r "aifoundryproject_" --include="*.cs"

# Search for old connection string names
grep -r '"aifoundry"' --include="*.cs"
```

### Manual Verification

1. Review all sample README files for accuracy
2. Test build all solution files
3. Run sample applications with new configuration
4. Verify all translated documentation matches English version
5. Check all hyperlinks for correctness

---

## Timeline and Effort Estimate

| Phase | Files Affected | Estimated Effort |
|-------|---------------|------------------|
| Phase 1: English Docs | ~15 files | 2-3 hours |
| Phase 2: Code Updates | ~25 files + 6 directories | 3-4 hours |
| Phase 3: Translations | ~40 files | 4-6 hours |
| Phase 4: Testing | All affected files | 2-3 hours |
| **Total** | **~80 files** | **11-16 hours** |

---

## Notes and Considerations

1. **Portal URL Unchanged**: The actual portal URL (`https://ai.azure.com/`) does not change, only the display text.

2. **SDK Package Names**: The Azure SDK package names (e.g., `Azure.AI.Projects`) are NOT changed as they are maintained by Microsoft independently.

3. **Service Name in Azure**: The underlying Azure service name may not change immediately; this is a branding update.

4. **Backward Compatibility**: This is a breaking change for configuration keys. Consider providing migration guidance or supporting both old and new keys temporarily.

5. **Documentation Links**: Some Microsoft Learn URLs may still reference "ai-foundry" in paths. These should be updated as Microsoft updates their documentation.

6. **Image Assets**: Screenshot images showing "Azure AI Foundry" in the portal UI may need to be retaken once the portal UI is updated by Microsoft.

---

## Summary Statistics

- **Total Files to Update**: ~80
- **Languages Affected**: 9 (English + 8 translations)
- **Directories to Rename**: 6
- **Code Files to Update**: ~25
- **Documentation Files to Update**: ~55
- **Configuration Keys Changed**: 2
- **Class Names Changed**: 1
- **Method Names Changed**: 1

---

**Document Version**: 1.0  
**Last Updated**: 2025-11-11  
**Prepared For**: Generative-AI-for-beginners-dotnet Repository
