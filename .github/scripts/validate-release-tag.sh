#!/usr/bin/env bash
# Resolves an optional release tag and verifies that it belongs to the exact
# immutable namespace protected by the repository ruleset: 20xx-MM-DD.

set -euo pipefail

resolve_release_tag() {
  local input="${1:-}"
  local tag normalized

  if [ -z "$input" ]; then
    tag="$(date -u +%Y-%m-%d)"
  else
    tag="$input"
  fi

  if [[ ! "$tag" =~ ^20[0-9]{2}-[0-9]{2}-[0-9]{2}$ ]]; then
    echo "::error::tag '$tag' must be a date from 2000 through 2099 in YYYY-MM-DD format." >&2
    return 1
  fi

  normalized="$(date -u -d "$tag" +%Y-%m-%d 2>/dev/null)" || {
    echo "::error::tag '$tag' is not a valid calendar date." >&2
    return 1
  }
  if [ "$normalized" != "$tag" ]; then
    echo "::error::tag '$tag' is not a valid calendar date." >&2
    return 1
  fi

  printf '%s\n' "$tag"
}

if [ "${BASH_SOURCE[0]}" = "${0}" ]; then
  if [ "$#" -gt 1 ]; then
    echo "usage: $0 [YYYY-MM-DD]" >&2
    exit 2
  fi
  resolve_release_tag "${1:-}"
fi
