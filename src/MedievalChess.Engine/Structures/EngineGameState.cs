using System;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Engine.Structures
{
    // High-performance struct for Minimax
    public struct EngineGameState
    {
        // Minimal representation of the board for speed
        // 8x8 array or 64-length flat array for pieces
        // Using int for Piece data: [Type, Color, HP, ID] packed
        public byte[] Board; 
        
        public PlayerColor CurrentTurn;
        public int TurnNumber;
        public int WhiteAP;
        public int BlackAP;
        
        // Metadata required for Medieval logic
        public bool IsStressState;
        public bool IsAttritionMode;

        // Factory/Conversion logic will be in Mapper
    }
}
