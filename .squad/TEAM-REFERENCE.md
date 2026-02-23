# Squad Team Reference

## Team Roster

| Badge | Name | Role | Responsibilities | Sample Prompts |
|-------|------|------|-----------------|----------------|
| 🏗️ | **Morpheus** | Lead | Scope, decisions, code review, architecture | *"Morpheus, review the project structure"* |
| 🔧 | **Neo** | .NET Dev | Core samples, implementations, C#/.NET code | *"Neo, build a new Semantic Kernel sample"* |
| 📝 | **Trinity** | Content Dev | Lessons, documentation, translations, READMEs | *"Trinity, improve the lesson 03 readme"* |
| 🧪 | **Tank** | Tester | Tests, quality, edge cases, build verification | *"Tank, verify all samples build correctly"* |
| 📊 | **Oracle** | Changelog Analyst | Repo changes, What's New updates in README & 10-WhatsNew | *"Oracle, analyze recent changes and update What's New"* |
| ⚙️ | **Dozer** | Upgrade Engineer | NuGet updates, .NET upgrades, build verification, PRs | *"Dozer, check for package upgrades"* |
| 📝 | **Niobe** | Education Strategist | Content analysis, AI/GenAI trends, Microsoft tech scouting | *"Niobe, analyze the course and suggest improvements"* |
| 📋 | **Scribe** | Session Logger | Memory, decisions, session logs (silent — auto-spawned) | *(automatic — not directly addressed)* |
| 🔄 | **Ralph** | Work Monitor | Work queue, backlog, GitHub issue tracking | *"Ralph, go"* / *"Ralph, status"* / *"Ralph, idle"* |

## Agent Details

### 🏗️ Morpheus — Lead

**Responsibilities:** Architecture decisions, scope management, code review, team coordination.

**When to use:**
- Reviewing project structure or architecture
- Making scope decisions
- Code review gates
- Resolving cross-agent conflicts

**Sample prompts:**
- *"Morpheus, review the project structure"*
- *"Morpheus, should we add a new lesson on function calling?"*
- *"Morpheus, review Neo's implementation"*

---

### 🔧 Neo — .NET Dev

**Responsibilities:** Core .NET development — building samples, implementations, and code across the course.

**When to use:**
- Building new code samples
- Implementing .NET features
- Fixing code bugs
- Creating new projects

**Sample prompts:**
- *"Neo, build a new Semantic Kernel sample"*
- *"Neo, add error handling to the chat sample"*
- *"Neo, create a new MEAI integration example"*

---

### 📝 Trinity — Content Dev

**Responsibilities:** Lesson content, documentation, README files, and translations.

**When to use:**
- Writing or improving lesson documentation
- Updating README files
- Managing translations
- Making content beginner-friendly

**Sample prompts:**
- *"Trinity, improve the lesson 03 readme"*
- *"Trinity, add setup instructions for the new sample"*
- *"Trinity, review all lessons for consistency"*

---

### 🧪 Tank — Tester

**Responsibilities:** Testing, quality assurance, build verification, and edge case analysis.

**When to use:**
- Verifying builds across all projects
- Writing test cases
- Checking code quality
- Validating after upgrades

**Sample prompts:**
- *"Tank, verify all samples build correctly"*
- *"Tank, write tests for the new chat sample"*
- *"Tank, check for breaking changes after Dozer's upgrades"*

---

### 📊 Oracle — Changelog Analyst

**Responsibilities:** Analyze recent repository changes and maintain the What's New sections.

**When to use:**
- After new samples or lessons are added
- Periodic changelog updates
- Keeping README current with latest additions

**Owned files:**
- Root `README.md` — `✨ What's New` section
- `10-WhatsNew/readme.md` — detailed changelog

**Sample prompts:**
- *"Oracle, analyze recent changes and update What's New"*
- *"Oracle, what's been added in the last month?"*
- *"Oracle, update the changelog with the new Claude samples"*

---

### ⚙️ Dozer — Upgrade Engineer

**Responsibilities:** NuGet package upgrades, .NET framework upgrades, build verification, and PR creation.

**When to use:**
- Checking for outdated NuGet packages
- Upgrading .NET framework versions
- Dependency maintenance
- Always creates a new branch and PR for human review

**Workflow:**
1. Creates branch `squad/dozer-upgrades-{date}`
2. Scans all `.csproj` files for outdated packages
3. Updates packages and verifies builds
4. Creates PR with full upgrade summary
5. Notifies Oracle for major version bumps

**Sample prompts:**
- *"Dozer, check for package upgrades"*
- *"Dozer, upgrade all projects to latest stable packages"*
- *"Dozer, check if we can move to .NET 10"*

---

### 📝 Niobe — Education Strategist

**Responsibilities:** Content analysis, AI/GenAI trend research, Microsoft technology scouting, and improvement suggestions.

**When to use:**
- Identifying gaps in course content
- Researching latest AI developments
- Suggesting new topics or lessons
- Evaluating educational quality

**Capabilities:**
- **Content Analysis** — reviews lessons for completeness, clarity, progression
- **AI/GenAI Research** — tracks new models, techniques, tools
- **Microsoft Tech Scouting** — monitors .NET, Azure AI, Semantic Kernel, MEAI updates
- **Proposals** — structured suggestions with priority and effort ratings

**Sample prompts:**
- *"Niobe, analyze the course and suggest improvements"*
- *"Niobe, what's new in AI that we should cover?"*
- *"Niobe, evaluate lesson 04 for beginner accessibility"*
- *"Niobe, what Microsoft AI announcements should we add?"*

---

### 📋 Scribe — Session Logger (Silent)

**Responsibilities:** Maintains decisions.md, orchestration logs, session logs, and cross-agent context sharing.

**Note:** Scribe is automatically spawned after agent work. You don't address Scribe directly.

---

### 🔄 Ralph — Work Monitor

**Responsibilities:** Tracks and drives the work queue. Monitors GitHub issues, PRs, and CI status.

**Commands:**
- *"Ralph, go"* — Start the work-check loop (processes all pending work)
- *"Ralph, status"* — One-time board check, no loop
- *"Ralph, idle"* — Stop monitoring
- *"Ralph, check every 10 minutes"* — Set polling interval

---

## Multi-Agent Prompts

For tasks that span multiple domains, address the team:

- *"Team, review lesson 03 for quality and completeness"* — spawns Morpheus + Trinity + Niobe
- *"Team, build a new lesson on function calling"* — spawns Morpheus (architecture) + Neo (code) + Trinity (content) + Tank (tests)
- *"Team, prepare for the .NET 10 release"* — spawns Dozer (upgrades) + Niobe (content opportunities) + Oracle (changelog)

## Agent Coordination Flows

```
Dozer finds major upgrades → notifies Oracle (changelog)
Niobe suggests new content → hands to Trinity (docs) + Neo (code)
Neo builds new sample → Tank verifies → Oracle updates What's New
Morpheus reviews → approves or rejects (lockout enforced)
```
