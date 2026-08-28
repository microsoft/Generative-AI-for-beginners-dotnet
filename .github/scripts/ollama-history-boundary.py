#!/usr/bin/env python3
"""Deterministic Ollama chat stub and conversation-history payload validator."""

import argparse
import json
import sys
import time
from http.server import BaseHTTPRequestHandler, HTTPServer
from pathlib import Path

FIRST_USER = "boundary-first-user-turn"
SECOND_USER = "boundary-second-user-turn"
FIRST_ASSISTANT = "BOUNDARY_ASSISTANT_RESPONSE_7f3c9a"
SECOND_ASSISTANT = "BOUNDARY_SECOND_RESPONSE_OK"
EXPECTED_MODEL = "phi4-mini"


def validate_capture(capture):
    if not isinstance(capture, list) or len(capture) != 2:
        return "expected exactly two outbound chat requests"

    first, second = capture
    for index, request in enumerate(capture, start=1):
        if request.get("model") != EXPECTED_MODEL:
            return f"request {index} did not use model {EXPECTED_MODEL!r}"
        if request.get("stream") is not True:
            return f"request {index} did not request a streamed response"

    first_messages = first.get("messages")
    second_messages = second.get("messages")
    if not isinstance(first_messages, list) or not isinstance(second_messages, list):
        return "both requests must contain message arrays"

    if [message.get("role") for message in first_messages] != ["system", "user"]:
        return "first request roles must be exactly system,user"
    if first_messages[-1].get("content") != FIRST_USER:
        return "first request is missing the exact first user turn"

    if [message.get("role") for message in second_messages] != [
        "system",
        "user",
        "assistant",
        "user",
    ]:
        return "second request roles must be exactly system,user,assistant,user"
    if second_messages[1].get("content") != FIRST_USER:
        return "second request is missing the exact first user turn"
    if second_messages[2].get("content") != FIRST_ASSISTANT:
        return "second request is missing the exact first assistant response"
    if second_messages[3].get("content") != SECOND_USER:
        return "second request is missing the exact second user turn"

    return None


def response_line(content, done):
    return json.dumps(
        {
            "model": EXPECTED_MODEL,
            "created_at": "2026-01-01T00:00:00Z",
            "message": {"role": "assistant", "content": content},
            "done": done,
        },
        separators=(",", ":"),
    ).encode("utf-8") + b"\n"


def serve(capture_path):
    capture = []

    class Handler(BaseHTTPRequestHandler):
        def do_POST(self):
            if self.path != "/api/chat":
                self.send_error(404)
                return

            try:
                length = int(self.headers.get("Content-Length", "0"))
                request = json.loads(self.rfile.read(length))
            except (ValueError, json.JSONDecodeError) as error:
                self.send_error(400, str(error))
                return

            capture.append(request)
            content = FIRST_ASSISTANT if len(capture) == 1 else SECOND_ASSISTANT
            body = response_line(content, False) + response_line("", True)
            self.send_response(200)
            self.send_header("Content-Type", "application/x-ndjson")
            self.send_header("Content-Length", str(len(body)))
            self.end_headers()
            self.wfile.write(body)

        def log_message(self, format, *args):
            return

    server = HTTPServer(("127.0.0.1", 11434), Handler)
    server.timeout = 1
    deadline = time.monotonic() + 120
    while len(capture) < 2 and time.monotonic() < deadline:
        server.handle_request()
    server.server_close()

    capture_path.write_text(json.dumps(capture, indent=2) + "\n", encoding="utf-8")
    error = validate_capture(capture)
    if error:
        print(f"Boundary validation failed: {error}", file=sys.stderr)
        return 1
    print("Boundary validation passed: second request contains the exact assistant response with role assistant.")
    return 0


def validate(capture_path):
    try:
        capture = json.loads(capture_path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as error:
        print(f"Boundary validation failed: {error}", file=sys.stderr)
        return 1

    error = validate_capture(capture)
    if error:
        print(f"Boundary validation failed: {error}", file=sys.stderr)
        return 1
    print("Boundary validation passed.")
    return 0


def main():
    parser = argparse.ArgumentParser()
    subparsers = parser.add_subparsers(dest="command", required=True)
    for command in ("serve", "validate"):
        command_parser = subparsers.add_parser(command)
        command_parser.add_argument("capture", type=Path)
    args = parser.parse_args()
    return serve(args.capture) if args.command == "serve" else validate(args.capture)


if __name__ == "__main__":
    sys.exit(main())
