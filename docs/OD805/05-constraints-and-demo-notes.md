# 05 — Constraints & Demo Notes

Operational notes to make the OD805 demos reliable on stage.

## Hard constraints

- **40 minutes total**, including any Q&A. Script targets ~39 min with a ~3 min buffer.
- **6 demos max.** More than that and transitions eat the clock. Pre-open all projects.
- **Conference Wi‑Fi is unreliable.** Have a **local fallback** (Ollama) for every
  cloud-dependent demo and a recorded backup video of the cold-open app.
- **No live Azure provisioning on stage.** Resources pre-provisioned; secrets pre-set.

## Environment / prerequisites

- **.NET 10 SDK** (repo standard; verify `dotnet --version` ≥ 10.0).
- **Aspire** workload (`aspire start` for the Zava app; `dotnet workload list` to confirm).
- **Python 3.10+** with a **virtual environment created before `aspire start`** (Aspire uses
  the venv's Python to run the NeMo agent): `python -m venv .venv` → activate →
  `pip install -r requirements.txt`.
- **Ollama** running locally with models pulled (fallback for the standalone MEAI demos):
  - `ollama pull phi4-mini` (chat fallback)
  - `ollama pull all-minilm` (embeddings fallback for RAG/ingestion demos)
- Trust dev certs: `dotnet dev-certs https --trust`.
- **Zava embeddings are local** (`ElBruno.LocalEmbeddings`, ONNX MiniLM) — the model is
  downloaded/cached on first run; warm it before going live. **No cloud embedding
  deployment is needed for the cold-open/close app.**

## Secrets / config (per `AGENTS.md`, never hardcode)

**Zava Support Center** (the bookend app — see its `docs/CONFIGURATION.md`):

- **Azure OpenAI** chat (`AZURE_OPENAI_ENDPOINT`, deployment name) — keyless preferred.
- **GPT-Image-2** (Azure OpenAI) credentials for the Pitch Image Agent (hero image).
- **NVIDIA API key** for the NeMo analysis agent.
- Embeddings: **none** — local ONNX via `ElBruno.LocalEmbeddings`.
- Optional MCP: `ENABLE_MCP_RETRIEVAL` (default off).

**Standalone CoreSamples** — set via `dotnet user-secrets` in each project:

- Azure OpenAI / **Microsoft Foundry** (chat + embeddings):
  - `AzureOpenAI:Endpoint` (or `endpoint` for older CoreSamples)
  - `AzureOpenAI:Deployment` (chat = model name; swap `gpt-5.5` ↔ `grok-4` for the Block 2 demo)
  - `AzureOpenAI:AuthMode` = `integrated` (recommended, Entra ID) or `apikey`
  - `AzureOpenAI:ApiKey` — only when `AuthMode=apikey`
  - embedding deployment, e.g. `text-embedding-3-small`
- **Integrated Security** is the recommended Foundry path: `AzureCliCredential` /
  `DefaultAzureCredential` — no keys in config, just `az login`.
- `MCP-03-MicrosoftLearn`: **no token** — the Learn MCP server is public/keyless.

> Zava uses **keyless** Azure auth where possible (`AzureCliCredential` /
> `DefaultAzureCredential`) — make sure the presenting account has the **Azure AI
> Developer** role and is signed in (`az login` / Visual Studio).

## Per-demo pre-flight checklist

- [ ] **Zava Support Center** runs via `aspire start`; venv created + deps installed; NVIDIA
      + AOAI + GPT-Image-2 secrets set; **hero image pre-rendered**; local-embeddings KB
      indexed; 3 prompts rehearsed; citation chips clickable; `/knowledge` viewer opens.
- [ ] **BasicChat-05AIFoundryModels** runs (`dotnet run app.cs`); Foundry endpoint set;
      `gpt-5.5` **and** `grok-4` deployments exist; apikey set; `az login` done so
      Integrated Security (`AuthMode=integrated`) works; model swap verified end-to-end.
- [ ] **BasicChat-03Ollama** runs against local Ollama; model pulled.
- [ ] **MCP-03-MicrosoftLearn** lists tools and completes one tool-calling request (keyless).
- [ ] **RAGSimple-02MEAIVectorsMemory** returns expected movies for the query.
- [ ] **DataIngestion-01-Simple** ingests + queries successfully.
- [ ] **MAF01 / MAF02** restore + run.
- [ ] Solutions build in Release: `CoreGenerativeAITechniques.sln`, `MAF-Demos.slnx`.

## Risks & mitigations

| Risk | Mitigation |
|---|---|
| Cloud model latency/timeout on stage | Pre-warm calls before session; Ollama fallback ready |
| NeMo / NVIDIA API slow or down | NeMo pre-warm on startup; recorded fallback video |
| GPT-Image-2 latency (~3–4 min) | **Hero image pre-rendered**; never generate on the critical path |
| MCP endpoint down / token expired | Keyless Learn MCP; pre-test morning-of; backup screenshot/recording |
| Aspire dashboard cert prompt | Trust certs ahead; launch app before going live |
| Python venv not created before `aspire start` | Create venv + install deps in pre-flight; verify NeMo agent starts |
| DataIngestion preview API churn | Pin package version; build & run morning-of |
| Demo overrun | Blocks 3B, 4B, 5B are explicitly optional — drop first |
| Embedding dimension mismatch | Zava uses local MiniLM (384-dim); standalone samples use `text-embedding-3-small` (1536) — keep them separate |

## Stage flow tips

- **Pre-open** every project in separate VS Code / VS windows; pre-build to avoid first-run
  restore delays.
- Keep a **single terminal per demo** with the `cd` already done.
- Increase editor font size; use a high-contrast theme.
- Have the **roadmap slide** and **reassemble slide** as anchors between demos.

## Definition of "ready to present"

- [ ] Concept approved by Bruno (hook line chosen).
- [ ] All reused samples verified on the presenting machine.
- [ ] **Zava Support Center** runs end-to-end (image → NeMo → grounded MAF) on the presenting machine.
- [ ] Backup recordings captured for the Zava cold open + one cloud demo.
- [ ] Full dry run completed within 40 min.
