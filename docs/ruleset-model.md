# Medieval Chess - Complete Ruleset v1.0

## Core Concept
A chess variant combining feudal hierarchy, loyalty mechanics, tactical RPG progression, and asymmetric court dynamics. Players manage not just piece position but social bonds, resource allocation, and strategic progression trees.

---

## 1. Board & Setup

### Board Structure
- Standard 8×8 chess board
- Each square can have terrain modifiers (unlocked through specific piece abilities)
- Board divided into King's Court (queenside: a-d files) and Queen's Court (kingside: e-h files)

### Initial Piece Placement
Standard chess setup with extended piece types:

**Back Rank (Rank 1/8):**
- Rook, Knight, Bishop, Queen, King, Bishop, Knight, Rook

**Second Rank (Rank 2/7):**
- 8 Pawns

**Hidden Reserve (unlocked through gameplay):**
- 2 Peasants (per side, can be recruited via specific abilities)
- 1 Farrier (per side, spawns when Knight reaches Level 3)
- 1 Blacksmith (per side, spawns when Rook reaches Level 3)
- 1 Medic (per side, spawns when Bishop reaches Level 3)

---

## 2. Feudal Hierarchy System

### Loyalty Graph Structure

**Tier System:**
1. **Monarch Tier:** King (ultimate authority)
2. **Royal Tier:** Queen (primary vassal in peace), Bishops (primary vassals in stress)
3. **Noble Tier:** Rooks, Knights
4. **Common Tier:** Pawns, Peasants
5. **Support Tier:** Farrier, Blacksmith, Medic (bound to specific nobles)

**Vassalage Rules:**
- Each piece has exactly ONE direct lord
- Lords can have unlimited vassals
- King's direct vassals (default): Queen only
- Queen's vassals: Both Bishops, King's Court Rook, King's Court Knight
- Bishop's vassals: Adjacent Knight, all Pawns in their court
- Rook's vassals: None initially (can claim through "Garrison" ability)
- Knight's vassals: None initially (can claim Peasants through "Levy" ability)

**Stress Mechanics:**
Game enters "Stress State" when:
- King is in check
- Queen is captured
- 50% or more of starting pieces are captured

During Stress State:
- Queen's authority transfers to King
- Bishops become King's direct vassals
- Loyalty recalculation occurs (see Section 2.2)

### Loyalty Scale

Each piece has a **Loyalty Value (LV)** ranging from 0-100:

- **90-100:** Devoted (immune to defection, +10% XP gain)
- **70-89:** Loyal (standard behavior)
- **50-69:** Wavering (-1 movement range, abilities cost +1 AP)
- **30-49:** Disloyal (can be claimed by opponent, 20% chance to refuse orders)
- **0-29:** Defecting (automatically transfers to opponent at turn end)

**Loyalty Modifiers:**
- Lord captured: -30 LV immediately
- Piece adjacent to lord: +5 LV per turn
- Piece captures enemy: +10 LV
- Piece takes damage without nearby allies: -5 LV per turn
- Player uses "Sacrifice" command on adjacent piece: -20 LV to survivor, +15 LV to all other allied pieces

**Morale & Insurrection:**
- If 3+ pieces reach Disloyal state simultaneously outside Stress State: **Insurrection Event**
- Insurrection: Disloyal pieces move randomly for 1 turn, then loyalty resets to 50 LV
- Insurrection cannot occur during Stress State (war unifies factions)

### Defection Mechanics

**When Lord is Captured:**
Player has 1 turn to reassign orphaned vassals to new lords. If not reassigned:
- Roll d100 for each orphaned piece
- If roll < (100 - Current LV): piece becomes neutral (gray coloring, cannot move for 2 turns)
- Neutral pieces can be claimed by either player by moving a lord-tier piece adjacent

**Defection Outcomes (Probabilistic):**
When piece defects to opponent:
- 40% chance: Gains +1 to all stats (motivated by new allegiance)
- 30% chance: Retains current stats
- 20% chance: Loses 1 random ability unlock (guilt/confusion)
- 10% chance: Gains entirely new ability from random tree (fresh start)

