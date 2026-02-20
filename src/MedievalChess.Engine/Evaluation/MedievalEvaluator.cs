using System;
using MedievalChess.Domain.Enums;
using MedievalChess.Engine.Structures;

namespace MedievalChess.Engine.Evaluation
{
    public static class MedievalEvaluator
    {
        // Standard values (centipawns)
        private static readonly int[] PieceTypeValues = { 0, 100, 320, 330, 500, 900, 20000 }; 
        // 0=None, 1=Pawn, 2=Knight, 3=Bishop, 4=Rook, 5=Queen, 6=King (from Enum 0-6 usually)
        
        // Enum mapping verification:
        // Pawn=0? No, let's check Enum again.
        // PieceType.Pawn is 0?
        // Game.cs line 3: enum PieceType { Pawn, Knight... }
        // Yes, Pawn=0. 
        // So: Pawn(0)=100, Knight(1)=320, Bishop(2)=330, Rook(3)=500, Queen(4)=900, King(5)=20000.
        // I will adjust the array.

        private static readonly int[] Values = { 100, 320, 330, 500, 900, 20000 };

        public static int Evaluate(EngineGameState state, PlayerColor myColor)
        {
            // Terminal check is usually done in Search, but Evaluator can handle Checkmate bonus?
            // Search usually handles Mate.
            
            // 1. Material (Medieval Weighted)
            int score = 0;
            
            // Value AP (e.g. 20 centipawns per AP)
            int whiteScore = state.WhiteAP * 20;
            int blackScore = state.BlackAP * 20;

            for (int i = 0; i < state.Pieces.Length; i++)
            {
                var p = state.Pieces[i];
                if (p.IsCaptured) continue;
                
                // Base Value
                int typeIndex = p.Type; // 0-5
                int baseVal = (typeIndex >= 0 && typeIndex < Values.Length) ? Values[typeIndex] : 0;
                
                // HP Scaling (Medieval)
                // Value = Base * (HP/MaxHP)
                // Avoid divide by zero
                int hpVal = baseVal;
                if (p.MaxHP > 0)
                {
                    // Use long calculation to avoid overflow before divide
                    hpVal = (int)((long)baseVal * p.CurrentHP / p.MaxHP);
                }
                
                // Loyalty Risk
                if (p.Loyalty < 30)
                {
                    hpVal -= 50; // Significant penalty for risk of losing the piece
                }

                // Court Control (Simple rank check)
                if (p.Color == 0) // White
                {
                    // Enemy Court: Ranks 4-7 (Index 32-63)
                    if (p.SquareIndex >= 32) hpVal += 10;
                    whiteScore += hpVal;
                }
                else // Black
                {
                    // Enemy Court: Ranks 0-3 (Index 0-31)
                    if (p.SquareIndex < 32) hpVal += 10;
                    blackScore += hpVal;
                }
            }
            
            score = whiteScore - blackScore;
            
            return myColor == PlayerColor.White ? score : -score;
        }
    }
}
