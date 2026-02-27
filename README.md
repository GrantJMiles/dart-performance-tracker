# System Design Document
# Project: Team Match Recording & Statistics System
# Target Runtime: .NET 10
# Architecture: Blazor WebAssembly Hosted
# Database: SQLite
# Hosting Target: Azure App Service (Free Tier)

---

# 1. SYSTEM OVERVIEW

## 1.1 Purpose

Build a web-based system that:

- Manages Teams and Players
- Records Game Nights
- Supports configurable seasonal match formats
- Stores individual and match-level statistics
- Generates team and player dashboards
- Supports future authentication
- Has zero required paid infrastructure

This document is written for implementation agents. All structures are explicit.

---

# 2. ARCHITECTURE

## 2.1 Solution Structure

Blazor WebAssembly Hosted (.NET 10)

/src /Client        -> Blazor WebAssembly UI /Server        -> ASP.NET Core API /Shared        -> DTOs and Contracts /Infrastructure (optional if separated)

## 2.2 Architectural Principles

- Clean separation of UI and API
- No direct DB access from Client
- All database logic lives in Server
- EF Core as ORM
- SQLite as persistent store
- Match structure must be data-driven (NOT hardcoded)

---

# 3. DOMAIN MODEL

## 3.1 Core Entities

### Season

Id (int, PK) Name (string) StartDate (DateTime) EndDate (DateTime)

### Team

Id (int, PK) Name (string)

### Player

Id (int, PK) Name (string) TeamId (FK -> Team) IsActive (bool)

### MatchType
Defines structure (Singles, Pairs, Triples, etc.)

Id (int, PK) Name (string) PlayersPerSide (int)  // 1,2,3,4,6

### SeasonMatchConfiguration
Defines what match types exist in a season

Id (int, PK) SeasonId (FK -> Season) MatchTypeId (FK -> MatchType) NumberOfMatches (int)  // e.g. 6 singles, 3 pairs OrderIndex (int)       // display ordering

### GameNight

Id (int, PK) SeasonId (FK -> Season) Date (DateTime) Opponent (string) IsHome (bool)

### Match
Generic match container

Id (int, PK) GameNightId (FK -> GameNight) MatchTypeId (FK -> MatchType) LegsWon (int) LegsLost (int) Won (bool)

### MatchPlayer
Join table

MatchId (FK -> Match) PlayerId (FK -> Player) PRIMARY KEY (MatchId, PlayerId)

### PlayerMatchStats
Individual performance metrics per match

Id (int, PK) MatchId (FK -> Match) PlayerId (FK -> Player) Tons (int) Maximums (int)

### ManOfTheMatch

GameNightId (FK -> GameNight) PlayerId (FK -> Player) PRIMARY KEY (GameNightId, PlayerId)

---

# 4. DATABASE RULES

- No hardcoded assumptions about singles/pairs.
- Match size is determined by MatchType.PlayersPerSide.
- All aggregate statistics must be derived from Match + MatchPlayer + PlayerMatchStats.
- No reporting tables initially.
- Migrations must be enabled.

---

# 5. CORE QUERIES

## 5.1 Player Top Teammates (Generic for any team size >1)

SELECT teammate.Name, COUNT(*) AS GamesPlayed, SUM(CASE WHEN m.Won = 1 THEN 1 ELSE 0 END) AS GamesWon FROM MatchPlayer mp1 JOIN MatchPlayer mp2 ON mp1.MatchId = mp2.MatchId AND mp1.PlayerId <> mp2.PlayerId JOIN Players p1 ON p1.Id = mp1.PlayerId JOIN Players teammate ON teammate.Id = mp2.PlayerId JOIN Match m ON m.Id = mp1.MatchId JOIN MatchType mt ON mt.Id = m.MatchTypeId WHERE p1.Name = @PlayerName AND mt.PlayersPerSide > 1 GROUP BY teammate.Name ORDER BY GamesWon DESC LIMIT 3;