---

## 3. Combat System

### Attack Resolution

**Instant Capture Mode (Turns 1-20):**
- Standard chess rules: attacker captures defender immediately
- No HP, no damage calculation
- Experience still gained normally

**Attrition Mode (Turn 21+):**
Unlocked when either player reaches total Level 50 across all pieces.

Each piece gains **Hit Points (HP):**
- Pawn/Peasant: 20 HP
- Knight/Bishop: 40 HP
- Rook: 60 HP
- Queen: 80 HP
- King: 100 HP (cannot be reduced below 1 HP)

**Attack Calculation:**
```
Base Damage = Attacker's Piece Value × 2
Modified Damage = Base Damage × (Attacker Level / 10 + 1) × RNG(0.8-1.2)
Defense Reduction = Defender's Armor × RNG(0.5-1.0)
Final Damage = Modified Damage - Defense Reduction

Armor Values:
- Pawn: 2
- Knight/Bishop: 5
- Rook: 8
- Queen: 10
- King: 15
```

**Critical Hits:**
- 15% chance to deal 1.5× damage
- Chance increases by +5% if attacker is Devoted loyalty

**Capture:**
- Piece captured when HP reaches 0
- Damaged pieces remain on board with visible HP bar

### Healing Mechanics

**Passive Healing:**
- Piece heals 10% max HP per turn if:
  - Not moved during that turn AND
  - No enemy pieces within 2 squares AND
  - At least 1 allied piece adjacent

**Active Healing:**
- Medic ability: "Field Surgery" (see Section 4)
- Bishop ability: "Sanctify" heals 20 HP to adjacent allies
- Rook ability: "Fortify" grants 50% damage reduction for 3 turns

---

## 4. Experience & Progression

### Experience Gain

**XP Sources:**
- Capture enemy piece: +10 XP (+5 bonus if higher piece value)
- Survive 5 turns: +5 XP
- Defend allied piece from attack: +8 XP (must be adjacent when ally attacked)
- Deliver check: +15 XP
- Use ability successfully: +3 XP
- Lord survives turn with all vassals alive: +2 XP to all vassals

**XP Penalties:**
- Piece loses LV below 50: -5 XP
- Piece refuses order (Disloyal state): -10 XP

### Promotion Paths

**Pawn Progression:**
```
Pawn (0 XP)
  ↓ 20 XP
Footman (+1 movement, unlock Ability Slot 1)
  ↓ 40 XP
Man-at-Arms (+1 HP cap to 30, unlock Ability Slot 2)
  ↓ 70 XP
Sergeant (can move 2 squares forward, Ability Slot 3)
  ↓ 100 XP
Captain (becomes minor lord: can have 1 vassal, Ability Slot 4)
```

**Knight Progression:**
```
Knight (0 XP)
  ↓ 25 XP
Knight-Errant (unlock Farrier support)
  ↓ 50 XP
Knight-Bachelor (+1 L-shape move per turn)
  ↓ 80 XP
Knight-Banneret (can command 2 Peasants)
  ↓ 120 XP
Knight-Commander (Charge ability cooldown reduced by 1)
```

**Bishop Progression:**
```
Deacon (0 XP)
  ↓ 30 XP
Priest (unlock Sanctify ability)
  ↓ 60 XP
Bishop (+1 diagonal range)
  ↓ 90 XP
Archbishop (can move 1 square orthogonally)
  ↓ 130 XP
Patriarch (Pontifical Guard ability unlocked)
  ↓ 170 XP
Cardinal (can combine with other Cardinal)
  ↓ 200 XP + Bishop Fusion
Pope (moves unlimited diagonals + 3 squares orthogonally, both Pontifical Guards merge)
```

