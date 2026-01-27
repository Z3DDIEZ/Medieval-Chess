# Medieval Chess - Domain Modeling

## 1. Overview

This document defines the core domain model for Medieval Chess using Domain-Driven Design principles. The model separates business logic from infrastructure concerns and ensures game rules are enforced at the domain layer.

### Key Design Principles

1. **Aggregates enforce invariants** - Game aggregate controls all state changes
2. **Value objects are immutable** - Position, LoyaltyValue, AbilityState cannot change after creation
3. **Domain events capture what happened** - Events are facts, never commands
4. **Rich domain models** - Entities contain behavior, not just data
5. **Encapsulation** - Private setters, expose behavior through methods

---

## 2. Aggregate Roots

### 2.1 Game Aggregate

The Game is the primary aggregate root, controlling all gameplay state and enforcing invariants.

**Identity & Ownership:**
- `Id` (Guid): Primary key
- `GameCode` (string): Human-readable identifier (e.g., "MCH-A7K2")
- `WhitePlayerId` (Guid): Foreign key to Player
- `BlackPlayerId` (Guid): Foreign key to Player
- `CurrentPlayer` (PlayerColor enum): White or Black

**State Management:**
- `Status` (GameStatus enum): InProgress, Completed, Forfeited, Abandoned
- `TurnNumber` (int): Current turn counter
- `TurnStartedAt` (DateTime): When current turn began
- `TurnDeadline` (DateTime?): When turn expires (72 hours)
- `CreatedAt` (DateTime): Game creation timestamp
- `CompletedAt` (DateTime?): Game end timestamp

**Action Points:**
- `WhiteAP` (int): White player's current AP (0-10)
- `BlackAP` (int): Black player's current AP (0-10)
- Constants: `MaxStoredAP = 10`, `APPerTurn = 5`

**Game Mode Flags:**
- `IsStressState` (bool): Whether stress conditions are active
- `IsAttritionMode` (bool): Whether HP/damage combat is active
- `TotalWhiteLevel` (int): Sum of all white piece levels
- `TotalBlackLevel` (int): Sum of all black piece levels

**Court Control:**
- `KingsCourtControl` (CourtControl enum): WhiteControlled, BlackControlled, Neutral
- `QueensCourtControl` (CourtControl enum)
- `KingsCourtContestedTurns` (int): Consecutive turns court has been contested
- `QueensCourtContestedTurns` (int)

**Win Condition Tracking:**
- `WhiteCourtDominationTurns` (int): Turns white has controlled both courts
- `BlackCourtDominationTurns` (int)
- `WhiteDefectedPieceCount` (int): Enemy pieces that defected to white
- `BlackDefectedPieceCount` (int)

**Navigation Properties:**
- `Board` (Board entity): 1-to-1 relationship
- `PlayedMoves` (IReadOnlyCollection<Move>): All moves made
- `DrawOfferedBy` (PlayerColor?): For draw negotiation
- `DomainEvents` (ICollection<DomainEvent>): Uncommitted events

**Core Behaviors:**

1. **ExecuteMove(Position from, Position to, IEngineService engine) → void**
   - Validates turn ownership and AP availability
   - Delegates move legality to Piece entity
   - Executes move on Board
   - Handles combat resolution (instant or attrition)
   - Awards XP and updates loyalty
   - Evaluates stress state and attrition mode
   - Publishes PieceMovedEvent

2. **ActivateAbility(AbilityCommand) → Result<AbilityOutcome>**
   - Validates piece ownership and ability unlock status
   - Checks cooldown state and AP cost
   - Delegates target validation to Ability
   - Executes ability effect via event-driven handlers
   - Triggers cooldown and awards XP
   - Publishes AbilityActivatedEvent

3. **EndTurn(IEngineService engine) → void**
   - Applies passive healing to eligible pieces
   - Advances all cooldowns by 1 turn
   - Ticks active effects (damage over time, buffs)
   - Updates loyalty values for all pieces
   - Checks for insurrection event
   - Updates court control status
   - Evaluates win conditions
   - Switches current player and grants AP
   - Publishes TurnEndedEvent

