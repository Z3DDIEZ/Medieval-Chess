using MedievalChess.Domain.Enums;

namespace MedievalChess.Engine.Structures
{
    // Optimized struct for holding piece state in the engine search tree
    // Blittable or near-blittable for performance ideally
    public struct PieceState
    {
        public byte Id;             // 0-31 (Mapping back to Domain Entity via mapper)
        public byte Type;           // Cast to PieceType (1=Pawn, etc.)
        public byte Color;          // 0=White, 1=Black
        
        // Spatial
        public byte SquareIndex;    // 0-63, 255=Captured
        public bool IsCaptured;
        public bool HasMoved;       // For castling logic

        // RPG Stats
        public short CurrentHP;
        public short MaxHP;
        public byte Loyalty;        // 0-100
        public byte Level;
        public int XP;

        // Abilities
        // In this phase, we support tracking cooldowns for up to 3 abilities
        public byte Ability1Cooldown;
        public byte Ability2Cooldown;
        public byte Ability3Cooldown;

        // Factory/Helper
        public static PieceState Captured() => new PieceState { IsCaptured = true, SquareIndex = 255 };
    }
}
