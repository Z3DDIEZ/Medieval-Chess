using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Logic;

/// <summary>
/// Static catalog of all ability definitions in the game.
/// Used to look up ability costs, cooldowns, and effects.
/// </summary>
public static class AbilityCatalog
{
    private static readonly Dictionary<AbilityType, AbilityDefinition> _abilities = new();

    static AbilityCatalog()
    {
        RegisterAllAbilities();
    }

    public static AbilityDefinition? Get(AbilityType type)
    {
        return _abilities.TryGetValue(type, out var def) ? def : null;
    }

    public static IEnumerable<AbilityDefinition> GetForPieceType(PieceType pieceType)
    {
        return _abilities.Values.Where(a => a.RequiredPieceType == pieceType);
    }

    private static void RegisterAllAbilities()
    {
        // ===== KNIGHT ABILITIES =====
        // Vanguard Tree
        Register(new AbilityDefinition(
            AbilityType.Charge, "Charge", 
            "Move in L-shape, then 2 additional squares in cardinal direction. +5 damage if ending on enemy.",
            AbilityTier.Basic, apCost: 2, cooldown: 3, xpRequired: 0, 
            PieceType.Knight, requiresTarget: true, range: 4));
        
        Register(new AbilityDefinition(
            AbilityType.LanceStrike, "Lance Strike",
            "Charge ignores intervening pieces.",
            AbilityTier.Upgrade1, apCost: 2, cooldown: 3, xpRequired: 30,
            PieceType.Knight, requiresTarget: true, range: 4));
        
        Register(new AbilityDefinition(
            AbilityType.CavalryMomentum, "Cavalry Momentum",
            "After Charge capture, can immediately move 1 square.",
            AbilityTier.Upgrade2, apCost: 3, cooldown: 4, xpRequired: 60,
            PieceType.Knight, requiresTarget: true, range: 4));
        
        Register(new AbilityDefinition(
            AbilityType.DevastatingImpact, "Devastating Impact",
            "Charge deals +15 damage and pushes defender 1 square back.",
            AbilityTier.Upgrade3, apCost: 3, cooldown: 5, xpRequired: 100,
            PieceType.Knight, requiresTarget: true, range: 4));

        // Defender Tree
        Register(new AbilityDefinition(
            AbilityType.ShieldWall, "Shield Wall",
            "Grant +5 armor to adjacent allies for 2 turns.",
            AbilityTier.Basic, apCost: 2, cooldown: 4, xpRequired: 0,
            PieceType.Knight, requiresTarget: false, range: 1));
        
        Register(new AbilityDefinition(
            AbilityType.Stalwart, "Stalwart",
            "Shield Wall duration +1 turn.",
            AbilityTier.Upgrade1, apCost: 2, cooldown: 4, xpRequired: 30,
            PieceType.Knight, requiresTarget: false, range: 1));

        // ===== BISHOP ABILITIES =====
        // Divine Tree
        Register(new AbilityDefinition(
            AbilityType.Sanctify, "Sanctify",
            "Heal adjacent ally 20 HP, grant +10 LV.",
            AbilityTier.Basic, apCost: 2, cooldown: 3, xpRequired: 0,
            PieceType.Bishop, requiresTarget: true, range: 1));
        
        Register(new AbilityDefinition(
            AbilityType.HolyGround, "Holy Ground",
            "Sanctify affects all adjacent allies.",
            AbilityTier.Upgrade1, apCost: 2, cooldown: 3, xpRequired: 35,
            PieceType.Bishop, requiresTarget: false, range: 1));
        
        Register(new AbilityDefinition(
            AbilityType.Resurrection, "Resurrection",
            "Once per game, revive captured allied piece at 50% HP on adjacent square.",
            AbilityTier.Upgrade3, apCost: 5, cooldown: 999, xpRequired: 120,
            PieceType.Bishop, requiresTarget: true, range: 1));

        // ===== ROOK ABILITIES =====
        // Siege Tree
        Register(new AbilityDefinition(
            AbilityType.Fortify, "Fortify",
            "Reduce incoming damage by 50% for 3 turns, cannot move.",
            AbilityTier.Basic, apCost: 2, cooldown: 5, xpRequired: 0,
            PieceType.Rook, requiresTarget: false, range: 0));
        
        // Logistics Tree
        Register(new AbilityDefinition(
            AbilityType.Garrison, "Garrison",
            "Store 1 allied piece inside Rook (immune to damage, can exit next turn).",
            AbilityTier.Basic, apCost: 2, cooldown: 4, xpRequired: 0,
            PieceType.Rook, requiresTarget: true, range: 1));

        // ===== QUEEN ABILITIES =====
        // Command Tree
        Register(new AbilityDefinition(
            AbilityType.Rally, "Rally",
            "Reset cooldowns of all vassals within 3 squares.",
            AbilityTier.Basic, apCost: 3, cooldown: 5, xpRequired: 0,
            PieceType.Queen, requiresTarget: false, range: 3));
        
        Register(new AbilityDefinition(
            AbilityType.InspiringPresence, "Inspiring Presence",
            "Rally also grants +15 LV to affected pieces.",
            AbilityTier.Upgrade1, apCost: 3, cooldown: 5, xpRequired: 50,
            PieceType.Queen, requiresTarget: false, range: 3));

        // Domination Tree
        Register(new AbilityDefinition(
            AbilityType.QueensGambit, "Queen's Gambit",
            "Sacrifice adjacent allied piece to gain +3 movement this turn.",
            AbilityTier.Basic, apCost: 2, cooldown: 4, xpRequired: 0,
            PieceType.Queen, requiresTarget: true, range: 1));

        // ===== KING ABILITIES =====
        // Survival Tree
        Register(new AbilityDefinition(
            AbilityType.LastDitch, "Last Ditch",
            "Teleport to any square within 2 spaces.",
            AbilityTier.Basic, apCost: 5, cooldown: 6, xpRequired: 0,
            PieceType.King, requiresTarget: true, range: 2));
        
        Register(new AbilityDefinition(
            AbilityType.DivineRight, "Divine Right",
            "Last Ditch can swap positions with any allied piece on board.",
            AbilityTier.Upgrade3, apCost: 5, cooldown: 6, xpRequired: 180,
            PieceType.King, requiresTarget: true, range: 8));

        // Leadership Tree
        Register(new AbilityDefinition(
            AbilityType.KingsDecree, "King's Decree",
            "All allied pieces gain +20 LV.",
            AbilityTier.Basic, apCost: 5, cooldown: 8, xpRequired: 0,
            PieceType.King, requiresTarget: false, range: 8));
        
        Register(new AbilityDefinition(
            AbilityType.AbsolutePower, "Absolute Power",
            "Decree transforms all Wavering/Disloyal pieces to Loyal.",
            AbilityTier.Upgrade3, apCost: 5, cooldown: 8, xpRequired: 180,
            PieceType.King, requiresTarget: false, range: 8));
    }

    private static void Register(AbilityDefinition def)
    {
        _abilities[def.Type] = def;
    }
}
