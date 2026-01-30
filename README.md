# Medieval Chess

**A Feudal Hierarchy Chess Variant with RPG Elements.**

| Attribute | Details |
| :--- | :--- |
| **Status** | 🚧 Phase 15: Frontend Integration (In Progress) |
| **Core Logic** | ✅ Standard + Medieval + AI Engines (Minimax/Greedy) |
| **API** | ✅ ASP.NET Core 9 / SignalR / AI Integration |
| **Frontend** | ✅ 3D/2D Hybrid Board + Rich Narrative Log + AI Visualization |
| **License** | Custom (Educational Use w/ Attribution) |
| **Docs** | [Architecture](docs/architecture-model.md) • [Ruleset](docs/ruleset-model.md) • [Security](docs/security-model.md) |

## ⚔️ Project Overview
Medieval Chess re-imagines the classic game by enforcing the social structures of the Middle Ages onto the board. Pieces are not just units; they are lords and vassals bound by loyalty, capable of defection, promotion, and performing tactical RPG-style abilities.

### Key Features
- **Feudal Loyalty System**: Dynamic chains of command. Capturing a Lord (Queen/Bishop/Rook) causes their Vassals to waver or defect.
- **RPG Progression**: Pieces gain XP, level up, and unlock ability trees (e.g., Knights learn "Charge", Bishops learn "Sanctify").
- **Asymmetric War**: "King's Court" (Defensive) vs "Queen's Court" (Offensive) bonuses.
- **AI Opponents**: Integrated Minimax Bot with "Thinking" visualization.
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
*   **Dependencies**: References `MedievalChess.Engine` for AI logic.

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
**Current Status:** Phase 15 (Frontend Integration).

For the full detailed project roadmap and phase tracking, please see [Roadmap](docs/roadmap.md).

---

## 📦 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Games` | Create a new game |
| GET | `/api/Games/{id}` | Get game state (pieces, moves, check status) |
| POST | `/api/Games/{id}/moves` | Execute a move (with optional promotion piece) |
| POST | `/api/Games/{id}/ai-move` | Request AI to calculate and execute a move |
| GET | `/api/Games/{id}/legal-moves/{from}` | Get legal destinations for a piece |
| POST | `/api/Games/{id}/resign` | Resign the game |
| POST | `/api/Games/{id}/offer-draw` | Offer a draw |
