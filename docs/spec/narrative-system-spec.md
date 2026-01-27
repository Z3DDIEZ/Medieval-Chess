# Battle Narrative System - Technical Specification

## 1. Overview
The **Battle Narrative System** adds an immersive RPG layer to Medieval Chess by translating game events into narrative text and dialogs. It simulates a "Dungeon Master" or "Sports Caster" providing commentary and dramatic flair to piece actions, dice rolls, and game state changes.

## 2. Core Components

### 2.1 GameNarrative Aggregate (Domain Layer)
*Refer to `docs/domain-model.md` Section 5 for the entity structure.*

**Responsibilities:**
- Store the history of narrative entries (`Entries`).
- Track the current `Tone` of the battle (e.g., Heroic, Desperate, Crushing).
- Persist generation history to allow for "Replay Story" features later.

### 2.2 NarrativeEngineService (Application Layer)
A domain service responsible for generating `NarrativeEntry` objects from `DomainEvent` triggers.

**Dependencies:**
- `IRNGService`: For variance in message variety.
- `IPieceRepository`: To lookup piece names/titles (e.g., "Sir Gallahad" instead of "White Knight").

**Key Methods:**
- `Handle(PieceMovedEvent)`
- `Handle(PieceAttackedEvent)` (Custom event for combat calculations)
- `Handle(AbilityUsedEvent)`
- `Handle(LoyaltyChangedEvent)`

### 2.3 Narrative Templates & Logic
The system uses a template-based generation system with condition matching.

**Template Structure:**
```json
{
  "trigger": "CombatHit",
  "conditions": {
    "minDamage": 15,
    "isCritical": true,
    "attackerType": "Knight"
  },
  "templates": [
    "{attacker} delivers a crushing blow! The earth shakes as {defender} takes {damage} damage!",
    "A perfect strike from {attacker}! {defender} reels from the impact."
  ],
  "tone": "Heroic"
}
```

### 2.4 Tone Analyzer
Dynamically adjusts the "mood" of the narrative.

- **Grim**: Triggered when a player loses >50% army or King is at <30% HP.
  - *Output Style*: "Hope fades...", "Desperation mounts..."
- **Heroic**: Triggered by critical hits, high-value captures, or major ability success.
  - *Output Style*: "Glorious!", "Unstoppable!"
- **Chaotic**: Triggered by high variance RNG (Max rolls followed by Min rolls), or many loyalty changes.
  - *Output Style*: "The tides of battle turn wildly!", "Chaos reigns!"

## 3. Integration Points

### 3.1 Combat Processing (Attack & Damage)
When the `CombatManager` calculates damage (see Attrition Mode):
1. **Input**: Attacker, Defender, BaseDamage, RollMultiplier, Armor, FinalDamage.
2. **Analysis**:
   - `RollMultiplier > 1.1` -> "High Roll" (Lucky/Skilled).
   - `RollMultiplier < 0.9` -> "Low Roll" (Unlucky/Glancing).
   - `FinalDamage == 0` -> "Blocked/Armor Save".
3. **Generation**: Create `NarrativeEntry` detailing the specific interaction.
   - *Example*: "Rolled 0.8 (Unlucky)... The attack glances off {defender}'s armor!"

### 3.2 UI Integration
- **Narrative/Dialog Panel**: A new frontend component (React).
- **Updates**: Real-time updates via SignalR when `NarrativeEntry` is generated.
- **Display**:
  - **Speaker Portrait**: Icon of the piece or a generic "General" avatar.
  - **Text**: The generated message.
  - **Visual Cues**: Shake effect for heavy damage, Gold border for criticals.

## 4. Development Tasks

### Backend (C#)
- [ ] Implement `GameNarrative` and `NarrativeEntry` entities in Domain.
- [ ] Create `NarrativeEngineService` interface and implementation.
- [ ] Define `NarrativeTemplates` (hardcoded initially, move to JSON/DB later).
- [ ] Wire up Domain Events (`PieceAttacked`, etc.) to the Narrative Service.

### Frontend (React)
- [ ] Create `BattleDialogComponent`.
- [ ] Integrate into `GamePage` layout (bottom or side panel).
- [ ] Animate updates using Framer Motion (typing effect for text).

## 5. Future Expansions
- **Personality System**: Pieces have traits (Cowardly, Brave) affecting their auto-generated dialog lines.
- **Voiceover**: Text-to-speech integration.
