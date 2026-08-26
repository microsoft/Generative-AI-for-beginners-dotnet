# Releases

This repository is released manually via the **Squad Release**
(`.github/workflows/squad-release.yml`) GitHub Actions workflow. There is no
push-triggered release automation — a release is always a deliberate,
human-dispatched action.

## Tag convention

Release tags are the UTC calendar date the release was cut, in strict
`YYYY-MM-DD` form within 2000–2099 (for example `2026-08-24`). This matches
every tag already published for this repository (`git tag --list`) and the
namespace protected by the immutable ruleset below. If you leave the `tag`
input empty when dispatching the workflow, it computes the current UTC date
for you; if you supply one explicitly, it must be a real calendar date in
that exact range and format (`1999-12-31`, `2100-01-01`, and `2026-02-30`
are rejected).

## Immutable date-tag ruleset

**Prerequisite for this workflow to run at all.** Every date-form tag is
protected by exactly one **active repository tag ruleset**, which the
workflow itself verifies before it validates or publishes anything (dry run
and live alike). If this ruleset is missing, disabled, misconfigured, or
bypassable, the workflow refuses to run — there is no "temporarily skip
this check" path.

- **Ruleset:** `Immutable date-tag releases` (id `21576333`), target `tag`,
  enforcement `active`.
- **Pattern:** `refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]` — matches only
  the repository's 2000–2099 `YYYY-MM-DD` release-tag convention; no other
  tag names are affected.
- **Rules:** `update` (restrict updates/retargeting) and `deletion`
  (restrict deletion). Tag **creation** is deliberately left unrestricted —
  this ruleset makes a matching tag immutable *after* it exists, it does not
  block the workflow from creating one.
- **Bypass actors:** none. `current_user_can_bypass` is `never` for every
  actor, including repository admins and the workflow's own token — nobody
  can retarget or delete a matching tag through any path.

