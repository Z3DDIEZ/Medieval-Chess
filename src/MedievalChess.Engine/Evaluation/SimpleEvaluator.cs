using System;
using System.Linq;
using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Logic;

namespace MedievalChess.Engine.Evaluation
{
    public static class SimpleEvaluator
    {
        // Piece Base Values
        private static readonly int PawnValue = 100;
        private static readonly int KnightValue = 320;
        private static readonly int BishopValue = 330;
        private static readonly int RookValue = 500;
        private static readonly int QueenValue = 900;
        private static readonly int KingValue = 20000;

        public static int Evaluate(Game game, PlayerColor color)
        {
            int score = 0;
            
            // 1. Material & HP Score
            foreach (var piece in game.Board.Pieces.Where(p => !p.IsCaptured))
            {
                int pieceScore = GetPieceValue(piece);
                
                // Adjust for HP (Attrition Mode)
                if (game.IsAttritionMode)
                {
                    double healthPercent = (double)piece.CurrentHP / piece.MaxHP;
                    pieceScore = (int)(pieceScore * (0.5 + 0.5 * healthPercent)); // Damaged pieces worth less, but not 0
                }

                if (piece.Color == color)
                    score += pieceScore;
                else
                    score -= pieceScore;
            }

            // 2. Mobility (Number of valid moves) - simplified
            // Running move gen for every position is expensive for a simple eval, skipping for now.

            // 3. Positional Bonuses (Center control)
            // ...

            return score;
        }

        private static int GetPieceValue(Piece piece)
        {
            int baseVal = piece.Type switch
            {
                PieceType.Pawn => PawnValue,
                PieceType.Knight => KnightValue,
                PieceType.Bishop => BishopValue,
                PieceType.Rook => RookValue,
                PieceType.Queen => QueenValue,
                PieceType.King => KingValue,
                _ => 0
            };

            // Level Bonus
            baseVal += (piece.Level - 1) * 50; 

            return baseVal;
        }
    }
}
