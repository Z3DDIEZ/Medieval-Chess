# Medieval Chess

**A Feudal Hierarchy Chess Variant with RPG Elements.**

Scale: Single Feature Deeply Executed
Status: Architecture Planning Phase

## Project Overview
Medieval Chess re-imagines the classic game by enforcing the social structures of the Middle Ages onto the board. Pieces are not just units; they are lords and vassals bound by loyalty, capable of defection, promotion, and performing tactical RPG-style abilities.

## Core Features
- **Feudal Loyalty System**: Chains of command where capturing a lord causes chaos among their vassals.
- **Dynamic Progression**: Pieces gain XP, level up, and unlock class-specific ability trees.
- **Asymmetric Gameplay**: King's Court vs. Queen's Court dynamics.
- **Modes**:
  - **Standard**: Instant capture calculation.
  - **Attrition**: HP-based combat (activates late-game).

## Documentation
- **[Ruleset](docs/ruleset-model.md)**: The "Bible" of the game's mechanics, loyalty logic, and ability trees.
- **[Domain Model](docs/domain-model.md)**: Detailed breakdown of Entities, Aggregates, and Value Objects (DDD).
- **[Architecture](docs/architecture-model.md)**: Technical stack (ASP.NET Core + React Three Fiber), system design, and diagrams.
- **[Security](docs/security-model.md)**: Anti-cheat measures, cryptographic move signing, and auth validation.
- **[API Contract](docs/api-contract-model.md)**: REST endpoints and SignalR event definitions.

## Roadmap
1. **Planning**: Architecture & Ruleset Definition (Completed)
2. **Core Domain**: Implementing the `Game`, `Piece`, and `Loyalty` engines (Next)
3. **API Foundation**: Setting up ASP.NET Core with MediatR
4. **Frontend Prototype**: basic 3D board with move validation

## Agent Planning
*Note: Internal agent logs and planning documents are located in `AG-docs/` (gitignored).*