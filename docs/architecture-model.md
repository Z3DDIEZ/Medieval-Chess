# Medieval Chess - Architecture Model

## 1. System Overview
Medieval Chess is a complex, turn-based strategy game featuring feudal hierarchy mechanics, tactical RPG elements, and asynchronous multiplayer capabilities. The system is designed to handle complex state validation server-side while providing a rich, immersive 3D experience on the client.

## 2. Technology Stack

### Backend
- **Framework:** ASP.NET Core 8 Web API
- **Language:** C# 12
- **Real-time Communication:** SignalR (for live matches and instant updates)
- **Database:** PostgreSQL 16 (stored on Azure Logic Apps or similar)
- **ORM:** Entity Framework Core
- **Pattern:** CQRS with MediatR
- **Logging:** Serilog (Structured logging)

### Frontend
- **Framework:** React 18 + TypeScript
- **Build Tool:** Vite
- **3D Rendering:** Three.js (via React Three Fiber)
- **State Management:** Zustand (Client state), TanStack Query (Server state)
- **Animations:** Framer Motion (2D UI), GSAP (3D sequences)
- **Styling:** Tailwind CSS + CSS Modules

### Infrastructure & DevOps
- **Hosting:** Azure App Service (Plan: B1 or higher for WebSockets)
- **CI/CD:** GitHub Actions
- **Auth:** ASP.NET Core Identity + OAuth 2.0 (Google/GitHub)

---

## 3. High-Level Architecture

```mermaid
graph TD
    User[User Client] -->|HTTPS/WSS| Gateway[API Gateway / Load Balancer]
    
    subgraph "Presentation Layer"
        SPA[React SPA]
        ThreeJS[3D Canvas]
    end
    
    Gateway --> API[ASP.NET Core API]
    
    subgraph "Application Layer"
        API --> CommandHandlers[Command Handlers (MediatR)]
        API --> QueryHandlers[Query Handlers]
        Hub[SignalR Hub] <-->|Push Events| User
    end
    
    subgraph "Domain Layer"
        CommandHandlers --> GameAgg[Game Aggregate]
        GameAgg --> Loyalty[Loyalty Engine]
        GameAgg --> Ability[Ability Engine]
    end
    
    subgraph "Infrastructure Layer"
        CommandHandlers --> Repo[Repositories]
        QueryHandlers --> ReadModel[Read Models]
        Repo --> DB[(PostgreSQL)]
        Auth[Identity Service] --> DB
    end
```

## 4. Key Components

### 4.1 Game Engine (Domain Proper)
The core logic resides entirely within the Domain layer, isolated from HTTP or Database concerns.
- **Move Validator:** Pure C# logic determining legal moves based on Tier, Class, and Loyalty.
- **Loyalty Calculator:** dynamic graph evaluation (DFS/BFS) to determine chain of command efficiency.
- **Effect Processor:** Event-driven system to apply RPG effects (buffs, debuffs, cooldowns).

### 4.2 State Synchronization
- **Optimistic UI:** Client predicts move success and animates immediately.
- **Server Reconciliation:** If server rejects move (e.g., anti-cheat violation), client rolls back state.
- **Delta Updates:** Server sends only changed entities (e.g., "Piece X moved", "Loyalty Y changed"), not full board state each turn.

### 4.3 Asynchronous Processing
- **Turn Expiry:** Background service (HostedService) checks for expired turns (72h limit) every hour.
- **Rating Decay:** Daily job calculates ELO decay for inactive players.

---

## 5. Data Flow

### Execute Move Flow
1. **Client**: User drags knight to e5.
2. **Client**: Generates `MoveCommand` payload + RSA Signature.
3. **API**: `POST /games/{id}/moves`.
4. **Auth**: Validates JWT and Signature.
5. **Application**: `ExecuteMoveHandler` loads `Game` aggregate from DB.
6. **Domain**: `Game.MovePiece()` validates logic, updates state, triggers events (`PieceMoved`, `LoyaltyChanged`).
7. **Infra**: `GameRepository.Save()` commits to DB transactionally.
8. **SignalR**: Publishes `GameStateUpdated` event to match group.
9. **Client**: Receives update, plays animation.

---

## 6. Cross-Cutting Concerns

### Error Handling
- Global Exception Handler Middleware maps Domain Exceptions (e.g., `InvalidMoveException`) to HTTP 400 Bad Request.
- System Exceptions log 500 errors with correlation IDs.

### Scalability
- **Stateless API**: Can scale horizontally.
- **SignalR Backplane**: Use Azure SignalR Service to manage connections across multiple API instances.
- **Database**: Partitioning by `GameId` if scale demands (future proofing).
