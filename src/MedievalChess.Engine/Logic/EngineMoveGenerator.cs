using System.Collections.Generic;
using MedievalChess.Domain.Enums;
using MedievalChess.Engine.Structures;

namespace MedievalChess.Engine.Logic
{
    public static class EngineMoveGenerator
    {
        // Offsets
        private static readonly int[] KnightOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };
        private static readonly int[] KingOffsets = { -9, -8, -7, -1, 1, 7, 8, 9 };
        private static readonly int[] RookDirs = { -8, -1, 1, 8 };
        private static readonly int[] BishopDirs = { -9, -7, 7, 9 };
        
        public static void GenerateMoves(ref EngineGameState state, List<EngineMove> moves)
        {
            // Iterate all pieces
            for (int i = 0; i < state.Pieces.Length; i++)
            {
                var p = state.Pieces[i];
                if (p.IsCaptured || p.SquareIndex == 255) continue;
                if (p.Color != (byte)state.CurrentTurn) continue;
                
                GeneratePieceMoves(ref state, p, moves);
            }
        }

        private static void GeneratePieceMoves(ref EngineGameState state, PieceState p, List<EngineMove> moves)
        {
            int sq = p.SquareIndex;
            int type = p.Type; // 0=Pawn...6=King? 

            // PieceType Enum: Pawn=0, Knight=1, Bishop=2, Rook=3, Queen=4, King=5
            switch (type)
            {
                case 0: // Pawn
                    GeneratePawnMoves(ref state, p, moves);
                    break;
                case 1: // Knight
                    GenerateLeaperMoves(ref state, p, KnightOffsets, moves);
                    break;
                case 2: // Bishop
                     GenerateSliderMoves(ref state, p, BishopDirs, moves);
                     break;
                case 3: // Rook
                     GenerateSliderMoves(ref state, p, RookDirs, moves);
                     break;
                case 4: // Queen
                     GenerateSliderMoves(ref state, p, RookDirs, moves);
                     GenerateSliderMoves(ref state, p, BishopDirs, moves);
                     break;
                case 5: // King
                     GenerateLeaperMoves(ref state, p, KingOffsets, moves);
                     break;
            }
        }

        private static void GeneratePawnMoves(ref EngineGameState state, PieceState p, List<EngineMove> moves)
        {
            int sq = p.SquareIndex;
            int rank = sq / 8;
            int file = sq % 8;
            
            // White moves UP (-8?), Black moves DOWN (+8?)
            // WAIT! Rank 0 is usually bottom. Rank 7 is top.
            // Index = Rank*8 + File.
            // Moving UP (Rank++) = +8.
            // Moving DOWN (Rank--) = -8.
            
            // Assumption: White is at Ranks 0-1. Black at 6-7.
            // White Pawns move +8. Black -8.
            
            int direction = (p.Color == 0) ? 8 : -8;
            int startRank = (p.Color == 0) ? 1 : 6;
            
            // 1. Single Push
            int target = sq + direction;
            if (IsValidSquare(target) && state.BoardSquares[target] == 255) // Empty
            {
                moves.Add(new EngineMove(p.Id, (byte)target));
                
                // 2. Double Push (only if single valid and on start rank)
                if (rank == startRank)
                {
                    int target2 = sq + (direction * 2);
                    if (IsValidSquare(target2) && state.BoardSquares[target2] == 255)
                    {
                        moves.Add(new EngineMove(p.Id, (byte)target2));
                    }
                }
            }
            
            // 3. Captures (Diagonal)
            // Diagonals: +7, +9 for White. -7, -9 for Black.
            // Need to wrap check (File overflow)
            int[] captureOffsets = (p.Color == 0) ? new[] { 7, 9 } : new[] { -9, -7 };
            
            foreach (var offset in captureOffsets)
            {
                int cTarget = sq + offset;
                if (!IsValidSquare(cTarget)) continue;
                
                // Wrap check: absolute difference in File must be 1
                int cFile = cTarget % 8;
                if (System.Math.Abs(cFile - file) != 1) continue;
                
                byte victimIdx = state.BoardSquares[cTarget];
                if (victimIdx != 255)
                {
                    // Check ownership
                    if (state.Pieces[victimIdx].Color != p.Color)
                    {
                         moves.Add(new EngineMove(p.Id, (byte)cTarget, victimIdx));
                    }
                }
            }
        }

        private static void GenerateLeaperMoves(ref EngineGameState state, PieceState p, int[] offsets, List<EngineMove> moves)
        {
            int sq = p.SquareIndex;
            int rank = sq / 8;
            int file = sq % 8;
            
            foreach(int off in offsets)
            {
                int target = sq + off;
                if (!IsValidSquare(target)) continue;
                
                // Boundary check (Knight jumps wrapping wrapping)
                // We compare files/ranks
                int tRank = target / 8;
                int tFile = target % 8;
                
                // Knights: Max file diff is 2, Max rank diff is 2
                // If diff > 2, it wrapped incorrectly
                if (System.Math.Abs(tFile - file) > 2 || System.Math.Abs(tRank - rank) > 2) continue;
                
                byte victimIdx = state.BoardSquares[target];
                if (victimIdx == 255)
                {
                    moves.Add(new EngineMove(p.Id, (byte)target));
                }
                else if (state.Pieces[victimIdx].Color != p.Color)
                {
                    moves.Add(new EngineMove(p.Id, (byte)target, victimIdx));
                }
            }
        }
        
        private static void GenerateSliderMoves(ref EngineGameState state, PieceState p, int[] directions, List<EngineMove> moves)
        {
             int sq = p.SquareIndex;
             
             foreach(int dir in directions)
             {
                 int current = sq;
                 while(true)
                 {
                     int prevFile = current % 8;
                     current += dir;
                     if (!IsValidSquare(current)) break;
                     
                     // Wrap check for sliders (East/West/Diagonal)
                     int currFile = current % 8;
                     // If we moved 1 square left/right, file diff should be 1
                     // If we moved diagonal, file diff 1
                     // If we moved vertical, file diff 0
                     
                     // If wrapping happened, geometric distance breaks
                     // Specifically: 
                     // +1 (Right): File increases. If 7->0 (wrapped), new file < old file.
                     // -1 (Left): File decreases.
                     // Vertical (+8): File same.
                     
                     // Robust check:
                     int fileDiff = System.Math.Abs(currFile - prevFile);
                     if (fileDiff > 1) break; // Wrapped around
                     
                     byte victimIdx = state.BoardSquares[current];
                     if (victimIdx == 255)
                     {
                         moves.Add(new EngineMove(p.Id, (byte)current));
                     }
                     else
                     {
                         if (state.Pieces[victimIdx].Color != p.Color)
                         {
                             moves.Add(new EngineMove(p.Id, (byte)current, victimIdx));
                         }
                         break; // Blocked
                     }
                 }
             }
        }

        private static bool IsValidSquare(int sq) => sq >= 0 && sq < 64;
    }
}