4. **ReassignVassal(vassalId, newLordId, playerId) → Result**
   - Validates ownership and lord eligibility
   - Costs 1 AP
   - Updates vassal's LordId in LoyaltyRelationships table
   - Publishes VassalReassignedEvent

5. **HandleLordDefeat(Piece defeatedLord)**
   - Queries all vassals via LoyaltyRelationships table
   - Reduces vassal loyalty by 30 points
   - Marks vassals as orphaned (grace period of 1 turn)
   - Publishes LordDefeatedEvent with list of orphaned vassals

6. **CheckWinConditions() → WinCheckResult**
   - Checkmate: Board.IsCheckmate()
   - Attrition: King HP = 1 AND Queen captured
   - Loyalty: 10+ enemy pieces defected
   - Domination: Control both courts for 10 turns

**Invariants Enforced:**
- Only current player can execute moves/abilities
- AP cannot go negative
- Pieces can only move if alive and not in violated state
- Turn deadline enforced (auto-forfeit after 3 missed turns)
- Attrition mode cannot activate before turn 21 unless level threshold met

---

### 2.2 Player Aggregate

**Identity:**
- `Id` (Guid): Primary key
- `Username` (string): Unique, 3-20 characters
- `Email` (string): Unique, validated
- `PasswordHash` (string): bcrypt hashed (if not OAuth)

**Authentication:**
- `OAuthProvider` (string?): "Google", "GitHub", null if password
- `OAuthId` (string?): External provider ID
- `PublicKey` (string): RSA public key (PEM format) for move verification
- `PrivateKeyEncrypted` (string): RSA private key, encrypted with user password/derived key
- `RefreshToken` (string?): JWT refresh token
- `RefreshTokenExpiry` (DateTime?)

**Rating & Statistics:**
- `EloRating` (int): Current rating (starts at 1200)
- `GamesPlayed` (int)
- `Wins` (int)
- `Losses` (int)
- `Draws` (int)
- `PeakRating` (int): Highest ELO ever achieved
- `LastActiveAt` (DateTime): For rating decay calculations

**Anti-Smurf Tracking:**
- `IsFlagged` (bool): Triggered if win rate >80% in first 20 games
- `ConsecutiveWins` (int): For streak detection
- `PlacementMatchesWinRate` (double): Win rate in games 1-30

**Core Behaviors:**

1. **CreateWithPassword(username, email, password) → Player**
   - Generates RSA key pair (2048-bit)
   - Encrypts private key using password-derived key (PBKDF2)
   - Stores public key in plaintext
   - Sets initial ELO to 1200

2. **CreateWithOAuth(username, email, provider, oauthId) → Player**
   - Generates key pair with random password
   - Links OAuth provider ID

3. **UpdateElo(opponentElo, GameResult) → void**
   - Calculates K-factor:
     - Games 1-30: K=40 (placement)
     - Games 31-100: K=20
     - Games 100+: K=10
     - If flagged and games <50: K×2
   - Applies ELO formula: `newElo = oldElo + K × (actualScore - expectedScore)`
   - Updates win/loss/draw counters
   - Checks for smurf flag (>80% win rate in first 20 games)
   - Recalibrates ELO at game 50 if flagged and win rate >75% (+300 ELO)

4. **ApplyRatingDecay() → void**
   - 7-29 days inactive: -5 ELO/day
   - 30+ days inactive: -10 ELO/day
   - Max decay: -200 ELO
   - Runs via scheduled job, not on-demand

5. **SignMove(moveData) → string**
   - Decrypts private key using stored password/session key
   - Signs move JSON with RSA-SHA256
   - Returns base64 signature

6. **VerifyMoveSignature(moveData, signature) → bool**
   - Uses public key to verify signature
   - Returns true if signature valid

**Invariants:**
- Username cannot change after creation
- Public key immutable after generation
- ELO cannot drop below 0
- Rating decay caps at -200

---

### 2.3 Board Entity

**Ownership:**
- `Id` (Guid): Primary key
- `GameId` (Guid): Foreign key to Game (1-to-1)

