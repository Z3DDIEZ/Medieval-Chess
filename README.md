# Medieval Chess

**A Feudal Hierarchy Chess Variant with RPG Elements.**

| Attribute | Details |
| :--- | :--- |
| **Status** | 🚧 Phase 4: Frontend Prototype (In Progress) |
| **Core Logic** | ✅ Implemented & Tested (DDD pattern) |
| **API** | ✅ Running (ASP.NET Core 9 / MediatR) |
| **Tech Stack** | .NET 9, React 18, PostgreSQL, SignalR |
| **Docs** | [Architecture](docs/architecture-model.md) • [Ruleset](docs/ruleset-model.md) • [Security](docs/security-model.md) |

## ⚔️ Project Overview
Medieval Chess re-imagines the classic game by enforcing the social structures of the Middle Ages onto the board. Pieces are not just units; they are lords and vassals bound by loyalty, capable of defection, promotion, and performing tactical RPG-style abilities.

### Key Features
- **Feudal Loyalty System**: Dynamic chains of command. Capturing a Lord (Queen/Bishop/Rook) causes their Vassals to waver or defect.
- **RPG Progression**: Pieces gain XP, level up, and unlock ability trees (e.g., Knights learn "Charge", Bishops learn "Sanctify").
- **Asymmetric War**: "King's Court" (Defensive) vs "Queen's Court" (Offensive) bonuses.
- **Game Modes**:
  - **Standard**: Instant capture, focus on positioning.
  - **Attrition**: HP-based combat (activates late-game).

---

## 🏗️ Technical Architecture

This project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles.

### 1. Core Domain (`src/MedievalChess.Domain`)
*   **Pure C# 12**, zero configurations dependencies.
*   **Entities**: `Game`, `Piece`, `Board`.
*   **Value Objects**: `Position`, `LoyaltyValue`.

### 2. Application & API (`src/MedievalChess.Application`, `src/MedievalChess.Api`)
*   **CQRS**: MediatR pattern (`CreateGameCommand`, `GetGameQuery`).
*   **REST API**: Exposes game state management.
*   **Infrastructure**: In-Memory persistence (Repository Pattern).

---

## 🚀 Getting Started

### Prerequisites
*   .NET 9.0 SDK
*   Node.js 20+

### 1. Run the API
The backend serves the game logic and state.

```powershell
dotnet run --project src/MedievalChess.Api/MedievalChess.Api.csproj
```
*   **Swagger UI**: `http://localhost:<port>/swagger`
*   **Test Game ID**: `11111111-1111-1111-1111-111111111111` (Seeded automatically)

### 2. Run Domain Tests
Verify the rules engine logic:

```powershell
dotnet test
```

---

## 🗺️ Roadmap & Progress

### Phase 1: Architecture & Planning ✅
- [x] Defined Ruleset & Domain Model
- [x] Configured GitHub Actions (CI & CodeQL)

### Phase 2: Core Domain Implementation ✅
- [x] Implemented Entites (Piece, Board, Game)
- [x] Unit Tests passing (100% Core Logic)

### Phase 3: API Foundation ✅
- [x] ASP.NET Core API with MediatR
- [x] REST Endpoints (POST /games, GET /games/{id})
- [x] Verification (Swagger + Curl)

### Phase 4: Frontend Prototype (Current Focus) 🚧
- [ ] Initialize React + Vite project
- [ ] Setup Three.js (React Three Fiber)
- [ ] Render 3D Board
- [ ] Fetch Game State from API
