#!/bin/bash
# Stops everything started by dev-up.sh (each service runs in its own session, so kill its process group).
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
for svc in client functions azurite; do
  pidfile="$ROOT/.dev/$svc.pid"
  if [ -f "$pidfile" ]; then
    pid="$(cat "$pidfile")"
    kill -- "-$pid" 2>/dev/null || kill "$pid" 2>/dev/null || true
    rm -f "$pidfile"
  fi
done
# Child processes (func host / dotnet worker) that survived: free the ports by listener pid.
for port in 7071 5211 10000 10001 10002; do
  for pid in $(lsof -t -iTCP:"$port" -sTCP:LISTEN 2>/dev/null | sort -u); do
    kill "$pid" 2>/dev/null || true
  done
done
exit 0
