# Releases

This repository is released manually via the **Squad Release**
(`.github/workflows/squad-release.yml`) GitHub Actions workflow. There is no
push-triggered release automation — a release is always a deliberate,
human-dispatched action.

## Tag convention

Release tags are the UTC calendar date the release was cut, in strict
`YYYY-MM-DD` form (for example `2026-08-24`). This matches every tag already
published for this repository (`git tag --list`). If you leave the `tag`
input empty when dispatching the workflow, it computes the current UTC date
for you; if you supply one explicitly, it must be a real calendar date in
that exact format (e.g. `2026-02-30` is rejected).

## Release notes: curated first, generated fallback

- **Curated notes (preferred):** add a Markdown file directly under
  `docs/releases/`, named to match the tag, e.g. `docs/releases/2026-08-26.md`.
  Pass that path as the `notes_path` input when dispatching the workflow.
  The path must be a single file directly inside `docs/releases/` ending in
  `.md` — no subdirectories, absolute paths, or `..` segments are accepted.
- **Generated fallback:** leave `notes_path` empty to use GitHub's
  `--generate-notes` (auto-generated title and notes from merged PRs since
  the previous release).

Curated notes are recommended whenever a release should call out
breaking changes, new lessons/samples, or anything that GitHub's
auto-generated PR list would obscure.

## How to cut a release (dry-run-first procedure)

1. **Dry run.** Go to **Actions → Squad Release → Run workflow**, select the
   `main` branch, leave `dry_run` at its default `true`, and set `tag` /
   `notes_path` / `mark_latest` as desired. This validates the exact commit
   SHA at the tip of `main` (rejecting the run if `main` has moved since
   dispatch), checks the tag/notes inputs, confirms no tag or release already
   exists for that tag, and runs the full reused `.NET Build Validation`
   workflow (all five Release solution builds, the test project, and every
   tracked file-based `app.cs` compile) against that SHA — without creating
   anything. The job summary prints the exact SHA, tag, notes source, and
   latest behavior that a live run would use.
2. **Review the dry-run summary.** Confirm the SHA, tag, and notes source are
   what you expect.
3. **Live run.** Re-dispatch the same workflow with `dry_run: false` (all
   other inputs unchanged). Only after validation succeeds again does the
   workflow create the tag and publish the GitHub release — both from the
   exact SHA that was validated, not whatever `main` happens to point to when
   the publish step runs.

> There is no "promote" step and no `dev`/`preview`/`insider` branches in this
> repository — every release is cut directly from `main`.

## Opt-in Ollama smoke validation (prerequisite, not part of release)

`.github/workflows/ollama-smoke-validation.yml` is a separate,
`workflow_dispatch`-only workflow that exercises the Ollama-backed samples
against a real local model. It is **not** invoked automatically by Squad
Release and is **not required** to cut a release, but running it manually
before a release that touches Ollama-integrated samples is recommended,
since the reused build validation only compiles those samples — it does not
call a live model.

## Permissions

- `resolve` (input validation, duplicate checks): `contents: read`.
- `validate` (reused `.NET Build Validation` workflow): `contents: read`.
- `publish` (tag + release creation): `contents: write`, and only runs when
  `dry_run` is `false` and `validate` succeeded. It checks out with
  `persist-credentials: false` and uses `GH_TOKEN` explicitly for every
  `gh`/GitHub API call.
- No job ever requests or uses NuGet/package publishing credentials — this
  workflow never runs `dotnet pack` or `dotnet nuget push`, and never will
  unless a packable project and feed are explicitly introduced in a future
  change.

## Duplicate behavior

If the resolved tag already has a tag or a published GitHub release, the
`resolve` job fails before any validation or publish work runs. The `publish`
job re-checks immediately before publishing, in case another run created the
same tag/release in the meantime. To re-release the same date, choose a
different tag (unusual) or delete the existing tag/release first (rare,
manual, outside this workflow).

## Recovery

- **Tag created but release publish failed:** `gh release create --target
  <sha>` creates the tag and the release in one API-driven step, so this
  should be rare. If it happens anyway, the `publish` job's cleanup step
  detects a tag with no matching release and deletes only that tag
  (`gh api -X DELETE .../git/refs/tags/<tag>`) — no history is rewritten, and
  no other tag or branch is touched. No manual cleanup should be needed.
- **`main` moved between dispatch and validation:** the `resolve` job
  compares the dispatched SHA against the live tip of `main` via the GitHub
  API and fails closed if they differ. Just re-run the workflow to capture
  the new tip.
- **Wrong branch/ref dispatched:** the workflow fails immediately if
  `github.ref` is not `refs/heads/main`, before any checkout or validation
  work happens.
