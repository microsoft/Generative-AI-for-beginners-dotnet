#!/usr/bin/env bash
# Regression tests for the BasicChat-10 response-length check in
# .github/workflows/ollama-smoke-validation.yml.
#
# Issue #536: the workflow previously rejected valid concise model responses
# (e.g. "Paris") because it required >= 3 words. The fix accepts any
# non-empty response; these tests prove:
#   - A one-word answer ("Paris") passes.
#   - A multi-word answer ("Paris is the capital of France.") passes.
#   - An empty response fails.
#
# No network access, no real Ollama daemon, no cloud secrets.
# Run with: bash .github/scripts/tests/test-ollama-smoke-response-check.sh

set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
SCRATCH_DIR="$SCRIPT_DIR/.scratch-smoke"
mkdir -p "$SCRATCH_DIR"
trap 'rm -rf "$SCRATCH_DIR"' EXIT

PASS_COUNT=0
FAIL_COUNT=0

pass() { PASS_COUNT=$((PASS_COUNT + 1)); echo "  ok   - $1"; }
fail() { FAIL_COUNT=$((FAIL_COUNT + 1)); echo "  FAIL - $1"; }

# ---------------------------------------------------------------------------
# check_response mirrors the post-fix check from the workflow:
#   - returns 0 (success) when word_count >= 1
#   - returns 1 (failure) when word_count == 0
# ---------------------------------------------------------------------------
check_response() {
  local label="$1"
  local content="$2"
  local tmp_file="$SCRATCH_DIR/resp.txt"
  printf '%s' "$content" > "$tmp_file"
  local word_count
  word_count=$(wc -w < "$tmp_file")
  if [ "$word_count" -lt 1 ]; then
    return 1
  fi
  return 0
}

echo "== BasicChat-10 response-length check (issue #536 regression) =="

# 1. Single-word concise answer: must pass after the fix.
if check_response "first" "Paris"; then
  pass "one-word response ('Paris'): accepted"
else
  fail "one-word response ('Paris'): rejected — regression of #536"
fi

# 2. Multi-word answer: must still pass.
if check_response "first" "Paris is the capital of France."; then
  pass "multi-word response: accepted"
else
  fail "multi-word response: rejected"
fi

# 3. Empty response: must be rejected (model did not respond).
if check_response "first" ""; then
  fail "empty response: accepted — should have been rejected"
else
  pass "empty response: rejected correctly"
fi

# 4. Whitespace-only response: must be rejected (wc -w counts 0 words).
if check_response "first" "   "; then
  fail "whitespace-only response: accepted — should have been rejected"
else
  pass "whitespace-only response: rejected correctly"
fi

# 5. Two-word answer: accepted.
if check_response "second" "France. Paris."; then
  pass "two-word response: accepted"
else
  fail "two-word response: rejected"
fi

# ---------------------------------------------------------------------------
# Context-carryover check (response2 must reference france/paris/capital).
# This test is independent of word count: a one-word "France" must satisfy
# the grep even though it would have been rejected by the old threshold.
# ---------------------------------------------------------------------------
echo
echo "== Context-carryover grep check =="

check_context() {
  local content="$1"
  local tmp_file="$SCRATCH_DIR/ctx.txt"
  printf '%s' "$content" > "$tmp_file"
  grep -qiE "france|paris|capital" "$tmp_file"
}

if check_context "France"; then
  pass "one-word 'France': satisfies context check"
else
  fail "one-word 'France': failed context check"
fi

if check_context "Paris"; then
  pass "one-word 'Paris': satisfies context check"
else
  fail "one-word 'Paris': failed context check"
fi

if check_context "You asked about the capital of France."; then
  pass "sentence with 'capital of France': satisfies context check"
else
  fail "sentence with 'capital of France': failed context check"
fi

if check_context "I don't know."; then
  fail "unrelated response: incorrectly satisfied context check"
else
  pass "unrelated response: correctly fails context check"
fi

echo
echo "== summary: $PASS_COUNT passed, $FAIL_COUNT failed =="
[ "$FAIL_COUNT" -eq 0 ]