**Rook Progression:**
```
Rook (0 XP)
  ↓ 30 XP
Tower (unlock Blacksmith support)
  ↓ 65 XP
Castle (+1 orthogonal range)
  ↓ 100 XP
Fortress (Garrison ability: store 1 piece inside, immune to damage)
  ↓ 150 XP
Citadel (can store 2 pieces, Fortify affects all garrisoned)
```

**Queen Progression:**
```
Queen (0 XP)
  ↓ 50 XP
Queen-Consort (Rally ability cooldown -1)
  ↓ 100 XP
Queen-Regent (+1 range all directions)
  ↓ 160 XP
Queen-Paramount (can command all courts simultaneously)
```

**Support Piece Unlocks:**
- **Farrier:** Spawns when any Knight reaches Knight-Errant. Grants mounted Knight +2 movement for 1 turn (3-turn cooldown).
- **Blacksmith:** Spawns when Rook reaches Tower. Grants adjacent piece "Sharpened Blade" (+10 damage for 2 turns, 4-turn cooldown).
- **Medic:** Spawns when Bishop reaches Priest. "Field Surgery" heals 30 HP (5-turn cooldown).

---

## 5. Ability System

### Action Points (AP)

Each player has **5 AP per turn**, persistent across turns (max 10 AP stored).

**AP Costs:**
- Move piece: 1 AP
- Use Basic Ability: 2 AP
- Use Advanced Ability: 3 AP
- Use Ultimate Ability: 5 AP
- Reassign vassal: 1 AP

### Ability Trees

Each piece class has **2 base abilities** with **4 upgrade tiers each** (8 total unlocks).

#### Knight Abilities

**Tree A: Vanguard**
1. **Charge (Basic):** Move in L-shape, then 2 additional squares in cardinal direction. If ending on enemy, deal +5 damage.
2. **Lance Strike (Upgrade 1, 30 XP):** Charge ignores intervening pieces.
3. **Cavalry Momentum (Upgrade 2, 60 XP):** After Charge capture, can immediately move 1 square.
4. **Devastating Impact (Upgrade 3, 100 XP):** Charge deals +15 damage and pushes defender 1 square back.

**Tree B: Defender**
1. **Shield Wall (Basic):** Grant +5 armor to adjacent allies for 2 turns.
2. **Stalwart (Upgrade 1, 30 XP):** Shield Wall duration +1 turn.
3. **Guardian's Resolve (Upgrade 2, 60 XP):** If ally protected by Shield Wall takes damage, Knight gains +10 LV.
4. **Unyielding Bastion (Upgrade 3, 100 XP):** Shield Wall grants damage immunity for 1 turn.

#### Bishop Abilities

**Tree A: Divine**
1. **Sanctify (Basic):** Heal adjacent ally 20 HP, grant +10 LV.
2. **Holy Ground (Upgrade 1, 35 XP):** Sanctify affects all adjacent allies.
3. **Martyr's Blessing (Upgrade 2, 70 XP):** If Bishop takes damage while Sanctify active, healing doubled.
4. **Resurrection (Upgrade 3, 120 XP):** Once per game, revive captured allied piece at 50% HP on adjacent square.

**Tree B: Authority**
1. **Pontifical Guard (Basic):** Summon 2 ethereal guards (cannot move, block attacks) on adjacent squares for 3 turns.
2. **Consecrated Line (Upgrade 1, 35 XP):** Guards can be placed up to 2 squares away.
3. **Divine Judgment (Upgrade 2, 70 XP):** Enemies adjacent to guards take 5 damage per turn.
4. **Papal Decree (Upgrade 3, 120 XP):** As Pope, Pontifical Guard creates 4 guards, lasts 5 turns.

#### Rook Abilities

**Tree A: Siege**
1. **Fortify (Basic):** Reduce incoming damage by 50% for 3 turns, cannot move.
2. **Immovable Object (Upgrade 1, 35 XP):** Fortify duration +2 turns.
3. **Reactive Plating (Upgrade 2, 70 XP):** Attackers take 10 damage when hitting Fortified Rook.
4. **Siege Engine (Upgrade 3, 110 XP):** While Fortified, Rook can attack enemies within 2 squares without moving.