**State:**
- No explicit board array stored—pieces contain their positions
- Board logic queries Pieces collection dynamically

**Navigation Properties:**
- `Pieces` (IReadOnlyCollection<Piece>): All pieces on board (alive and captured)
- Captured pieces have `IsCaptured = true` and `Position = null`
- `EnPassantTarget` (Position?): Target square for en passant capture
- `HalfMoveClock` (int): For 50-move rule
- `CastlingRights` (bools): WhiteKingsideCastle, etc.

**Core Behaviors:**

1. **CreateStandardSetup() → Board**
   - Instantiates 32 pieces in standard chess positions
   - Assigns initial loyalty relationships (hard-coded)
   - Queen is King's direct vassal
   - Bishops are Queen's vassals
   - Knights/Rooks are Bishop vassals
   - Pawns distributed among Bishops

2. **GetPieceAt(Position) → Piece?**
   - Queries `Pieces.Where(p => p.Position == position && !p.IsCaptured)`

3. **GetPieceById(Guid) → Piece?**
   - Direct lookup by ID

4. **MovePiece(from, to) → Result<MoveResult>**
   - Updates piece's Position property
   - If destination occupied, returns CapturedPiece in result
   - Does NOT handle loyalty/combat—delegates to Game aggregate

5. **IsKingInCheck(PlayerColor) → bool**
   - Gets king position
   - Queries all opponent pieces
   - Checks if any can legally attack king's position

6. **IsCheckmate(PlayerColor) → bool**
   - Validates IsKingInCheck() first
   - Iterates all legal moves for player
   - Returns true if no move escapes check

7. **GetVassalsOfLord(lordId) → List<Piece>**
   - Queries LoyaltyRelationships table
   - Returns all pieces with `LordId = lordId`

8. **InitializeAllPieceHP()**
   - Called when attrition mode activates (turn 21 or level 50 threshold)
   - Sets each piece's `CurrentHP = MaxHP` based on type

9. **CountPiecesInCourt(PlayerColor, CourtSide) → int**
   - CourtSide.KingsCourt = files a-d
   - CourtSide.QueensCourt = files e-h
   - Counts pieces matching color and file range

**Invariants:**
- Only one piece per square (enforced in MovePiece)
- King cannot be captured (HP minimum = 1 in attrition mode)
- Captured pieces cannot move

---

## 3. Entities

### 3.1 Piece Entity

**Identity:**
- `Id` (Guid): Primary key
- `BoardId` (Guid): Foreign key to Board

**Classification:**
- `Type` (PieceType enum): Pawn, Knight, Bishop, Rook, Queen, King, Peasant, Farrier, Blacksmith, Medic
- `Color` (PlayerColor enum): White, Black
- `Position` (Position value object?): Nullable, null when captured

**Progression:**
- `Level` (int): Current level (0-based)
- `XP` (int): Current experience points
- `PromotionTier` (string): E.g., "Footman", "Knight-Errant", "Priest"
- `PieceValue` (int): For combat calculations (Pawn=1, Knight=3, Bishop=3, Rook=5, Queen=9)

**Combat (Attrition Mode):**
- `MaxHP` (int): Base HP determined by type
- `CurrentHP` (int): Current health
- `Armor` (int): Damage reduction stat
- `IsCaptured` (bool): Whether piece is off-board

**Loyalty:**
- Loyalty relationship stored in separate `LoyaltyRelationships` table
- Piece does NOT have direct `LordId` foreign key
- Instead, queries junction table for current lord

**Court Membership:**
- `HomeCourt` (CourtSide enum): KingsCourt or QueensCourt
- Determines which court bonuses apply

**State Flags:**
- `IsLord` (bool): Computed property—true if any other piece lists this as lord
- `IsOrphaned` (bool): Temporarily true after lord captured (1 turn grace period)
- `OrphanedSince` (int?): Turn number when orphaned

**Navigation Properties:**
- `UnlockedAbilities` (ICollection<PieceAbility>): Many-to-many via junction table
- `ActiveEffects` (ICollection<ActiveEffect>): Temporary buffs/debuffs

