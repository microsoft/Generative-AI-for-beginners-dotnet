#!/usr/bin/env bash
# Regression tests for the BasicChat-10 response-length and context-carryover
# checks in .github/workflows/ollama-smoke-validation.yml.
#
# Issue #536 (reopened): the post-merge run 33176784855 failed because turn 2
# ("What did I just ask you about?") allowed phi4-mini to hallucinate off-topic
# content that omitted France/Paris/capital.  The fix changes the turn-2 probe
# to "Repeat your previous answer verbatim." — a verbatim-repeat instruction
# constrains the model to echo its own prior output, which must contain 'paris'
# if conversation history was correctly appended.
#
# These tests verify:
#   1. Response-length check: non-empty responses pass; empty/whitespace fail.
#   2. Context-carryover grep: responses that contain france|paris|capital pass;
#      unrelated/hallucinated responses (off-topic sentences, refusals, model
#      preambles without the key words) fail — proving the check would have
#      caught the run-33176784855 regression.
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
# With the verbatim-repeat probe ("Repeat your previous answer verbatim."),
# any valid model response will echo the first-turn answer which necessarily
# contains one of these keywords.  Off-topic hallucinations and refusal
# phrases that omit all three keywords must fail — exactly the class of output
# that caused the post-merge regression in run 33176784855 (issue #536).
# ---------------------------------------------------------------------------
echo
echo "== Context-carryover grep check (verbatim-repeat probe) =="

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

if check_context "Paris is the capital of France."; then
  pass "verbatim-repeat of full sentence: satisfies context check"
else
  fail "verbatim-repeat of full sentence: failed context check"
fi

if check_context "You asked about the capital of France."; then
  pass "sentence with 'capital of France': satisfies context check"
else
  fail "sentence with 'capital of France': failed context check"
fi

# --- Rejection tests: responses that must FAIL the check ---
# These simulate the class of hallucinations that caused run 33176784855 to fail
# even though conversation history was present.  The verbatim-repeat probe should
# produce a response containing 'paris', but the grep must also correctly reject
# off-topic or evasive output if the model drifts.

if check_context "I don't know."; then
  fail "unrelated response ('I don't know.'): incorrectly satisfied context check"
else
  pass "unrelated response ('I don't know.'): correctly fails context check"
fi

if check_context "As an AI language model, I can help you with many things."; then
  fail "off-topic preamble: incorrectly satisfied context check — regression of #536"
else
  pass "off-topic preamble without key words: correctly fails context check"
fi

if check_context "The weather forecast shows sunny skies all week."; then
  fail "unrelated hallucination: incorrectly satisfied context check — regression of #536"
else
  pass "unrelated hallucination ('weather forecast'): correctly fails context check"
fi

if check_context "I am unable to repeat that information."; then
  fail "refusal without topic keywords: incorrectly satisfied context check"
else
  pass "refusal without topic keywords: correctly fails context check"
fi

echo
echo "== summary: $PASS_COUNT passed, $FAIL_COUNT failed =="
[ "$FAIL_COUNT" -eq 0 ]