**Tree B: Logistics**
1. **Garrison (Basic):** Store 1 allied piece inside Rook (immune to damage, can exit next turn).
2. **Supply Line (Upgrade 1, 35 XP):** Garrisoned piece heals 20 HP per turn.
3. **Rapid Deployment (Upgrade 2, 70 XP):** Garrisoned piece can exit and immediately move.
4. **War Wagon (Upgrade 3, 110 XP):** Store 2 pieces, both can exit and attack same turn.

#### Queen Abilities

**Tree A: Command**
1. **Rally (Basic):** Reset cooldowns of all vassals within 3 squares.
2. **Inspiring Presence (Upgrade 1, 50 XP):** Rally also grants +15 LV to affected pieces.
3. **Tactical Genius (Upgrade 2, 100 XP):** Rallied pieces gain +1 movement for 1 turn.
4. **Supreme Commander (Upgrade 3, 160 XP):** Rally affects entire board, costs 4 AP instead of 5.

**Tree B: Domination**
1. **Queen's Gambit (Basic):** Sacrifice adjacent allied piece to gain +3 movement this turn.
2. **Calculated Risk (Upgrade 1, 50 XP):** Sacrificed piece grants +2 AP instead of +3 movement.
3. **Blood for Glory (Upgrade 2, 100 XP):** If sacrifice captures enemy, gain sacrificed piece's XP.
4. **Ruthless Efficiency (Upgrade 3, 160 XP):** Gambit no longer requires sacrifice, costs 3 AP.

#### King Abilities

**Tree A: Survival**
1. **Last Ditch (Basic):** Teleport to any square within 2 spaces, costs 5 AP.
2. **Desperate Flight (Upgrade 1, 60 XP):** Last Ditch range +1.
3. **Royal Escape (Upgrade 2, 120 XP):** Last Ditch costs 4 AP.
4. **Divine Right (Upgrade 3, 180 XP):** Last Ditch can swap positions with any allied piece on board.

**Tree B: Leadership**
1. **King's Decree (Basic):** All allied pieces gain +20 LV, costs 5 AP.
2. **Monarch's Will (Upgrade 1, 60 XP):** Decree also resets 1 random ability cooldown per piece.
3. **Crown Authority (Upgrade 2, 120 XP):** Decree grants +10 HP to all allies.
4. **Absolute Power (Upgrade 3, 180 XP):** Decree transforms all Wavering/Disloyal pieces to Loyal.

---

## 6. Court System

### King's Court (Files a-d) vs Queen's Court (Files e-h)

**Court Bonuses:**
- Pieces in their home court gain +5 LV per turn
- Cross-court movements cost +1 AP
- Court-specific abilities activate only when piece is in correct court

**Court Abilities:**

**King's Court Focus:** Defense, Endurance
- Pieces in King's Court heal +5 HP per turn passively
- Rooks in King's Court: Fortify duration +1 turn

**Queen's Court Focus:** Offense, Mobility
- Pieces in Queen's Court: +1 movement range
- Knights in Queen's Court: Charge damage +10

**Court Control:**
When opponent has more pieces in your court than you do:
- **Court Contested:** Your court bonuses reduced by 50%
- If contested for 5+ consecutive turns: **Court Fallen**
- Court Fallen: Opponent gains your court bonuses, you lose them until you reclaim majority

---

## 7. Win Conditions

**Standard Victory:**
- Checkmate opponent's King (King cannot move, all escape squares attacked)

**Attrition Victory (Attrition Mode only):**
- Reduce opponent's King to 1 HP and capture Queen

**Loyalty Victory:**
- Have 10+ enemy pieces defect to your side via loyalty mechanics

**Domination Victory:**
- Control both courts (8+ pieces in enemy territory) for 10 consecutive turns

---

## 8. Turn Structure