**Core Behaviors:**

1. **CanMoveTo(target, Board) → bool**
   - Validates move based on Type and PromotionTier
   - Base Pawn: 1 forward, 2 on first move
   - Promoted Footman: +1 forward
   - Knight: L-shape, modified by Farrier support
   - Returns false if loyalty state is Disloyal and random roll fails

2. **CalculateDamage(defender) → int**
   - Formula: `BaseDamage = PieceValue × 2`
   - Modify by level: `× (Level / 10 + 1)`
   - Apply RNG: `× Random(0.8, 1.2)`
   - Critical hit (15% chance): `× 1.5`
   - Returns final damage before armor reduction

3. **TakeDamage(amount) → void**
   - Applies armor reduction: `finalDamage = amount - (Armor × Random(0.5, 1.0))`
   - `CurrentHP -= finalDamage`
   - If `CurrentHP <= 0`: mark `IsCaptured = true`

4. **GainXP(amount, XPSource) → void**
   - Adds XP
   - Checks for level-up thresholds (varies by type)
   - If level-up: triggers promotion, unlocks abilities
   - Publishes PieceLeveledUpEvent

5. **OnLordDefeated() → void**
   - Reduces loyalty by 30 (updates LoyaltyRelationships entry)
   - Sets `IsOrphaned = true`
   - Sets `OrphanedSince = currentTurn`

6. **AssignToLord(newLordId) → void**
   - Updates LoyaltyRelationships table
   - Clears `IsOrphaned` flag
   - Resets loyalty penalties

7. **ApplyPassiveHealing() → void**
   - Called during end-turn phase
   - Conditions: piece didn't move this turn, no enemies within 2 squares, at least 1 ally adjacent
   - Heals `10% of MaxHP`

8. **CanAcceptVassals() → bool**
   - True for: Queen, Bishops, Rooks (at Fortress level), Knights (at Banneret level)

**Invariants:**
- Position must be within 0-7 rank/file bounds when not captured
- CurrentHP cannot exceed MaxHP
- Level cannot decrease
- Captured pieces have Position = null

---

### 3.2 Move Entity (Abstract Base)

**Polymorphic Table-Per-Type (TPT) Design:**
- Base `Moves` table with shared columns
- Inherited tables: `StandardMoves`, `AbilityMoves`, `DefectionMoves`, `VassalReassignments`

**Base Properties:**
- `Id` (Guid): Primary key
- `GameId` (Guid): Foreign key
- `PlayerId` (Guid): Who executed the move
- `TurnNumber` (int): When move occurred
- `Timestamp` (DateTime): Precise execution time
- `APCost` (int): How much AP was spent
- `MoveType` (MoveType enum): Standard, Ability, Defection, Reassignment

**Cryptographic Audit:**
- `MoveDataJson` (string): Serialized move details (from, to, piece ID, etc.)
- `Signature` (string): RSA signature from player's private key
- `HashChainValue` (string): SHA-256 hash of (previousHash + MoveDataJson + Timestamp)

**Derived Entities:**

**StandardMove:**
- `PieceId` (Guid): Which piece moved
- `FromPosition` (Position value object)
- `ToPosition` (Position value object)
- `CapturedPieceId` (Guid?): If piece was captured
- `DamageDealt` (int?): If attrition mode

**AbilityMove:**
- `PieceId` (Guid): Which piece used ability
- `AbilityId` (Guid): Which ability
- `TargetPositions` (string): JSON array of target squares
- `EffectResults` (string): JSON describing what happened

**DefectionMove:**
- `PieceId` (Guid): Which piece defected
- `FromPlayerId` (Guid): Original owner
- `ToPlayerId` (Guid): New owner
- `Reason` (string): "LordDefeated", "LoyaltyThreshold"
- `DefectionOutcome` (string): "+1 stats", "lost ability", etc.

**VassalReassignment:**
- `VassalId` (Guid)
- `OldLordId` (Guid?)
- `NewLordId` (Guid)

