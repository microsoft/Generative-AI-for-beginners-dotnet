#!/usr/bin/env bash
# Read-back verification for the annotated tag object and ref created by
# squad-release.yml's publish job, plus post-release re-verification.
#
# Because the immutable-date-tag ruleset (see verify-tag-ruleset.sh) makes a
# matching ref un-updatable and un-deletable *after* creation, binding
# `gh release create --verify-tag` to a specific object is only safe if the
# ref was proven to point at our own freshly-created, correctly-shaped
# annotated tag object *before* publishing, and proven unchanged *after*
# publishing. These functions implement that proof. Neither function ever
# deletes or repairs anything — on any mismatch they fail loudly and leave
# all state as-is for manual investigation, per the "no destructive repair"
# rule.
#
# All logic lives in functions so this file can be sourced by tests without
# executing main.

set -euo pipefail

# verify_tag_object <repo> <tag> <expected_commit_sha> <expected_tag_object_sha>
#
# Reads the annotated tag object back from the Git Tags API and the ref back
# from the Git Refs API, and asserts:
#   - the tag object's .tag equals <tag>
#   - the tag object's .object.sha equals <expected_commit_sha>
#   - the tag object's .object.type equals "commit"
#   - the tag object's own .sha equals <expected_tag_object_sha>
#   - the ref refs/tags/<tag> resolves to <expected_tag_object_sha>
verify_tag_object() {
  local repo="$1" tag="$2" expected_sha="$3" expected_tag_object_sha="$4"
  local tag_obj actual_tag actual_object_sha actual_object_type actual_self_sha
  local ref_obj actual_ref_sha

  if ! tag_obj="$(gh api "repos/$repo/git/tags/$expected_tag_object_sha" 2>/dev/null | tr -d '\r')"; then
    echo "::error::failed to read back annotated tag object '$expected_tag_object_sha' for tag '$tag'. Refusing to publish." >&2
    return 1
  fi

  actual_tag="$(jq -r '.tag' <<<"$tag_obj")"
  actual_object_sha="$(jq -r '.object.sha' <<<"$tag_obj")"
  actual_object_type="$(jq -r '.object.type' <<<"$tag_obj")"
  actual_self_sha="$(jq -r '.sha' <<<"$tag_obj")"

  if [ "$actual_tag" != "$tag" ]; then
    echo "::error::annotated tag object '$expected_tag_object_sha' has tag name '$actual_tag', expected '$tag'. Refusing to publish." >&2
    return 1
  fi
  if [ "$actual_object_sha" != "$expected_sha" ]; then
    echo "::error::annotated tag object '$expected_tag_object_sha' points at commit '$actual_object_sha', expected '$expected_sha'. Refusing to publish." >&2
    return 1
  fi
  if [ "$actual_object_type" != "commit" ]; then
    echo "::error::annotated tag object '$expected_tag_object_sha' has object type '$actual_object_type', expected 'commit'. Refusing to publish." >&2
    return 1
  fi
  if [ "$actual_self_sha" != "$expected_tag_object_sha" ]; then
    echo "::error::annotated tag object read back as '$actual_self_sha', expected '$expected_tag_object_sha'. Refusing to publish." >&2
    return 1
  fi

  if ! ref_obj="$(gh api "repos/$repo/git/ref/tags/$tag" 2>/dev/null | tr -d '\r')"; then
    echo "::error::failed to read back ref 'refs/tags/$tag'. Refusing to publish." >&2
    return 1
  fi
  actual_ref_sha="$(jq -r '.object.sha' <<<"$ref_obj")"
  if [ "$actual_ref_sha" != "$expected_tag_object_sha" ]; then
    echo "::error::ref 'refs/tags/$tag' resolves to '$actual_ref_sha', expected this run's tag object '$expected_tag_object_sha'. Another actor holds this ref. Refusing to publish." >&2
    return 1
  fi

  return 0
}

# verify_release_published <repo> <tag> <expected_commit_sha> <expected_tag_object_sha>
#
# Post-release re-verification: re-reads the release, the ref, and the tag
# object, and fails loudly (without attempting any repair or deletion) if any
# of them disagree with what was validated before publishing.
verify_release_published() {
  local repo="$1" tag="$2" expected_sha="$3" expected_tag_object_sha="$4"
  local release_obj actual_release_tag actual_release_sha

  if ! release_obj="$(gh api "repos/$repo/releases/tags/$tag" 2>/dev/null | tr -d '\r')"; then
    echo "::error::release '$tag' was reported as created but could not be read back. Leaving all state as-is for manual investigation; do not retry automatically." >&2
    return 1
  fi

  actual_release_tag="$(jq -r '.tag_name' <<<"$release_obj")"
  actual_release_sha="$(jq -r '.target_commitish' <<<"$release_obj")"

  if [ "$actual_release_tag" != "$tag" ]; then
    echo "::error::published release reports tag_name '$actual_release_tag', expected '$tag'. Leaving all state as-is for manual investigation." >&2
    return 1
  fi

  # target_commitish on a release created from an existing annotated tag
  # reflects the branch/SHA recorded at release-creation time; it must still
  # resolve to the exact validated commit.
  if [ -n "$actual_release_sha" ] && [ "$actual_release_sha" != "$expected_sha" ] && [ "$actual_release_sha" != "main" ]; then
    echo "::error::published release '$tag' reports target_commitish '$actual_release_sha', expected '$expected_sha' (or the default branch pointer). Leaving all state as-is for manual investigation." >&2
    return 1
  fi

  if ! verify_tag_object "$repo" "$tag" "$expected_sha" "$expected_tag_object_sha"; then
    echo "::error::post-release re-verification of the tag/ref/object for '$tag' failed. The release exists but the tag metadata no longer matches what was validated. Leaving all state as-is for manual investigation — do not attempt automatic repair or deletion." >&2
    return 1
  fi

  return 0
}

if [ "${BASH_SOURCE[0]}" = "${0}" ]; then
  echo "this script defines functions for sourcing; it is not meant to be run directly." >&2
  exit 2
fi