---

## 5.2 Team Record

SELECT SUM(CASE WHEN Won = 1 THEN 1 ELSE 0 END) AS GamesWon, SUM(CASE WHEN Won = 0 THEN 1 ELSE 0 END) AS GamesLost, SUM(LegsWon) AS TotalLegsWon, SUM(LegsLost) AS TotalLegsLost FROM Match WHERE GameNightId IN ( SELECT Id FROM GameNight WHERE SeasonId = @SeasonId );

---

## 5.3 Last 5 Form

SELECT Won FROM Match JOIN GameNight ON GameNight.Id = Match.GameNightId WHERE GameNight.SeasonId = @SeasonId ORDER BY GameNight.Date DESC LIMIT 5;

---

# 6. API DESIGN

## 6.1 Teams

GET /api/teams  
POST /api/teams  
GET /api/teams/{id}  

## 6.2 Players

GET /api/players  
POST /api/players  
GET /api/players/{id}  

## 6.3 Seasons

GET /api/seasons  
POST /api/seasons  

## 6.4 Game Nights

POST /api/gamenights  
GET /api/gamenights/{id}  

## 6.5 Dashboard

GET /api/dashboard/team/{seasonId}  
GET /api/dashboard/player/{playerId}  

---

# 7. CLIENT STRUCTURE

## 7.1 Pages

/pages Teams.razor Players.razor Seasons.razor GameNightEntry.razor TeamDashboard.razor PlayerDashboard.razor

## 7.2 Components

MatchEntryComponent.razor MatchTypeRenderer.razor PlayerSelectorComponent.razor StatsSummaryComponent.razor

GameNightEntry must dynamically render matches based on SeasonMatchConfiguration.

---

# 8. GAME NIGHT ENTRY FLOW

1. Load SeasonMatchConfiguration
2. For each configuration:
   - Render NumberOfMatches
   - Render MatchType.PlayersPerSide number of player selectors
3. Submit entire GameNight as one DTO
4. Server:
   - Create GameNight
   - Create Match records
   - Create MatchPlayer records
   - Create PlayerMatchStats records
   - Create ManOfTheMatch records

Transaction must be atomic.

---

# 9. AUTHENTICATION STRATEGY

Phase 1:
- Optional simple passphrase middleware
- Protect POST endpoints

Phase 2:
- Integrate ASP.NET Identity
- Roles:
  - Admin
  - Viewer

No DB schema changes required for future identity integration.

---

# 10. PERFORMANCE CONSIDERATIONS

- SQLite adequate for expected load.
- All joins indexed on foreign keys.
- Create indexes:
  - Match(GameNightId)
  - MatchPlayer(PlayerId)
  - PlayerMatchStats(PlayerId)
  - GameNight(SeasonId)

---

# 11. FUTURE EXTENSIONS

- ELO rating per player
- Cross-season analytics
- Export to CSV
- Public read-only dashboard
- Mobile UI client using same API
- Add caching layer if required

---

# 12. NON-FUNCTIONAL REQUIREMENTS

- Code must follow SOLID principles.
- All DB operations async.
- DTOs must not expose EF entities.
- No business logic inside controllers.
- Services layer required.
- Migrations enabled.
- Seed basic MatchTypes (Singles, Pairs, Triples, Fours, Sixes).

---

# 13. DEPLOYMENT

- Single Azure App Service deployment.
- SQLite stored in application root.
- Connection string via environment variable.
- Enable migrations on startup (if safe).

---

# 14. COMPLETION CRITERIA

System is complete when:

- Season can define match structure dynamically.
- GameNight entry adapts to season format.
- Team dashboard shows aggregate stats.
- Player dashboard shows:
  - Win/loss
  - Top teammate
  - Tons and maximums totals.
- All queries work across multiple seasons.

---

END OF SYSTEM DESIGN
