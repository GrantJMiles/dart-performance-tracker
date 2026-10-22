#!/bin/bash
# SessionStart hook for Claude Code cloud sessions: make sure the app can be built, run and tested.
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

cd "$CLAUDE_PROJECT_DIR"
./scripts/setup-cloud.sh

# Telemetry opt-outs persist for the session.
cat >> "$CLAUDE_ENV_FILE" <<'ENV'
export FUNCTIONS_CORE_TOOLS_TELEMETRY_OPTOUT=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV
