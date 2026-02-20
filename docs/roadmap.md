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

### Phase 12: The Loyalty System ✅

- [x] Scaffold Loyalty Relationship Entity & Manager
- [x] Initial Game.EndTurn hook
- [x] Wire end-of-turn processing to fully apply LoyaltyManager defection mechanics
- [x] Process defection state transitions (Lord -> Vassal chaining)

### Phase 13: The Progression System ✅

- [x] Action Point (AP) System scaffolding
- [x] Ability & Effect Entities
- [x] XP collection mechanisms
- [x] Execute capability for pieces to spend collected XP
- [x] Unlock and activate unique Ability Trees

### Phase 14: AI Development ✅

- [x] Develop Minimax Engine with Alpha-Beta Pruning
- [x] Apply Minimax AI algorithm in the Engine for solo-play
- [x] Implement MedievalEvaluator (Court/Loyalty Heuristics)

### Phase 15: Frontend Integration & Narrative ✅

- [x] PieceInfoPanel: Court, Abilities, Defection warnings
- [x] 3D Models & Scene refinements
- [x] Connect Narrative System to UI (Battle Log Component)
- [x] Visualize AI Thinking and Attrition Mode UI feedback
