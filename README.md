# Medieval Chess

**A Feudal Hierarchy Chess Variant with RPG Elements.**

| Attribute | Details |
| :--- | :--- |
| **Status** | 🚧 Phase 3: API Implementation (In Progress) |
| **Core Logic** | ✅ Implemented & Tested (DDD pattern) |
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

This project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles to ensure the complex game rules remain testable and isolated.

### 1. Core Domain (`src/MedievalChess.Domain`)
*   **Pure C# 12**, zero configurations dependencies.
*   **Entities**: `Game`, `Piece`, `Board` enforce invariants (e.g., "A piece cannot move if it is Disloyal").
*   **Value Objects**: `Position` (Algebraic notation), `LoyaltyValue` (0-100 state machine).
*   **Aggregates**: `Game` controls the transaction boundary for turns and moves.

### 2. Application Layer (Planned)
*   **CQRS**: MediatR pattern separating Reads (`GetGameQuery`) from Writes (`ExecuteMoveCommand`).
*   **Orchestration**: Manages side effects like saving to DB and broadcasting SignalR events.

### 3. Infrastructure & API (Planned)
*   **ASP.NET Core 9**: Trusted backend for auth and move validation.
*   **PostgreSQL**: Relational storage for game history and loyalty graphs.
*   **React + Three.js**: Frontend client (visuals only, no logic authority).

---

## 🚀 Getting Started

### Prerequisites
*   .NET 9.0 SDK
*   Node.js 20+

### Build & Test Core Domain
The core game logic is currently implemented. You can verify the behavior:

```powershell
# Clone and enter directory
git clone https://github.com/Z3DDIEZ/Medieval-Chess.git
cd Medieval-Chess

# Run Unit Tests
dotnet test
```

---

## 🗺️ Roadmap & Progress

### Phase 1: Architecture & Planning ✅
- [x] Defined [Ruleset](docs/ruleset-model.md) (Abilities, XP, Loyalty flows)
- [x] Designed [Domain Model](docs/domain-model.md) and [Security Policy](docs/security-model.md)
- [x] Configured GitHub Actions (CI & CodeQL)

### Phase 2: Core Domain Implementation ✅
- [x] Implemented `Piece` states (Loyalty/HP/XP)
- [x] Implemented `Board` setup and geometry
- [x] Implemented `Game` turn management
- [x] Unit Tests passing (100% Core Logic)

### Phase 3: API Foundation (Current Focus) 🚧
- [ ] Initialize ASP.NET Core API project
- [ ] Implement MediatR Command/Query pipelines
- [ ] Expose REST endpoints for Game creation
- [ ] Setup Persistence (In-Memory -> SQL)

### Phase 4: Frontend Prototype (Upcoming)
- [ ] Setup React + Vite + Three.js
- [ ] Render 3D Board
- [ ] Connect to API
