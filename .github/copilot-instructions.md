# Copilot Instructions

## Project Overview

**Dart Performance Tracker** is a web application for recording and analysing darts team match statistics. It tracks teams, players, seasons, game nights, match results, and individual player stats (tons, maximums, man of the match).

## Tech Stack

- **Runtime**: .NET 10
- **Architecture**: Blazor WebAssembly Hosted (Client + Server + Shared in one solution)
- **Database**: SQLite via EF Core (code-first, migrations enabled)
- **API documentation**: OpenAPI (`/openapi/v1.json`) + Scalar UI (`/scalar/v1`)

## Solution Structure

```
DartPerformanceTracker.slnx
src/
  Client/   # Blazor WebAssembly UI (.razor pages and components)
  Server/   # ASP.NET Core Web API (controllers, services, EF Core)
  Shared/   # DTOs and domain models shared by Client and Server
```

## Architecture Conventions

- **No business logic in controllers.** Controllers delegate to a service interface (`ITeamService`, `IPlayerService`, `ISeasonService`, `IGameNightService`, `IDashboardService`).
- **Services layer is required.** All DB access lives in `src/Server/Services/`.
- **DTOs must never expose EF entities.** Only types from `src/Shared/DTOs/` cross the API boundary.
- **All database operations must be `async`.**
- **Match structure is data-driven.** Never hardcode Singles/Pairs assumptions; derive everything from `MatchType.PlayersPerSide` and `SeasonMatchConfiguration`.
- **GameNight creation must be atomic** — a single `SaveChangesAsync` call covers `GameNight`, `Match`, `MatchPlayer`, `PlayerMatchStats`, and `ManOfTheMatch` records.
- Follow **SOLID principles** throughout.

## EF Core / Database Rules

- Use `AppDbContext` (in `src/Server/Data/`).
- Always create and apply a migration for schema changes (`dotnet ef migrations add <Name> --project src/Server`).
- Auto-migration runs on startup via `db.Database.Migrate()` in `Program.cs` — do not remove this.
- Required indexes: `Match(GameNightId)`, `MatchPlayer(PlayerId)`, `PlayerMatchStats(PlayerId)`, `GameNight(SeasonId)`.
- Seed `MatchType` rows: Singles (1), Pairs (2), Triples (3), Fours (4), Sixes (6).

## Client Conventions

- Pages live in `src/Client/Pages/`.
- Reusable components live in `src/Client/Components/`.
- The `GameNightEntry.razor` page must dynamically render match inputs driven by `SeasonMatchConfiguration` — never hardcode match counts.

## Build & Run

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the Server (also serves the Blazor Client in development)
dotnet run --project src/Server

# Add an EF Core migration
dotnet ef migrations add <MigrationName> --project src/Server
```

The app runs on `https://localhost:5001` (or the port shown in the console). The Scalar API explorer is available at `/scalar/v1` in development.

## Connection String

The SQLite connection string is read from `ConnectionStrings:DefaultConnection` in `appsettings.json`. It defaults to `Data Source=dart_tracker.db` if not set. In production, inject it via an environment variable — do not hardcode a path.