**Core Behaviors:**

1. **ValidateSignature() → bool**
   - Fetches Player.PublicKey
   - Verifies Signature against MoveDataJson
   - Returns false if tampered

2. **ValidateHashChain(previousMove) → bool**
   - Recomputes hash: `SHA256(previousMove.HashChainValue + MoveDataJson + Timestamp)`
   - Compares to stored HashChainValue
   - Returns false if chain broken

---

### 3.3 LoyaltyRelationship Entity (Junction Table)

**Purpose:**
- Models feudal hierarchy dynamically
- Allows pieces to change lords without modifying Piece entity

**Properties:**
- `Id` (Guid): Primary key
- `VassalId` (Guid): Foreign key to Piece (the subordinate)
- `LordId` (Guid): Foreign key to Piece (the superior)
- `LoyaltyValue` (int): 0-100 scale
- `EstablishedAt` (DateTime): When relationship formed
- `LastModifiedAt` (DateTime): When loyalty last changed

**Loyalty States (derived from LoyaltyValue):**
- 90-100: Devoted (+10% XP, immune to defection)
- 70-89: Loyal (standard)
- 50-69: Wavering (-1 movement, +1 AP cost for abilities)
- 30-49: Disloyal (20% refuse orders, claimable by opponent)
- 0-29: Defecting (transfers at turn end)

**Core Behaviors:**

1. **AdjustLoyalty(delta) → void**
   - `LoyaltyValue = Clamp(LoyaltyValue + delta, 0, 100)`
   - Updates `LastModifiedAt`

2. **CheckDefectionThreshold() → bool**
   - Returns true if `LoyaltyValue < 30`

3. **GetLoyaltyState() → LoyaltyState**
   - Maps value to enum

**Triggers for Loyalty Changes:**
- Lord captured: -30
- Adjacent to lord for full turn: +5
- Piece captures enemy: +10
- Takes damage without allies nearby: -5
- Player uses "Sacrifice" on adjacent ally: -20 to survivor, +15 to all others

---

### 3.4 PieceAbility Entity (Many-to-Many)

**Junction Table Structure:**
- `PieceAbilities` table links Pieces to Abilities with state

**Properties:**
- `Id` (Guid): Primary key
- `PieceId` (Guid): Foreign key
- `AbilityDefinitionId` (Guid): Foreign key to AbilityDefinitions (static data)
- `UnlockedAt` (DateTime): When ability became available
- `UpgradeTier` (int): 0-3 (base → upgrade 3)
- `CurrentCooldown` (int): Turns remaining (0 = ready)
- `MaxCooldown` (int): From ability definition
- `TimesUsed` (int): For statistics

**Navigation:**
- `AbilityDefinition` (AbilityDefinition entity): Static data

**Core Behaviors:**

1. **IsReady() → bool**
   - Returns `CurrentCooldown == 0`

2. **TriggerCooldown() → void**
   - Sets `CurrentCooldown = MaxCooldown`

3. **AdvanceCooldown() → void**
   - Called each turn end: `CurrentCooldown = Math.Max(0, CurrentCooldown - 1)`

4. **GetAPCost() → int**
   - Reads from AbilityDefinition based on UpgradeTier

---

### 3.5 AbilityDefinition Entity (Static Reference Data)

**Properties:**
- `Id` (Guid): Primary key
- `Name` (string): "Charge", "Sanctify", "Rally"
- `PieceType` (PieceType enum): Which piece type uses this
- `TreePath` (string): "A" or "B" (for two-tree system)
- `BaseAPCost` (int)
- `BaseCooldown` (int)
- `EffectType` (EffectType enum): Damage, Heal, Buff, Teleport, Summon
- `EffectParameters` (string): JSON with ability-specific data

**Upgrade Tiers (1-to-many):**
- `UpgradeTiers` (ICollection<AbilityUpgradeTier>)

**AbilityUpgradeTier Sub-Entity:**
- `TierLevel` (int): 0 (base), 1, 2, 3
- `XPRequired` (int): Cost to unlock
- `ModifiedAPCost` (int?)
- `ModifiedCooldown` (int?)
- `EnhancedEffectParameters` (string): JSON

