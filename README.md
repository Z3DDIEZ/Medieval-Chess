# Medieval Chess

**A Feudal Hierarchy Chess Variant with RPG Elements.**

| Attribute | Details |
| :--- | :--- |
| **Status** | 🚧 Phase 12: Medieval Logic (In Progress) |
| **Core Logic** | ✅ Standard Chess + Medieval Foundations (27 tests) |
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
*   **Pure C# 12**, zero external dependencies.
*   **Entities**: `Game`, `Piece`, `Board`, `LoyaltyRelationship`, `ActiveEffect`, `PieceAbility`.
*   **Value Objects**: `Position`, `LoyaltyValue`.
*   **Services**: `LoyaltyManager`, `AbilityManager`, `EngineService`.

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

### Phase 5: Movement & Interaction ✅
- [x] Drag & Drop / Click-to-Move Interface
- [x] Move Validation (Server-side & Client-side)

### Phase 6: Feudal Loyalty ✅
- [x] Loyalty Graph & Logic
- [x] Status Effects (Loyalty/HP Stats)

### Phase 7: Real-time Sync ✅
- [x] SignalR Integration (Live Updates)

### Phase 8: 2D & 3D Visualization Modes ✅
- [x] In-Game Toggle (3D Immersive / 2D Tactical)
- [x] SVG-based 2D Board Implementation

### Phase 9: Standard Chess Engine Refinement ✅
- [x] Special Moves: Castling (Kingside/Queenside), En Passant, Pawn Promotion
- [x] Check/Checkmate/Stalemate Detection
- [x] Fifty-Move Rule Counter
- [x] Proper Algebraic Notation (PGN-style)
- [x] Promotion Picker UI (Queen/Rook/Bishop/Knight)

### Phase 10: Frontend Polish ✅
- [x] Piece Info Panel (HP, Loyalty, Value, Description)
- [x] Legal Move Highlighting (Dots for empty squares, Rings for captures)
- [x] Check Indicator (Red highlight on King)
- [x] Last Move Highlighting (Yellow squares)
- [x] Fixed Layout Issues (No unwanted scrolling/gestures)
- [x] Unit Tests for Special Moves (Castling, En Passant, Fool's Mate)

### Phase 11: 3D Camera & Board Controls ✅
- [x] Turn-Based Camera Rotation (Smooth lateral rotation on turn change)
- [x] Auto-Rotate Toggle (Enable/disable automatic camera rotation)
- [x] Free Camera Mode (Manual orbit controls in 3D mode)
- [x] Board Flip Toggle (Switch between White/Black perspective in 2D mode)
- [x] Coordinate Labels (File a-h, Rank 1-8 on board edges)
- [x] Mode-Specific Controls (3D: camera toggles, 2D: flip toggle)

### Phase 12: Medieval Logic Domain Layer 🚧
- [x] Loyalty Relationship Entity & Manager
- [x] Action Point (AP) System (5 AP/turn, 10 max)
- [x] Ability & Effect Entities
- [x] AbilityManager (Cooldowns, Effect Ticks)
- [x] Unit Tests (LoyaltyManagerTests, AbilityManagerTests)
- [ ] Ability Definitions & Activation
- [ ] Court System (King's vs Queen's Court)
- [ ] Defection Logic

---

## 📦 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Games` | Create a new game |
| GET | `/api/Games/{id}` | Get game state (pieces, moves, check status) |
| POST | `/api/Games/{id}/moves` | Execute a move (with optional promotion piece) |
| GET | `/api/Games/{id}/legal-moves/{from}` | Get legal destinations for a piece |
| POST | `/api/Games/{id}/resign` | Resign the game |
| POST | `/api/Games/{id}/offer-draw` | Offer a draw |
