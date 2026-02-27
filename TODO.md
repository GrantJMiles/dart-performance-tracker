# Implementation TODO

## Solution Structure
- [x] Create .NET 10 solution (`DartPerformanceTracker.slnx`)
- [x] `src/Shared` — class library for DTOs and domain models
- [x] `src/Server` — ASP.NET Core Web API
- [x] `src/Client` — Blazor WebAssembly UI

## Domain Model
- [x] `Season` entity (Id, Name, StartDate, EndDate)
- [x] `Team` entity (Id, Name)
- [x] `Player` entity (Id, Name, TeamId, IsActive)
- [x] `MatchType` entity (Id, Name, PlayersPerSide)
- [x] `SeasonMatchConfiguration` entity (Id, SeasonId, MatchTypeId, NumberOfMatches, OrderIndex)
- [x] `GameNight` entity (Id, SeasonId, Date, Opponent, IsHome)
- [x] `Match` entity (Id, GameNightId, MatchTypeId, LegsWon, LegsLost, Won)
- [x] `MatchPlayer` join table (MatchId, PlayerId — composite PK)
- [x] `PlayerMatchStats` entity (Id, MatchId, PlayerId, Tons, Maximums)
- [x] `ManOfTheMatch` entity (GameNightId, PlayerId — composite PK)

## Database
- [x] EF Core with SQLite
- [x] Migrations enabled (`InitialCreate` migration)
- [x] Auto-migrate on startup
- [x] Composite PK on `MatchPlayer` (MatchId, PlayerId)
- [x] Composite PK on `ManOfTheMatch` (GameNightId, PlayerId)
- [x] Index on `Match(GameNightId)`
- [x] Index on `MatchPlayer(PlayerId)`
- [x] Index on `PlayerMatchStats(PlayerId)`
- [x] Index on `GameNight(SeasonId)`
- [x] Seed `MatchType` rows: Singles (1), Pairs (2), Triples (3), Fours (4), Sixes (6)
- [x] Match structure is data-driven — no hardcoded singles/pairs assumptions

## API Endpoints
- [x] `GET  /api/teams`
- [x] `POST /api/teams`
- [x] `GET  /api/teams/{id}`
- [x] `GET  /api/players`
- [x] `POST /api/players`
- [x] `GET  /api/players/{id}`
- [x] `GET  /api/seasons`
- [x] `POST /api/seasons`
- [x] `GET  /api/gamenights/{id}`
- [x] `POST /api/gamenights`
- [x] `GET  /api/dashboard/team/{seasonId}`
- [x] `GET  /api/dashboard/player/{playerId}`

## Server Architecture
- [x] Services layer (no business logic in controllers)
- [x] `ITeamService` / `TeamService`
- [x] `IPlayerService` / `PlayerService`
- [x] `ISeasonService` / `SeasonService`
- [x] `IGameNightService` / `GameNightService`
- [x] `IDashboardService` / `DashboardService`
- [x] All DB operations async
- [x] DTOs never expose EF entities
- [x] OpenAPI document (`Microsoft.AspNetCore.OpenApi`) at `/openapi/v1.json`
- [x] Scalar interactive API UI at `/scalar/v1`
- [x] `GameNight` creation is atomic (single `SaveChangesAsync`)

## Core Queries
- [x] Team record (games won / lost / drawn, legs won / lost)
- [x] Last N game night results per season
- [ ] Player top teammates query (most played with, ordered by wins)
- [ ] "Last 5 form" strip on team dashboard

## Client Pages
- [x] `Teams.razor` — list teams, add team
- [x] `Players.razor` — list players, add player
- [x] `Seasons.razor` — list seasons, add season
- [x] `GameNightEntry.razor` — dynamic match entry driven by `SeasonMatchConfiguration`
- [x] `TeamDashboard.razor` — aggregate stats and game night results
- [x] `PlayerDashboard.razor` — win/loss, tons, maximums, man of the match count, recent matches

## Client Components
- [x] `MatchEntryComponent.razor`
- [x] `MatchTypeRenderer.razor`
- [x] `PlayerSelectorComponent.razor`
- [x] `StatsSummaryComponent.razor`
- [x] `NavMenu.razor` updated with all navigation links

## Player Dashboard — Completion Criteria
- [x] Win / loss record
- [x] Total tons and maximums
- [x] Man of the match count
- [ ] Top teammate (name, games played together, wins together)

## Authentication
- [ ] Phase 1 — optional passphrase middleware protecting POST endpoints
- [ ] Phase 2 — ASP.NET Identity with Admin / Viewer roles

## Deployment
- [x] SQLite file path configurable via connection string
- [x] Default connection string falls back to `dart_tracker.db`
- [ ] Azure App Service deployment configuration (e.g. `azure.yaml` / GitHub Actions workflow)
- [ ] Connection string injected via environment variable in production (no hardcoded default)

## Future Extensions
- [ ] ELO rating per player
- [ ] Cross-season analytics
- [ ] Export stats to CSV
- [ ] Public read-only dashboard link
- [ ] Mobile client using the same API
- [ ] Caching layer (if load requires it)
