#!/usr/bin/env bash
# Verifies that exactly one *active* repository tag ruleset protects a given
# date-form release tag from update (retarget) and deletion, with zero bypass
# actors and without blocking tag *creation*. This is the repository-level
# guarantee squad-release.yml depends on: GitHub has no conditional
# ref-delete API and `gh release create --verify-tag` does not itself bind a
# mutable tag to a commit, so the workflow must fail closed unless this
# ruleset is present and correctly configured before it validates or
# publishes anything.
#
# All logic lives in find_protecting_ruleset() so this file can be sourced by
# tests without executing main (see .github/scripts/tests/).
#
# Usage: verify-tag-ruleset.sh <owner/repo> <tag>
# Requires: gh (authenticated via GH_TOKEN), jq
# On success, prints GITHUB_OUTPUT-compatible lines to stdout:
#   ruleset_id=<id>
#   ruleset_name=<name>
# On failure, prints one or more ::error:: lines to stderr and returns 1.

set -euo pipefail

find_protecting_ruleset() {
  local repo="$1" tag="$2"
  local target_ref="refs/tags/$tag"
  local expected_pattern='refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]'
  local ids id detail enforcement target bypass_count
  local has_update has_deletion has_creation include_count expected_include_count exclude_count
  local found_id="" found_name=""

  if ! ids="$(gh api "repos/$repo/rulesets?includes_parents=true&targets=tag" --paginate --jq '.[] | select(.enforcement=="active") | .id' 2>/dev/null | tr -d '\r')"; then
    echo "::error::failed to list repository tag rulesets for '$repo'. The workflow token could not read ruleset configuration." >&2
    return 1
  fi

  if [ -z "$ids" ]; then
    echo "::error::no active tag ruleset exists on '$repo'. Configure the required immutable date-tag ruleset (target: tag, rules: update + deletion, zero bypass_actors, ref_name.include matching the YYYY-MM-DD tag pattern, no creation restriction) before this workflow can validate or publish. See docs/releases/README.md#immutable-date-tag-ruleset." >&2
    return 1
  fi

  for id in $ids; do
    if ! detail="$(gh api "repos/$repo/rulesets/$id" 2>/dev/null | tr -d '\r')"; then
      echo "::error::failed to read ruleset $id detail for '$repo'." >&2
      return 1
    fi

    enforcement="$(jq -r '.enforcement' <<<"$detail")"
    target="$(jq -r '.target' <<<"$detail")"
    [ "$enforcement" = "active" ] || continue
    [ "$target" = "tag" ] || continue

    bypass_count="$(jq '(.bypass_actors // []) | length' <<<"$detail")"
    if [ "$bypass_count" -ne 0 ]; then
      # A non-empty bypass_actors list means some actor (a role, a team, an
      # app, or "OrganizationAdmin") could retarget/delete a matching tag.
      # That does not close the race this ruleset exists to close.
      continue
    fi

    has_update="$(jq '[(.rules // [])[] | select(.type=="update")] | length' <<<"$detail")"
    has_deletion="$(jq '[(.rules // [])[] | select(.type=="deletion")] | length' <<<"$detail")"
    has_creation="$(jq '[(.rules // [])[] | select(.type=="creation")] | length' <<<"$detail")"
    [ "$has_update" -ge 1 ] || continue
    [ "$has_deletion" -ge 1 ] || continue
    # A "creation" restriction (with no bypass) would block the workflow's
    # own tag creation. That is out of scope for this design and must not be
    # silently accepted as "protection".
    [ "$has_creation" -eq 0 ] || continue

    # Do not emulate GitHub's File.fnmatch(..., FNM_PATHNAME) semantics with
    # shell globs: Bash allows '*' to cross '/', while GitHub does not. Require
    # the reviewed repository policy exactly so a broader or excluded pattern
    # cannot make this fail-closed guard accept a misconfigured ruleset.
    include_count="$(jq '(.conditions.ref_name.include // []) | length' <<<"$detail")"
    expected_include_count="$(jq --arg expected "$expected_pattern" \
      '[(.conditions.ref_name.include // [])[] | select(. == $expected)] | length' <<<"$detail")"
    exclude_count="$(jq '(.conditions.ref_name.exclude // []) | length' <<<"$detail")"
    [ "$include_count" -eq 1 ] || continue
    [ "$expected_include_count" -eq 1 ] || continue
    [ "$exclude_count" -eq 0 ] || continue

    if [ -n "$found_id" ]; then
      echo "::error::multiple active tag rulesets on '$repo' independently protect '$target_ref' with zero bypass (ids $found_id and $id). Consolidate to exactly one ruleset — an ambiguous protection state is not acceptable." >&2
      return 1
    fi
    found_id="$id"
    found_name="$(jq -r '.name' <<<"$detail")"
  done

  if [ -z "$found_id" ]; then
    echo "::error::no active repository tag ruleset protects '$target_ref' from update/deletion with zero bypass actors and without a creation restriction. Configure it before this workflow can validate or publish. See docs/releases/README.md#immutable-date-tag-ruleset." >&2
    return 1
  fi

  echo "ruleset_id=$found_id"
  echo "ruleset_name=$found_name"
  return 0
}

if [ "${BASH_SOURCE[0]}" = "${0}" ]; then
  if [ "$#" -ne 2 ]; then
    echo "usage: $0 <owner/repo> <tag>" >&2
    exit 2
  fi
  find_protecting_ruleset "$1" "$2"
fi
