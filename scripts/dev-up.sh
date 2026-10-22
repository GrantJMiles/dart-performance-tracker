#!/bin/bash
# Starts the local stack on SQLite: Azurite, Functions API (:7071) and Blazor client (:5211).
# Idempotent: anything already listening is left alone. Logs and pids go to .dev/.
# Usage: scripts/dev-up.sh [--reset]   (--reset deletes the SQLite db so it is re-created and re-seeded)
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEV="$ROOT/.dev"
API_PORT=7071
CLIENT_PORT=5211
mkdir -p "$DEV"

if [ "${1:-}" = "--reset" ]; then
  "$ROOT/scripts/dev-down.sh"
  rm -f "$DEV"/dev.db*
fi

listening() { (echo > "/dev/tcp/127.0.0.1/$1") >/dev/null 2>&1; }

wait_for() { # name port seconds
  for _ in $(seq 1 "$3"); do
    listening "$2" && return 0
    sleep 1
  done
  echo "Timed out waiting for $1 on :$2 - see $DEV/$1.log" >&2
  tail -20 "$DEV/$1.log" >&2 || true
  return 1
}

# Functions settings: SQLite + seed data. Generated (gitignored); an existing file is respected.
SETTINGS="$ROOT/src/Functions/local.settings.json"
if [ ! -f "$SETTINGS" ]; then
  cat > "$SETTINGS" <<JSON
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "DATABASE_PROVIDER": "Sqlite",
    "SQLITE_CONNECTION_STRING": "Data Source=$DEV/dev.db",
    "SEED_DEV_DATA": "true"
  },
  "Host": { "CORS": "*", "LocalHttpPort": $API_PORT }
}
JSON
fi

if ! listening 10000; then
  setsid nohup azurite --silent --location "$DEV/azurite" --skipApiVersionCheck > "$DEV/azurite.log" 2>&1 < /dev/null &
  echo $! > "$DEV/azurite.pid"
  wait_for azurite 10000 30
fi

if ! listening "$API_PORT"; then
  setsid nohup bash -c "cd '$ROOT/src/Functions' && exec func start --port $API_PORT" > "$DEV/functions.log" 2>&1 < /dev/null &
  echo $! > "$DEV/functions.pid"
  wait_for functions "$API_PORT" 180
fi

if ! listening "$CLIENT_PORT"; then
  ASPNETCORE_ENVIRONMENT=Development setsid nohup dotnet run --project "$ROOT/src/Client/DartPerformanceTracker.Client.csproj" \
    --no-launch-profile --urls "http://localhost:$CLIENT_PORT" > "$DEV/client.log" 2>&1 < /dev/null &
  echo $! > "$DEV/client.pid"
  wait_for client "$CLIENT_PORT" 180
fi

echo "API:    http://localhost:$API_PORT/api  (try /api/teams)"
echo "Client: http://localhost:$CLIENT_PORT"