1. **Start Phase:** Gain 5 AP (max 10), passive healing triggers
2. **Loyalty Phase:** Calculate LV changes, check for insurrection
3. **Action Phase:** Spend AP on moves/abilities
4. **Resolution Phase:** Apply damage, check win conditions, advance cooldowns
5. **End Phase:** XP gains calculated, promotion checks, stress state evaluation

**Turn Timer (Async Mode):**
- 72-hour window per turn
- If player doesn't submit moves within 72 hours: auto-pass turn, -10 LV to all pieces
- If player misses 3 consecutive turns: forfeit

---

## 9. Special Rules

### Peasant Recruitment
- Knights with "Levy" ability can recruit Peasants from reserve
- Peasants: 15 HP, move 1 square any direction, 1 damage attacks
- Peasants gain XP 50% faster but cap at Footman rank

### Taxation System (Optional Future Module)
- Each captured enemy piece generates 1 Gold
- Gold can be spent:
  - 3 Gold: Heal any piece to full HP
  - 5 Gold: Grant piece +1 level instantly
  - 10 Gold: Revive captured piece at spawn location

### Terrain Tiles (Unlocked at Turn 30)
- Forests: -1 movement, +5 defense
- Rivers: Block orthogonal movement unless Knight
- Castles: Grant Fortify effect to occupying piece

---

## 10. Anti-Cheat & Validation

**Server-Side Validation:**
- All move legality checked server-side
- Client sends move intent, server calculates outcomes
- Cryptographic move log: SHA-256 hash chain, each move hashes (previous hash + move data + timestamp)

**Illegal Move Detection:**
- 3 illegal move attempts in single turn: -20 LV to all pieces
- 10 illegal moves in game: automatic forfeit

**Time Analysis:**
- Moves submitted <100ms apart flagged for review
- Consistent perfect ability timing (within 5ms) flagged

---

## 11. Matchmaking & ELO

**Initial Rating:** 1200 ELO

**ELO Calculation:**
```
Expected Score = 1 / (1 + 10^((Opponent ELO - Your ELO) / 400))
New ELO = Old ELO + K × (Actual Score - Expected Score)

K-Factor:
- Games 1-30: K=40 (placement matches)
- Games 31-100: K=20
- Games 100+: K=10

Actual Score:
- Win: 1.0
- Loss: 0.0
- Draw (mutual agreement): 0.5
```

**Anti-Smurf:**
- New accounts flagged if win rate >80% in first 20 games
- Flagged accounts: K-factor doubled, matched against higher ELO
- If maintain >75% win rate through game 50: ELO recalibrated +300

**Rating Decay:**
- 7 days inactive: -5 ELO per day
- 30 days inactive: -10 ELO per day
- Max decay: -200 ELO

---

## 12. Future Expansion Modules

**Weather System:**
- Rain: -1 movement all pieces, healing reduced 50%
- Fog: Vision limited to 3 squares
- Snow: Knights lose Charge ability

**Seasonal Campaigns:**
- Spring: +10% XP gain
- Summer: Attrition Mode activates earlier (Turn 15)
- Autumn: Court bonuses doubled
- Winter: Stress state triggered more easily

**Diplomacy:**
- Neutral AI-controlled pieces on board
- Can be recruited via LV gifts
- Mercenary pieces fight for highest bidder

---

## Glossary

- **LV (Loyalty Value):** 0-100 scale measuring piece allegiance
- **AP (Action Points):** Currency for moves/abilities, 5 per turn, max 10 stored
- **Stress State:** Game mode when King threatened or Queen captured
- **Court:** Half of board (King's Court: a-d files, Queen's Court: e-h files)
- **Vassalage:** Hierarchical lord-servant relationship between pieces
- **Attrition Mode:** Combat system with HP/damage, activates Turn 21+

---

**Version:** 1.0  
**Last Updated:** 2026-01-26  
**Status:** Foundation Ruleset - Pending Playtesting