**This is seeded data, not user-created**

---

### 3.6 ActiveEffect Entity

**Purpose:**
- Tracks temporary buffs/debuffs on pieces
- Examples: Fortify (damage reduction), Sharpened Blade (+damage), Poisoned (DoT)

**Properties:**
- `Id` (Guid): Primary key
- `PieceId` (Guid): Foreign key
- `EffectType` (EffectType enum): DamageReduction, DamageBonus, MovementBonus, Healing, DamageOverTime
- `Magnitude` (int): Amount (e.g., +10 damage, 50% reduction)
- `RemainingDuration` (int): Turns left
- `AppliedAt` (DateTime)
- `SourceAbilityId` (Guid?): Which ability caused this

**Core Behaviors:**

1. **Apply(Piece) → void**
   - Modifies piece stats temporarily
   - E.g., if DamageReduction: piece calculates defense with +Magnitude

2. **Tick() → void**
   - Called each turn end
   - Reduces `RemainingDuration` by 1
   - If duration = 0: removes itself

3. **IsExpired() → bool**
   - Returns `RemainingDuration <= 0`

---

## 4. Value Objects

### 4.1 Position

**Properties:**
- `Rank` (int): 0-7 (row)
- `File` (int): 0-7 (column)

**Behaviors:**
- Immutable record type
- Equality by value (two positions with same rank/file are equal)
- `ToChessNotation() → string`: Returns "e4", "a1", etc.
- `IsValid() → bool`: Checks 0 <= Rank, File <= 7
- `DistanceTo(Position) → int`: Chebyshev distance for adjacency checks

**Usage:**
- Embedded in Move entities
- Used for Board queries

---

### 4.2 LoyaltyValue

**Properties:**
- `Value` (int): 0-100

**Derived:**
- `State` (LoyaltyState enum): Computed from Value

**Behaviors:**
- Immutable
- `WithAdjustment(delta) → LoyaltyValue`: Creates new instance with clamped value
- `IsDefecting() → bool`: Returns Value < 30
- `IsWavering() → bool`: Returns Value < 70 && Value >= 50

**Could be embedded in LoyaltyRelationship** or used as value object

---

### 4.3 MoveOutcome

**Properties:**
- `MovedPiece` (Piece): Reference to piece that moved
- `CapturedPiece` (Piece?): Null if no capture
- `CombatOccurred` (bool)
- `DamageDealt` (int): 0 if instant capture
- `DefenderHP` (int?): Remaining HP after combat
- `WasInstantCapture` (bool)
- `XPAwarded` (int)

**Usage:**
- Returned from Game.ExecuteMove()
- Used to emit domain events

---

### 4.4 AbilityOutcome

**Properties:**
- `AbilityUsed` (AbilityDefinition)
- `TargetsAffected` (List<Piece>)
- `EffectsApplied` (List<ActiveEffect>)
- `APSpent` (int)
- `Success` (bool)
- `FailureReason` (string?)

**Usage:**
- Returned from Game.ActivateAbility()

---

## 5. Domain Events

Domain events are published after state changes and handled by event handlers to trigger side effects (update loyalty, apply abilities, send notifications).

### 5.1 GameCreatedEvent

**Properties:**
- `GameId` (Guid)
- `WhitePlayerId` (Guid)
- `BlackPlayerId` (Guid)
- `CreatedAt` (DateTime)

**Handlers:**
- Send notification to both players
- Initialize matchmaking record

---

### 5.2 PieceMovedEvent

**Properties:**
- `GameId` (Guid)
- `PieceId` (Guid)
- `From` (Position)
- `To` (Position)
- `TurnNumber` (int)
- `Timestamp` (DateTime)

**Handlers:**
- Update frontend board state via SignalR
- Log to move history
- Trigger passive loyalty adjustments

---

### 5.3 PieceCapturedEvent

**Properties:**
- `GameId` (Guid)
- `AttackerId` (Guid)
- `DefenderId` (Guid)
- `TurnNumber` (int)
- `CaptureType` (CaptureType enum): Instant, Attrition

