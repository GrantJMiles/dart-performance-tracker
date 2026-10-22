#!/bin/bash
# Installs everything needed to run, test and screenshot the app in a Claude Code cloud container.
# Idempotent: skips whatever is already installed. SQL Server is not used there - the app runs on SQLite
# (DATABASE_PROVIDER=Sqlite), see scripts/dev-up.sh.
set -euo pipefail

export DEBIAN_FRONTEND=noninteractive
SUDO=""; [ "$(id -u)" -ne 0 ] && SUDO="sudo"

need_apt=()
command -v dotnet >/dev/null || need_apt+=(dotnet-sdk-10.0)
command -v func >/dev/null || need_apt+=(azure-functions-core-tools-4)

if [ ${#need_apt[@]} -gt 0 ]; then
  # Core Tools is only published in Microsoft's apt repo (its npm package downloads from a blocked CDN).
  if [[ " ${need_apt[*]} " == *" azure-functions-core-tools-4 "* ]] && [ ! -f /etc/apt/sources.list.d/microsoft-prod.list ]; then
    . /etc/os-release
    curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | $SUDO gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/$VERSION_ID/prod $VERSION_CODENAME main" \
      | $SUDO tee /etc/apt/sources.list.d/microsoft-prod.list >/dev/null
  fi
  $SUDO apt-get update -qq
  $SUDO apt-get install -y -qq "${need_apt[@]}"
fi

command -v azurite >/dev/null || npm install -g azurite

export FUNCTIONS_CORE_TOOLS_TELEMETRY_OPTOUT=1 DOTNET_CLI_TELEMETRY_OPTOUT=1
cd "$(dirname "$0")/.."
dotnet restore DartPerformanceTracker.slnx

echo "Setup complete: dotnet $(dotnet --version), func $(func --version), azurite $(azurite --version)"
