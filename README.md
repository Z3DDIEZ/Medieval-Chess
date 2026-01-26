# Medieval Chess

**A Feudal Hierarchy Chess Variant with RPG Elements.**

| Attribute | Details |
| :--- | :--- |
| **Status** | ✅ **Prototype Live** (Phase 4 Complete) |
| **Core Logic** | ✅ Implemented & Tested (DDD pattern) |
| **API** | ✅ Running (ASP.NET Core 9 / MediatR) |
| **Frontend** | ✅ 3D Board (React Three Fiber) |
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

### 3. Frontend Client (`client/`)
*   **React 18 + Vite**: Fast SPA framework.
*   **React Three Fiber**: Declarative 3D rendering.
*   **Zustand**: Lightweight state management.

---

## 🚀 Getting Started

### Prerequisites
*   .NET 9.0 SDK
*   Node.js 20+

### 1. Run the API (Backend)
Open a terminal in the root folder:
```powershell
dotnet run --project src/MedievalChess.Api/MedievalChess.Api.csproj
```
The API will start on `http://localhost:5267` (or similar).

### 2. Run the Game (Frontend)
Open a **new** terminal in the `client/` folder:
```powershell
cd client
npm run dev
```
Open the URL shown (e.g., `http://localhost:5173`). The game will automatically create a new match and load the board.

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

### Phase 4: Frontend Prototype ✅
- [x] React + Vite + Three.js Setup
- [x] 3D Board & Piece Rendering
- [x] Full Stack Integration (Auto-Game Creation)

### Upcoming Features
- [ ] Move Interaction (Drag & Drop)
- [ ] SignalR Real-time Updates
- [ ] Feudal Loyalty Visuals