**Handlers:**
- Award XP to attacker
- Check if defender was a lord → trigger LordDefeatedEvent
- Update piece counts for win conditions

---

### 5.4 LordDefeatedEvent

**Properties:**
- `GameId` (Guid)
- `LordId` (Guid)
- `OrphanedVassalIds` (List<Guid>)
- `TurnNumber` (int)

**Handlers:**
- Reduce loyalty of all orphaned vassals by 30
- Mark vassals as orphaned
- Trigger UI animation showing loyalty drop

---

### 5.5 AbilityActivatedEvent

**Properties:**
- `GameId` (Guid)
- `PieceId` (Guid)
- `AbilityId` (Guid)
- `Targets` (List<Position>)
- `TurnNumber` (int)
- `Timestamp` (DateTime)

**Handlers:**
- Apply ability effects (event-driven)
- Update ability cooldown state
- Trigger frontend animation

---

### 5.6 DefectionTriggeredEvent

**Properties:**
- `GameId` (Guid)
- `PieceId` (Guid)
- `FromPlayerId` (Guid)
- `ToPlayerId` (Guid)
- `Reason` (string)
- `Outcome` (string): "+1 stats", "lost ability", etc.
- `TurnNumber` (int)

**Handlers:**
- Transfer piece ownership
- Apply defection outcome (random roll result)
- Increment defection counter for loyalty victory tracking
- Log to DefectionMoves table

---

### 5.7 VassalReassignedEvent

**Properties:**
- `GameId` (Guid)
- `VassalId` (Guid)
- `OldLordId` (Guid?)
- `NewLordId` (Guid)
- `TurnNumber` (int)

**Handlers:**
- Update LoyaltyRelationships table
- Clear orphaned flag
- Restore loyalty to 50 if was below

---

### 5.8 StressStateEnteredEvent

**Properties:**
- `GameId` (Guid)
- `TurnNumber` (int)
- `Trigger` (string): "KingInCheck", "QueenCaptured", "PiecesBelow50Percent"

**Handlers:**
- Transfer Queen's authority to King
- Make Bishops direct vassals of King
- Trigger UI change (darker theme, alert icons)

---

### 5.9 AttritionModeActivatedEvent

**Properties:**
- `GameId` (Guid)
- `TurnNumber` (int)
- `TriggerReason` (string): "Turn21Reached", "Level50Threshold"

**Handlers:**
- Initialize all piece HP values
- Enable HP bars in UI
- Publish announcement to both players

---

### 5.10 TurnEndedEvent

**Properties:**
- `GameId` (Guid)
- `NextPlayer` (PlayerColor)
- `TurnNumber` (int)

**Handlers:**
- Send turn notification to next player (if configured)
- Update turn timer display
- Trigger passive healing/cooldown advances

---

### 5.11 GameCompletedEvent

**Properties:**
- `GameId` (Guid)
- `Winner` (PlayerColor)
- `WinCondition` (WinCondition enum): Checkmate, Attrition, Loyalty, Domination, Forfeit
- `FinalTurn` (int)
- `CompletedAt` (DateTime)

**Handlers:**
- Calculate ELO changes for both players
- Update player statistics
- Archive game state
- Send victory/defeat notifications

---

### 5.12 PieceLeveledUpEvent

**Properties:**
- `GameId` (Guid)
- `PieceId` (Guid)
- `OldLevel` (int)
- `NewLevel` (int)
- `NewPromotionTier` (string)
- `UnlockedAbilities` (List<Guid>)

**Handlers:**
- Update piece stats (movement range, HP cap)
- Unlock new abilities in PieceAbilities table
- Trigger celebration animation

---

### 5.13 InsurrectionEvent

**Properties:**
- `GameId` (Guid)
- `AffectedPieceIds` (List<Guid>)
- `TurnNumber` (int)

**Handlers:**
- Move disloyal pieces randomly for 1 turn
- Reset loyalty to 50 after insurrection
- Display dramatic UI effect

---

### 5.14