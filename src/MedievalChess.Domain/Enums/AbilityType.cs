namespace MedievalChess.Domain.Enums;

/// <summary>
/// All ability types available in Medieval Chess.
/// Organized by piece class and ability tree.
/// </summary>
public enum AbilityType
{
    None = 0,
    
    // Knight - Vanguard Tree
    Charge,
    LanceStrike,
    CavalryMomentum,
    DevastatingImpact,
    
    // Knight - Defender Tree
    ShieldWall,
    Stalwart,
    GuardiansResolve,
    UnyieldingBastion,
    
    // Bishop - Divine Tree
    Sanctify,
    HolyGround,
    MartyrsBlessing,
    Resurrection,
    
    // Bishop - Authority Tree
    PontificalGuard,
    ConsecratedLine,
    DivineJudgment,
    PapalDecree,
    
    // Rook - Siege Tree
    Fortify,
    ImmovableObject,
    ReactivePlating,
    SiegeEngine,
    
    // Rook - Logistics Tree
    Garrison,
    SupplyLine,
    RapidDeployment,
    WarWagon,
    
    // Queen - Command Tree
    Rally,
    InspiringPresence,
    TacticalGenius,
    SupremeCommander,
    
    // Queen - Domination Tree
    QueensGambit,
    CalculatedRisk,
    BloodForGlory,
    RuthlessEfficiency,
    
    // King - Survival Tree
    LastDitch,
    DesperateFlight,
    RoyalEscape,
    DivineRight,
    
    // King - Leadership Tree
    KingsDecree,
    MonarchsWill,
    CrownAuthority,
    AbsolutePower
}
