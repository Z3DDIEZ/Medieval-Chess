# Medieval Chess CPU Engine - Architectural Decision Record

## 1. Context
We need a custom Chess AI capable of handling the unique "Medieval" extensions:
- **RPG Stats** (HP, Damage, Armor).
- **Abilities** (AP costs, Cooldowns, Effects).
- **Loyalty System** (Defection, Vassalage).
- **Asymmetric Board** (Courts).

Standard engines (Stockfish) cannot be easily adapted because they rely on static evaluations of standard chess rules. We must build a custom engine, leveraging standard techniques (Minimax/Alpha-Beta) adapted for our complex state space.

## 2. Architecture Overview

### 2.1 Core Algorithm
**Iterative Deepening Minimax with Alpha-Beta Pruning.**
- **Why?** Reliable, explainable, and tunable.
- **Depth Targets**: 
  - Easy: Depth 2 (Instant).
  - Medium: Depth 4 (~1-2 seconds).
  - Hard: Depth 6+ (3-5 seconds).

### 2.2 State Representation
The `Board` and `Piece` classes in the Domain model are "Heavy" (Entity Framework entities). The Engine requires a "Lightweight" representation for search speed.

**EngineGameState Struct:**
- `Bitboards` for standard piece locations (Fast spatial queries).
- `PieceState[]` array for RPG stats (HP, CD, AP, Loyalty).
- `GlobalState` struct (Turn, AP available, Court Control).

**Recommendation**: Use a **Mapper** to convert Domain `Game` -> Engine `EngineGameState` before search, and map back the best move.

## 3. Move Generation (Extended)
Standard chess has ~35 average moves. Medieval Chess increases this significantly due to Abilities.

**Move Categories:**
1. **Standard Moves**: Move, Capture.
2. **Ability Activations**: 
   - Treated as "Pseudo-Moves".
   - *Constraint*: Only generate "Sensible" ability moves (e.g., don't heal a full HP unit) to reduce branching factor.
3. **Vassal Commands**: Reassigning vassals (low priority search).

## 4. Evaluation Function (Heuristic)
The heart of the AI. `Score = Material + Positional + Medieval_Bonus`

### 4.1 Material Code (Modified)
Instead of `Pawn=1, Knight=3`, we use **Combat Value**:
- Driven by `CurrentHP`, `AttackDamage`, and `Armor`.
- *Formula*: `BaseValue * (CurrentHP / MaxHP)`. *A 10% HP Rook is worth less than a full HP Knight.*

### 4.2 Positional
- **Court Control**: Bonus for pieces in home court (if defensible) or enemy court (if aggressive).
- **Formation**: Bonus for "Phalanx" (Shield Wall) positioning.

### 4.3 Medieval Specifics
- **Loyalty Risk**: Huge Penalty if a piece is `Disloyal` (risk of defection).
- **AP Efficiency**: Bonus for using AP efficiently (not wasting turn with max AP).
- **Ability Potential**: Bonus if a high-impact ability (e.g., Resurrection) is off cooldown.

## 5. Implementation Roadmap

### Phase 1: The "random" but Valid Bot
- Implement `MoveGenerator` that supports all Medieval rules.
- AI picks a random valid move.
- *Goal*: Verify ruleset compliance and crash-testing.

### Phase 2: Greedy Materialist
- Implement basic Evaluation Function (HP + Piece Value).
- Depth 1 Search (Look ahead 1 ply).
- *Goal*: AI takes free captures and doesn't hang pieces foolishly.

### Phase 3: Tactical Thinker (Minimax)
- Implement Alpha-Beta Pruning.
- Tune Evaluation for Abilities (e.g., prioritizing `Sanctify` when ally is low).

### Phase 4: Personality Profiles
- **The Berserker**: High weight on Damage Dealt and Attacks.
- **The Turtle**: High weight on Armor, Formation, and King Safety.
- **The Politician**: Prioritizes Loyalty manipulation and Defection moves.

## 6. Technical Stack
- **Language**: C# (shared with Backend).
- **Location**: `MedievalChess.Engine` (New Project).
- **Interface**:
  ```csharp
  interface IMedievalEngine {
      MoveResult CalculateBestMove(Game domainGame, int timeLimitMs);
  }
  ```
