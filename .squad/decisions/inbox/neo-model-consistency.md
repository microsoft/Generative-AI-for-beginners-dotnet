# Decision: Repo-Wide Model Consistency — gpt-5-mini

**Author:** Neo  
**Date:** 2026-03-04  
**Status:** Implemented

## Decision

All Azure OpenAI model references across lesson documentation (01-04), code samples (CoreSamples, AppsWithGenAI), and MAF READMEs are now standardized to `gpt-5-mini`. This completes the consistency sweep started in Phases 2-3.

## Scope

- 3 `.cs` files fixed (HFMCP.GenImage default, LLMTornado label, MCP comment)
- 14 `.md` files fixed across lessons 01-04 and sample READMEs
- Total ~35 individual model name replacements

## Excluded (intentional)

- `translations/` — will be updated separately by translation team
- `.squad/` — agent infrastructure, not course content
- Ollama models, dall-e-3, sora, Claude, speech/audio — different providers/purposes
- No `appsettings.json` files had old model references

## Team Impact

- Translation team should update their localized docs to match (many `gpt-4o-mini` references remain in all 8 language translations)
- Any future docs or samples should use `gpt-5-mini` as the default Azure OpenAI model
