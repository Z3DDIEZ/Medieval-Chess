using System;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Engine.Structures
{
    // High-performance struct for Minimax
    public struct EngineGameState
    {
        // Board Representation
        // 64 squares. Content = PieceIndex (0-31) or 255 (Empty).
        // This allows O(1) lookup of "What is at E4?"
        public byte[] BoardSquares; 
        
        // Detailed Piece Data
        // Indexed 0-15 (White), 16-31 (Black) usually, but we'll map dynamically
        public PieceState[] Pieces; 

        // Bitboards for rapid occupancy checks
        public ulong WhiteOcc;
        public ulong BlackOcc;
        public ulong AllOcc => WhiteOcc | BlackOcc;

        // Global State
        public PlayerColor CurrentTurn;
        public ushort TurnNumber;
        public byte WhiteAP; // 0-10
        public byte BlackAP; // 0-10
        
        // Metadata required for Medieval logic
        public bool IsStressState; // Example global flag
        public bool IsAttritionMode;

        // Medieval Extensions
        public byte WhiteCourtControl; // Some metric of control? Or just calc on fly.
        public byte BlackCourtControl;

        public EngineGameState(int pieceCount)
        {
            BoardSquares = new byte[64];
            Array.Fill(BoardSquares, (byte)255); // 255 = Empty
            
            Pieces = new PieceState[pieceCount];
            WhiteOcc = 0;
            BlackOcc = 0;
            CurrentTurn = PlayerColor.White;
            TurnNumber = 1;
            WhiteAP = 5;
            BlackAP = 5;
            IsStressState = false;
            IsAttritionMode = true;
            WhiteCourtControl = 0;
            BlackCourtControl = 0;
        }

        public EngineGameState Clone()
        {
            EngineGameState clone = this;
            clone.BoardSquares = (byte[])BoardSquares.Clone();
            clone.Pieces = (PieceState[])Pieces.Clone();
            return clone;
        }
    }
}
