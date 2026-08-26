#!/usr/bin/env bash
# Static/mock tests for the ruleset-guard and tag-object read-back logic used
# by .github/workflows/squad-release.yml. No network access, no real GitHub
# API calls: `gh` is replaced by a mock function per scenario that returns
# fixture JSON and records every invocation (including any attempted
# mutation) to a call log, so tests can assert "no DELETE call was ever
# made" as well as pass/fail outcomes.
#
# Run with: bash .github/scripts/tests/test-squad-release-guards.sh

set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
REPO_ROOT="$(cd -- "$SCRIPT_DIR/../../.." >/dev/null 2>&1 && pwd)"

# shellcheck source=../verify-tag-ruleset.sh
source "$REPO_ROOT/.github/scripts/verify-tag-ruleset.sh"
# shellcheck source=../verify-tag-object.sh
source "$REPO_ROOT/.github/scripts/verify-tag-object.sh"
# shellcheck source=../validate-release-tag.sh
source "$REPO_ROOT/.github/scripts/validate-release-tag.sh"

PASS_COUNT=0
FAIL_COUNT=0
SCRATCH_DIR="$SCRIPT_DIR/.scratch"
mkdir -p "$SCRATCH_DIR"
CALL_LOG="$SCRATCH_DIR/gh-calls.log"
STDOUT_CAPTURE="$SCRATCH_DIR/stdout.txt"
STDERR_CAPTURE="$SCRATCH_DIR/stderr.txt"
trap 'rm -rf "$SCRATCH_DIR"' EXIT

pass() { PASS_COUNT=$((PASS_COUNT + 1)); echo "  ok   - $1"; }
fail() { FAIL_COUNT=$((FAIL_COUNT + 1)); echo "  FAIL - $1"; }

reset_call_log() { : >"$CALL_LOG"; }
delete_calls_made() { grep -c -- '-X DELETE' "$CALL_LOG" 2>/dev/null || true; }

# --- Mock `gh` -------------------------------------------------------------
# GH_MOCK_RULESETS_JSON: JSON array for the rulesets list call.
# GH_MOCK_RULESET_DETAIL[<id>]: full ruleset detail JSON, keyed by id.
# GH_MOCK_TAG_OBJECT_JSON: JSON for `git/tags/<sha>`.
# GH_MOCK_REF_JSON: JSON for `git/ref/tags/<tag>`.
# GH_MOCK_RELEASE_JSON: JSON for `releases/tags/<tag>`.
declare -A GH_MOCK_RULESET_DETAIL
GH_MOCK_RULESETS_JSON="[]"
GH_MOCK_TAG_OBJECT_JSON=""
GH_MOCK_REF_JSON=""
GH_MOCK_RELEASE_JSON=""
GH_MOCK_TAG_OBJECT_FAIL="false"
GH_MOCK_REF_FAIL="false"
GH_MOCK_RELEASE_FAIL="false"

