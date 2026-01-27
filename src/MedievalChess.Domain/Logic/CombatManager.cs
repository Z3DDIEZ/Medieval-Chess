using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Logic;

public class CombatManager
{
    private readonly Game _game;

    public CombatManager(Game game)
    {
        _game = game;
    }

    public CombatResult CalculateCombat(Piece attacker, Piece defender)
    {
        // 1. Get Values
        int attackerValue = GetPieceValue(attacker.Type);
        
        // 2. Deterministic RNG
        // Seed = GameSeed + Turn + AttackerID + DefenderID
        // This ensures the same attack always yields same result if replayed
        int seed = _game.CombatSeed + _game.TurnNumber + attacker.Id.GetHashCode() + defender.Id.GetHashCode();
        var rng = new Random(seed);

        // 3. Base Calculation
        int baseDamage = attackerValue * 2;

        // 4. Level Modifier
        // (Level/10 + 1) -> Level 10 gives 2x damage. Level 1 gives 1.1x
        double levelMultiplier = (attacker.Level / 10.0) + 1.0;

        // 5. RNG Modifier (0.8 to 1.2)
        double rngModifier = 0.8 + (rng.NextDouble() * 0.4);

        double modifiedDamage = baseDamage * levelMultiplier * rngModifier;

        // 6. Critical Hit (15% chance)
        // Bonus 5% if Devoted (Loyalty > 90)
        double critChance = 0.15;
        if (attacker.Loyalty.Value >= 90) critChance += 0.05;

        bool isCritical = rng.NextDouble() < critChance;
        if (isCritical)
        {
            modifiedDamage *= 1.5;
        }

        // 7. Armor Reduction
        // Armor * (0.5 to 1.0)
        double armorRng = 0.5 + (rng.NextDouble() * 0.5);
        double defenseReduction = defender.Armor * armorRng;

        // 8. Final Damage (Min 1)
        int finalDamage = (int)Math.Max(1, modifiedDamage - defenseReduction);

        return new CombatResult
        {
            DamageDealt = finalDamage,
            IsCritical = isCritical,
            BaseDamage = baseDamage,
            ArmorReduced = (int)defenseReduction
        };
    }

    public static int GetPieceValue(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => 1,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Rook => 5,
            PieceType.Queen => 9,
            PieceType.King => 4, // Fighting King value
            PieceType.Peasant => 1,
            _ => 1
        };
    }
}

public class CombatResult
{
    public int DamageDealt { get; set; }
    public bool IsCritical { get; set; }
    public int BaseDamage { get; set; }
    public int ArmorReduced { get; set; }
}
