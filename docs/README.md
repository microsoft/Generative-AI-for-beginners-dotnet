# 📚 Docs — Event & Session Reference Material

This folder holds **backstage reference material** for talks, podcasts, and live sessions
delivered from this repository.

Nothing here is part of the course itself. The lessons live in the numbered folders
(`01-IntroductionToGenerativeAI/` … `05-ResponsibleAI/`) and the runnable code lives in
[`samples/`](../samples/). This folder is where the *delivery* material lives: run-of-show
scripts, demo ordering, talking points, fallback plans, and links.

## Why keep it in the repo?

Session material and sample code drift apart fast. A demo script that says "run
`MAF02` and point out the handoff" is only correct as long as `MAF02` still behaves
that way. Keeping both in the same repository means a change to a sample and the
notes that describe it travel together in the same commit and the same PR.

## Sessions

| Folder | Session | Event | Date |
|--------|---------|-------|------|
| [`events/`](./events/) | Microsoft Foundry + Microsoft Agent Framework | Podcast (video) | 2026-08-10 |
| [`OD805/`](./OD805/) | AI Building Blocks for .NET: Add Intelligence to your C# Apps | Microsoft Build 2026 | 2026-05 |

## Conventions

- **One folder or file per session.** Long-running sessions with several artifacts
  (concept, script, slides, demo notes) get a folder with its own `README.md` index.
  Single-document sessions go in [`events/`](./events/) as `YY-MM-DD-Topic.md`.
- **Link to code with relative paths**, so links survive forks and branch renames.
  Remember the depth: a file in `docs/events/` needs `../../samples/…`.
- **Record what was verified, and when.** Every session doc should end with a
  validation section listing what was actually run and the result. A demo script
  nobody has executed is a liability on stage.
- **Use the repo documentation rules** from [`AGENTS.md`](../AGENTS.md): wrap URLs in
  `[text](url)`, avoid `/en-us/` locales, and add the tracking ID to Microsoft and
  GitHub links.

## A note on `.gitignore`

`/docs` used to be listed in the root [`.gitignore`](../.gitignore). Because Git hides
ignored files, new documents here never appeared in `git status` and were silently
dropped from commits — which is how the OD805 material ended up stranded on a branch
while the samples it documents shipped to `main`. The rule has been removed. If you
ever need to confirm whether a path is being ignored, use:

```bash
git check-ignore -v <path>
```