Why this exists: GitHub's Git Refs API has no conditional "delete only if
unreferenced" call, and `gh release create --verify-tag` verifies that a tag
*exists*, not that it still points at the exact object a previous step
validated. Without a ruleset, a small window remains between "this run
creates/verifies a tag" and "this run publishes a release from it" during
which another actor could retarget or delete that tag. The ruleset closes
that window at the repository level: once `refs/tags/<date>` is created, no
actor — including this workflow's own `GITHUB_TOKEN` — can move or delete it.
That is what makes binding `--verify-tag` to a freshly-created, freshly
read-back-and-verified annotated tag object safe (see "Duplicate behavior
and race safety" below).

If the ruleset is ever recreated (for example, after a repository transfer),
it must be re-created with exactly these properties — one active ruleset,
target `tag`, the exact 2000–2099 pattern shown above, `update` + `deletion`
rules, zero bypass actors, no `creation` rule, and an empty exclusion list —
and no second, overlapping ruleset should be added; the workflow fails closed
if it ever finds more than one active ruleset independently protecting the
same tag namespace, since that is an ambiguous protection state.

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
   exists for that tag, **verifies the immutable date-tag ruleset is active
   with no bypass** (see "Immutable date-tag ruleset" above — a dry run
   proves this prerequisite without creating anything), and runs the full
   reused `.NET Build Validation` workflow (all five Release solution
   builds, the test project, and every tracked file-based `app.cs` compile)
   against that SHA — without creating a tag or release. The job summary
   prints the exact SHA, tag, notes source, latest behavior, and protecting
   ruleset that a live run would use.
2. **Review the dry-run summary.** Confirm the SHA, tag, and notes source are
   what you expect.
3. **Live run.** Re-dispatch the same workflow with `dry_run: false` (all
   other inputs unchanged). Only after validation succeeds again does the
   workflow atomically create an annotated tag at the exact validated SHA and
   publish the GitHub release from that tag — see "Duplicate behavior and
   race safety" below for how tag creation and publishing are kept race-safe
   against concurrent actors.

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

## Duplicate behavior and race safety

If the resolved tag already has a tag or a published GitHub release, the
`resolve` job fails before any validation or publish work runs. Because a
second actor (another dispatch, a manual `git push --tags`, etc.) could still
create the same tag *after* `resolve` passes but before `publish` runs, the
`publish` job does not trust a duplicate-check-then-create sequence to be
race-safe — a check-then-act gap is exactly the kind of race that lets an
unvalidated commit get published under the tag `--target` would otherwise
have picked. Instead, `publish` uses an atomic protocol, backstopped by the
immutable date-tag ruleset described above, entirely within a single step:

1. **Re-verify the protecting ruleset** is still the exact same active,
   zero-bypass ruleset `resolve` found, immediately before any mutation
   (`validate` can take a while; this catches an admin disabling/editing it
   mid-run).
2. Create an **annotated Git tag object** (Git Tags API) pointing at the
   validated commit SHA, with a `github-actions[bot]` tagger identity. This
   step cannot race — tag objects are content-addressed and never collide.
3. **Atomically create `refs/tags/<tag>`** (Git Refs API) pointing at that
   exact tag-object SHA. This call is the true race boundary: GitHub rejects
   ref creation if the ref already exists, so if another actor's tag lands
   first, this run's ref-create fails and it stops immediately — it never
   publishes, and the dangling tag *object* it already created is
   unreachable (no ref points at it) and needs no cleanup.
4. **Read back and verify** the annotated tag object — its tag name, target
   commit SHA, object type (`commit`), and its own SHA — and verify the ref
   resolves to that exact object, before this tag is ever handed to `gh
   release create --verify-tag`.
5. **Publish** with `gh release create "$TAG" --verify-tag ...`. Because the
   date-tag ruleset (step 1) makes `refs/tags/<tag>` immutable — un-updatable
   and un-deletable by anyone, including this workflow's own token — the
   instant it was created in step 3, and step 4 just proved that ref points
   at exactly the object this run created from the validated commit,
   `--verify-tag` is now safely bound to that object: nothing between here
   and publish can retarget or delete it out from under this call. This is
   what closes the race a pre-ruleset design could not: `--verify-tag` alone
   only confirms a tag *exists*, not that nothing has touched it since.
6. **Re-read the release, ref, and tag object after publishing** and fail
   loudly (without attempting any repair or deletion) if any of them
   disagree with what was validated and published.

There is **no automatic tag cleanup** anywhere in this workflow. If tag
creation succeeds but release creation fails, the tag is retained
deliberately — see "Recovery" below.

## Recovery

- **Ref creation loses a race (another actor's tag lands first):** the Git
  Refs API rejects the create-ref call outright. The run fails immediately
  and publishes nothing. This run's own tag *object* (created in step 2
  above) is never referenced by any ref and requires no cleanup; the other
  actor's tag is never touched.
- **Tag ref created but release publish failed:** the tag is **retained on
  purpose**, as immutable audit evidence of exactly what commit was
  validated and tagged. The date-tag ruleset already makes it un-updatable
  and un-deletable, so there is nothing to "clean up" even if the workflow
  wanted to. To recover: investigate why `gh release create` failed, then
  either (a) re-run this workflow with a **new date tag** once the problem
  is fixed, or (b) have a repository admin manually publish a release from
  the existing tag with `gh release create <tag> --verify-tag ...` after
  confirming it is safe to do so. The orphaned tag is never deleted
  automatically, and the workflow does not attempt to retry or repair it.
- **Post-publish re-verification finds a mismatch:** the workflow fails
  loudly and leaves the release/tag/ref exactly as they are for manual
  investigation. It never attempts automatic repair or deletion — and the
  ruleset would block a delete/retarget attempt regardless.
- **The immutable date-tag ruleset is missing, disabled, misconfigured
  (wrong pattern, missing `update`/`deletion` rule, or blocks `creation`),
  or has any bypass actor:** both dry-run and live dispatches fail closed in
  the `resolve` job, before any validation or publish work runs. A
  repository admin must recreate the ruleset exactly as documented above
  (see "Immutable date-tag ruleset") before the workflow can run again.
- **`main` moved between dispatch and validation:** the `resolve` job
  compares the dispatched SHA against the live tip of `main` via the GitHub
  API and fails closed if they differ. Just re-run the workflow to capture
  the new tip.
- **Wrong branch/ref dispatched:** the workflow fails immediately if
  `github.ref` is not `refs/heads/main`, before any checkout or validation
  work happens.

## Testing the ruleset/tag-object guard logic

The ruleset-verification and tag-object read-back logic used by `resolve`
and `publish` lives in standalone, sourceable scripts so it can be tested
without touching GitHub:

- `.github/scripts/verify-tag-ruleset.sh` — `find_protecting_ruleset`
- `.github/scripts/verify-tag-object.sh` — `verify_tag_object`,
  `verify_release_published`

Run the mock test suite (no network access; `gh` is replaced with a fixture-
driven mock and every invocation is logged so tests can assert no `DELETE`
call was ever made):

```bash
bash .github/scripts/tests/test-squad-release-guards.sh
```

It covers: a missing ruleset, an inactive ruleset, a misconfigured ruleset
(no `deletion` rule; blocks `creation`), a bypassed ruleset, a non-matching
pattern, ambiguous duplicate rulesets, the successful path, an annotated-
object mismatch (wrong type, wrong target commit), a ref conflict (ref
replaced by another actor), a release-creation failure that must leave the
owned tag untouched, and a fully successful post-release re-verification.
