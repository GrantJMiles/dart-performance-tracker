---
name: run
description: Run, seed and screenshot the Dart Performance Tracker app to see a change working. Use when asked to run/start/screenshot the app or to verify a UI or API change in the real app.
---

# Run the app

The stack runs on SQLite in the sandbox (no SQL Server): Azurite + Azure Functions API on :7071 + Blazor WASM client on :5211.
The session-start hook installs the tooling (`scripts/setup-cloud.sh`); if `dotnet`/`func` are missing, run that script.

```bash
scripts/dev-up.sh            # start everything (idempotent, ~20s; logs in .dev/*.log)
scripts/dev-up.sh --reset    # wipe the SQLite db and re-seed
scripts/dev-down.sh          # stop everything
```

- Seed data comes from `src/Functions/Services/DevDataSeeder.cs` (team "The Oche Ops", 6 players, season 2025/26 id 1,
  5 completed game nights and 1 in progress). It only runs on an empty SQLite db when `SEED_DEV_DATA=true`
  (set in the generated `src/Functions/local.settings.json`). Change the seeder, then `--reset`.
- After editing Functions/Data/Shared code, restart with `scripts/dev-down.sh && scripts/dev-up.sh`.
  The client (`dotnet run`) is not hot-reloading under the script either, so restart it too after Client edits.
- Check the API: `curl localhost:7071/api/teams`, `/api/dashboard/team/1`, `/api/dashboard/player/1`.

# Screenshots

```bash
node scripts/screenshot.mjs                       # home, team + player dashboards, match night, config -> screenshots/*.png
node scripts/screenshot.mjs /config               # only specific routes
node scripts/screenshot.mjs --mobile /matchday    # 390x844 viewport, files get a -mobile suffix
```

Open the PNGs with the Read tool to look at them. `screenshots/` and `.dev/` are gitignored.
The script navigates client-side (the local index.html has no `<base>` tag, so reloading a deep link 404s - that is expected locally).

# Tests / build

```bash
dotnet build DartPerformanceTracker.slnx
```

There is no test project yet; verify behaviour through the API and screenshots.

# Notes

- Production uses SQL Server migrations (`src/Data/Migrations`); SQLite uses `EnsureCreated` from the model, so a model change
  shows up locally immediately but still needs a real `dotnet ef migrations add` for production.
- The client reads `ApiBaseUrl` from `src/Client/wwwroot/appsettings.Development.json` (http://localhost:7071/api).
