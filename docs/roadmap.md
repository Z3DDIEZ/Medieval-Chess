# Medieval Chess Roadmap

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

### Phase 12: Medieval Logic Domain Layer ✅
- [x] Loyalty Relationship Entity & Manager
- [x] Action Point (AP) System (5 AP/turn, 10 max)
- [x] Ability & Effect Entities
- [x] AbilityManager (Cooldowns, Effect Ticks)
- [x] Unit Tests (LoyaltyManagerTests, AbilityManagerTests)
- [x] Ability Definitions & Activation
- [x] Court System (King's vs Queen's Court)
- [x] Defection Logic

### Phase 13: Frontend Piece Details & 3D Enhancements ✅
- [x] API: Medieval piece data (XP, Level, Abilities, Court, Defection)
- [x] PieceInfoPanel: Court, Abilities, Defection warnings
- [x] 2D Board: Loyalty borders, Level badges, Defection icons
- [x] 3D GLTF Models: Low Poly Chess Pieces (CC BY 4.0, Steva_)
- [x] 3D Scene: Tournament-style lighting, Board frame, Dark environment
- [x] Project License: Custom (Educational Use w/ Attribution)
- [x] Attribution file for third-party assets

### Phase 14: Ruleset Implementation 🚧
- [ ] Deep analysis of `docs/ruleset-model.md`
- [ ] Implement remaining Medieval mechanics