gh() {
  echo "gh $*" >>"$CALL_LOG"
  local args=("$@")
  if [ "${args[0]}" = "api" ]; then
    local path="${args[1]}"
    case "$path" in
      *"-X"*) : ;; # not used; -X is a separate arg, handled below
    esac
    # detect -X DELETE anywhere in args
    local i method=""
    for ((i = 0; i < ${#args[@]}; i++)); do
      if [ "${args[$i]}" = "-X" ]; then
        method="${args[$((i + 1))]}"
      fi
    done
    if [ "$method" = "DELETE" ]; then
      echo '{}'
      return 0
    fi
    case "$path" in
      repos/*/rulesets\?*)
        if [ -n "${GH_MOCK_RULESETS_FAIL:-}" ]; then return 1; fi
        # Emulate --jq filtering of the list endpoint for our two call
        # shapes used by verify-tag-ruleset.sh.
        echo "$GH_MOCK_RULESETS_JSON" | jq -r '.[] | select(.enforcement=="active") | .id'
        return 0
        ;;
      repos/*/rulesets/*)
        local id="${path##*/}"
        if [ -n "${GH_MOCK_RULESET_DETAIL[$id]:-}" ]; then
          echo "${GH_MOCK_RULESET_DETAIL[$id]}"
          return 0
        fi
        return 1
        ;;
      repos/*/git/tags/*)
        if [ "$GH_MOCK_TAG_OBJECT_FAIL" = "true" ]; then return 1; fi
        echo "$GH_MOCK_TAG_OBJECT_JSON"
        return 0
        ;;
      repos/*/git/ref/tags/*)
        if [ "$GH_MOCK_REF_FAIL" = "true" ]; then return 1; fi
        echo "$GH_MOCK_REF_JSON"
        return 0
        ;;
      repos/*/releases/tags/*)
        if [ "$GH_MOCK_RELEASE_FAIL" = "true" ]; then return 1; fi
        echo "$GH_MOCK_RELEASE_JSON"
        return 0
        ;;
      *)
        return 1
        ;;
    esac
  fi
  return 1
}

VALID_DETAIL='{
  "id": 1, "name": "Immutable date-tag releases", "target": "tag",
  "enforcement": "active", "bypass_actors": [],
  "conditions": {"ref_name": {"include": ["refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"], "exclude": []}},
  "rules": [{"type": "update"}, {"type": "deletion"}]
}'

echo "== validate-release-tag.sh =="

if [ "$(resolve_release_tag "2026-08-26" 2>"$STDERR_CAPTURE")" = "2026-08-26" ]; then
  pass "protected 20xx date: accepted"
else
  fail "protected 20xx date: expected success"
fi

for unprotected_tag in "1999-12-31" "2100-01-01"; do
  if resolve_release_tag "$unprotected_tag" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
    fail "unprotected date $unprotected_tag: expected failure, got success"
  else
    pass "unprotected date $unprotected_tag: fails closed"
  fi
done

if resolve_release_tag "2026-02-30" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "calendar-invalid date: expected failure, got success"
else
  pass "calendar-invalid date: fails closed"
fi

echo
echo "== verify-tag-ruleset.sh =="

# 1. Missing ruleset (empty active list)
reset_call_log
GH_MOCK_RULESETS_JSON='[]'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "missing ruleset: expected failure, got success"
else
  grep -q "no active tag ruleset exists" "$STDERR_CAPTURE" && pass "missing ruleset: fails closed with actionable message" \
    || fail "missing ruleset: failed but message was not actionable"
fi

# 2. Inactive ruleset (enforcement=disabled) never appears in the active list
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 1, "enforcement": "disabled"}]'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "inactive ruleset: expected failure, got success"
else
  pass "inactive ruleset: fails closed (disabled ruleset excluded from candidates)"
fi

# 3. Misconfigured: active, target tag, matches pattern, but missing deletion rule
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 2, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[2]='{
  "id": 2, "name": "Update-only", "target": "tag", "enforcement": "active", "bypass_actors": [],
  "conditions": {"ref_name": {"include": ["refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"]}},
  "rules": [{"type": "update"}]
}'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "misconfigured (no deletion rule): expected failure, got success"
else
  pass "misconfigured (no deletion rule): fails closed"
fi

# 3b. Misconfigured: blocks creation too (would break automated publication)
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 3, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[3]='{
  "id": 3, "name": "Too strict", "target": "tag", "enforcement": "active", "bypass_actors": [],
  "conditions": {"ref_name": {"include": ["refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"]}},
  "rules": [{"type": "creation"}, {"type": "update"}, {"type": "deletion"}]
}'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "misconfigured (blocks creation): expected failure, got success"
else
  pass "misconfigured (blocks creation): fails closed"
fi

# 4. Bypassed: active, correct rules/pattern, but has a bypass actor
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 4, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[4]='{
  "id": 4, "name": "Bypassable", "target": "tag", "enforcement": "active",
  "bypass_actors": [{"actor_id": 1, "actor_type": "OrganizationAdmin", "bypass_mode": "always"}],
  "conditions": {"ref_name": {"include": ["refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"]}},
  "rules": [{"type": "update"}, {"type": "deletion"}]
}'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "bypassed ruleset: expected failure, got success"
else
  pass "bypassed ruleset: fails closed (bypass actor present)"
fi

# 5. Pattern does not match the tag under test
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 5, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[5]='{
  "id": 5, "name": "Wrong pattern", "target": "tag", "enforcement": "active", "bypass_actors": [],
  "conditions": {"ref_name": {"include": ["refs/tags/v*"]}},
  "rules": [{"type": "update"}, {"type": "deletion"}]
}'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "non-matching pattern: expected failure, got success"
else
  pass "non-matching pattern: fails closed"
fi

# 5b. A broad Bash-matching pattern must be rejected. GitHub evaluates
# ruleset patterns with File.fnmatch(..., FNM_PATHNAME), where '*' cannot
# cross '/', so refs/* does not protect refs/tags/YYYY-MM-DD.
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 51, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[51]='{
  "id": 51, "name": "Bash-only broad match", "target": "tag", "enforcement": "active", "bypass_actors": [],
  "conditions": {"ref_name": {"include": ["refs/*"], "exclude": []}},
  "rules": [{"type": "update"}, {"type": "deletion"}]
}'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "GitHub-incompatible broad pattern: expected failure, got success"
else
  pass "GitHub-incompatible broad pattern: fails closed"
fi

# 5c. Even the exact include is insufficient when an exclusion is present.
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 52, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[52]='{
  "id": 52, "name": "Excluded release tag", "target": "tag", "enforcement": "active", "bypass_actors": [],
  "conditions": {
    "ref_name": {
      "include": ["refs/tags/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"],
      "exclude": ["refs/tags/2026-08-26"]
    }
  },
  "rules": [{"type": "update"}, {"type": "deletion"}]
}'
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "ruleset with exclusion: expected failure, got success"
else
  pass "ruleset with exclusion: fails closed"
fi

# 6. Duplicate: two independently-valid active rulesets both match
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 6, "enforcement": "active"}, {"id": 7, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[6]="$(jq '.id = 6' <<<"$VALID_DETAIL")"
GH_MOCK_RULESET_DETAIL[7]="$(jq '.id = 7 | .name = "Duplicate"' <<<"$VALID_DETAIL")"
if find_protecting_ruleset "o/r" "2026-08-26" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "duplicate matching rulesets: expected failure, got success"
else
  grep -q "multiple active tag rulesets" "$STDERR_CAPTURE" && pass "duplicate matching rulesets: fails closed as ambiguous" \
    || fail "duplicate matching rulesets: failed but wrong message"
fi

# 7. Successful path
reset_call_log
GH_MOCK_RULESETS_JSON='[{"id": 21576333, "enforcement": "active"}]'
GH_MOCK_RULESET_DETAIL[21576333]="$(jq '.id = 21576333' <<<"$VALID_DETAIL")"
if OUT="$(find_protecting_ruleset "o/r" "2026-08-26" 2>"$STDERR_CAPTURE")"; then
  if grep -q "^ruleset_id=21576333$" <<<"$OUT" && grep -q "^ruleset_name=Immutable date-tag releases$" <<<"$OUT"; then
    pass "successful path: exact match found, id/name emitted correctly"
  else
    fail "successful path: succeeded but output was wrong: $OUT"
  fi
  [ "$(delete_calls_made)" -eq 0 ] && pass "successful path: no DELETE call was ever issued" \
    || fail "successful path: unexpected DELETE call recorded"
else
  fail "successful path: expected success, got failure ($(cat "$STDERR_CAPTURE"))"
fi

echo
echo "== verify-tag-object.sh =="

COMMIT_SHA="cccccccccccccccccccccccccccccccccccccccc"
TAGOBJ_SHA="tttttttttttttttttttttttttttttttttttttttt"

# 8. Successful tag-object + ref verification
reset_call_log
GH_MOCK_TAG_OBJECT_JSON="$(jq -n --arg tag "2026-08-26" --arg sha "$COMMIT_SHA" --arg self "$TAGOBJ_SHA" \
  '{tag: $tag, object: {sha: $sha, type: "commit"}, sha: $self}')"
GH_MOCK_REF_JSON="$(jq -n --arg self "$TAGOBJ_SHA" '{object: {sha: $self}}')"
if verify_tag_object "o/r" "2026-08-26" "$COMMIT_SHA" "$TAGOBJ_SHA" 2>"$STDERR_CAPTURE"; then
  pass "tag-object verification: matching object/ref accepted"
else
  fail "tag-object verification: expected success, got: $(cat "$STDERR_CAPTURE")"
fi

# 9. Annotated-object mismatch (wrong object type — not a commit)
reset_call_log
GH_MOCK_TAG_OBJECT_JSON="$(jq -n --arg tag "2026-08-26" --arg sha "$COMMIT_SHA" --arg self "$TAGOBJ_SHA" \
  '{tag: $tag, object: {sha: $sha, type: "tag"}, sha: $self}')"
if verify_tag_object "o/r" "2026-08-26" "$COMMIT_SHA" "$TAGOBJ_SHA" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "annotated-object mismatch (type): expected failure, got success"
else
  grep -q "object type" "$STDERR_CAPTURE" && pass "annotated-object mismatch (type): fails loudly" \
    || fail "annotated-object mismatch (type): wrong message"
fi

# 9b. Annotated-object mismatch (wrong target commit)
reset_call_log
GH_MOCK_TAG_OBJECT_JSON="$(jq -n --arg tag "2026-08-26" --arg sha "0000000000000000000000000000000000000000" --arg self "$TAGOBJ_SHA" \
  '{tag: $tag, object: {sha: $sha, type: "commit"}, sha: $self}')"
if verify_tag_object "o/r" "2026-08-26" "$COMMIT_SHA" "$TAGOBJ_SHA" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "annotated-object mismatch (commit): expected failure, got success"
else
  pass "annotated-object mismatch (commit): fails loudly"
fi

# 10. Ref conflict: ref points at a different object than expected
reset_call_log
GH_MOCK_TAG_OBJECT_JSON="$(jq -n --arg tag "2026-08-26" --arg sha "$COMMIT_SHA" --arg self "$TAGOBJ_SHA" \
  '{tag: $tag, object: {sha: $sha, type: "commit"}, sha: $self}')"
GH_MOCK_REF_JSON='{"object": {"sha": "ffffffffffffffffffffffffffffffffffffffff"}}'
if verify_tag_object "o/r" "2026-08-26" "$COMMIT_SHA" "$TAGOBJ_SHA" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "ref conflict: expected failure, got success"
else
  grep -q "Another actor holds this ref" "$STDERR_CAPTURE" && pass "ref conflict: fails loudly, attributes ownership correctly" \
    || fail "ref conflict: wrong message"
fi
[ "$(delete_calls_made)" -eq 0 ] && pass "ref conflict: no DELETE call was ever issued" \
  || fail "ref conflict: unexpected DELETE call recorded"

# 11. Release-failure-leaves-owned-tag: post-release check runs against a
# release read-back failure (simulating gh release create having failed, or
# a transient read failure) and must fail loudly without ever calling DELETE.
reset_call_log
GH_MOCK_RELEASE_FAIL="true"
if verify_release_published "o/r" "2026-08-26" "$COMMIT_SHA" "$TAGOBJ_SHA" >"$STDOUT_CAPTURE" 2>"$STDERR_CAPTURE"; then
  fail "release failure leaves owned tag: expected failure, got success"
else
  grep -q "Leaving all state as-is" "$STDERR_CAPTURE" && pass "release failure leaves owned tag: fails loudly, no repair attempted" \
    || fail "release failure leaves owned tag: wrong message"
fi
[ "$(delete_calls_made)" -eq 0 ] && pass "release failure leaves owned tag: no DELETE call was ever issued" \
  || fail "release failure leaves owned tag: unexpected DELETE call recorded"
GH_MOCK_RELEASE_FAIL="false"

# 12. Successful full post-release verification
reset_call_log
GH_MOCK_RELEASE_JSON="$(jq -n --arg tag "2026-08-26" --arg sha "main" '{tag_name: $tag, target_commitish: $sha}')"
GH_MOCK_TAG_OBJECT_JSON="$(jq -n --arg tag "2026-08-26" --arg sha "$COMMIT_SHA" --arg self "$TAGOBJ_SHA" \
  '{tag: $tag, object: {sha: $sha, type: "commit"}, sha: $self}')"
GH_MOCK_REF_JSON="$(jq -n --arg self "$TAGOBJ_SHA" '{object: {sha: $self}}')"
if verify_release_published "o/r" "2026-08-26" "$COMMIT_SHA" "$TAGOBJ_SHA" 2>"$STDERR_CAPTURE"; then
  pass "successful full path: post-release re-verification passes end to end"
else
  fail "successful full path: expected success, got: $(cat "$STDERR_CAPTURE")"
fi
[ "$(delete_calls_made)" -eq 0 ] && pass "successful full path: no DELETE call was ever issued" \
  || fail "successful full path: unexpected DELETE call recorded"

rm -f "$STDOUT_CAPTURE" "$STDERR_CAPTURE"

echo
echo "== summary: $PASS_COUNT passed, $FAIL_COUNT failed =="
[ "$FAIL_COUNT" -eq 0 ]
