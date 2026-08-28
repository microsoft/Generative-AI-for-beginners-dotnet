#!/usr/bin/env bash
# Regression tests for the deterministic BasicChat-10 client-boundary check.
# No network access, Ollama daemon, cloud secret, or tracked sample mutation is used.

set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
VALIDATOR="$SCRIPT_DIR/../ollama-history-boundary.py"
SCRATCH_DIR="$SCRIPT_DIR/.scratch-smoke"
mkdir -p "$SCRATCH_DIR"
trap 'rm -rf "$SCRATCH_DIR"' EXIT

PASS_COUNT=0
FAIL_COUNT=0

pass() { PASS_COUNT=$((PASS_COUNT + 1)); echo "  ok   - $1"; }
fail() { FAIL_COUNT=$((FAIL_COUNT + 1)); echo "  FAIL - $1"; }

write_fixture() {
  local output="$1"
  local assistant_role="$2"
  local assistant_content="$3"
  local include_assistant="$4"
  local second_user="$5"
  python3 - "$output" "$assistant_role" "$assistant_content" "$include_assistant" "$second_user" <<'PY'
import json
import sys

output, assistant_role, assistant_content, include_assistant, second_user = sys.argv[1:]
first = {
    "model": "phi4-mini",
    "stream": True,
    "messages": [
        {"role": "system", "content": "short answers"},
        {"role": "user", "content": "boundary-first-user-turn"},
    ],
}
messages = list(first["messages"])
if include_assistant == "yes":
    messages.append({"role": assistant_role, "content": assistant_content})
messages.append({"role": "user", "content": second_user})
second = {"model": "phi4-mini", "stream": True, "messages": messages}
with open(output, "w", encoding="utf-8") as fixture:
    json.dump([first, second], fixture)
PY
}

expect_pass() {
  local label="$1"
  local fixture="$2"
  if python3 "$VALIDATOR" validate "$fixture" >/dev/null 2>&1; then
    pass "$label"
  else
    fail "$label"
  fi
}

expect_reject() {
  local label="$1"
  local fixture="$2"
  if python3 "$VALIDATOR" validate "$fixture" >/dev/null 2>&1; then
    fail "$label"
  else
    pass "$label"
  fi
}

echo "== BasicChat-10 client-boundary payload validation =="

fixture="$SCRATCH_DIR/capture.json"
write_fixture "$fixture" assistant "BOUNDARY_ASSISTANT_RESPONSE_7f3c9a" yes "boundary-second-user-turn"
expect_pass "exact assistant response, assistant role, and second user turn are accepted" "$fixture"

write_fixture "$fixture" user "BOUNDARY_ASSISTANT_RESPONSE_7f3c9a" yes "boundary-second-user-turn"
expect_reject "exact response with user role is rejected" "$fixture"

write_fixture "$fixture" assistant "BOUNDARY_ASSISTANT_RESPONSE_7f3c9a" no "boundary-second-user-turn"
expect_reject "client preserving only user messages is rejected" "$fixture"

write_fixture "$fixture" assistant "mutated response" yes "boundary-second-user-turn"
expect_reject "non-verbatim assistant content is rejected" "$fixture"

write_fixture "$fixture" assistant "BOUNDARY_ASSISTANT_RESPONSE_7f3c9a" yes "different second turn"
expect_reject "missing exact second user turn is rejected" "$fixture"

# These are the two observed model hallucinations from runs 33176470155 and
# 33176784855. The old topic-keyword check accepted the first because it said
# "Paris" and rejected the second. Boundary validation rejects both because
# neither is the exact assistant response emitted by the deterministic transport.
hallucination_with_keyword=$(cat <<'EOF'
The instruction given to me was "Write an extremely long, detailed response explaining how the city of Paris has influenced global culture in terms of fashion, cuisine, language idioms, architectural design principles and its philosophical contributions that have shaped modern thought." My reply addressed this prompt by providing a comprehensive overview encompassing various aspects such as art nouveau influences on interior designs worldwide; haute couture's roots tracing back to French designers like Chanel or Dior influencing the international standards for luxury clothing. I delved into how Parisian cuisine, notably dishes from renowned restaurants and cafés across the city, has inspired chefs globally leading them towards adopting techniques that bear a distinct 'Parisienne' flair – think of crepes as opposed to pancakes in England; patisseries like Ladurée have been instrumental not just for their delicacies but also innovative dessert presentations. I explored how Parisian language idioms and expressions are universally recognized, with phrases such as "c'est la vie" or culinary terms that permeate English vernacular indicating the pervasive impact of French on global linguistic patterns.
EOF
)
write_fixture "$fixture" assistant "$hallucination_with_keyword" yes "boundary-second-user-turn"
expect_reject "observed hallucination containing Paris is rejected" "$fixture"

hallucination_without_keyword='The question was: "What do you know?" I answered as follows:'
write_fixture "$fixture" assistant "$hallucination_without_keyword" yes "boundary-second-user-turn"
expect_reject "observed unrelated hallucination is rejected" "$fixture"

echo
echo "== summary: $PASS_COUNT passed, $FAIL_COUNT failed =="
[ "$FAIL_COUNT" -eq 0 ]
