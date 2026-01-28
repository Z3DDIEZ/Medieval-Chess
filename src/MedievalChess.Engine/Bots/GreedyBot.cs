using System;
using System.Linq;
using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;
using MedievalChess.Engine.Evaluation;

namespace MedievalChess.Engine.Bots
{
    public class GreedyBot : IMedievalEngine
    {
        private readonly IEngineService _rulesEngine;

        public GreedyBot(IEngineService rulesEngine)
        {
            _rulesEngine = rulesEngine;
        }

        public Move CalculateBestMove(Game game, int timeLimitMs)
        {
            var myColor = game.CurrentTurn;
            var pieces = game.Board.Pieces.Where(p => p.Color == myColor && !p.IsCaptured).ToList();
            
            Move? bestMove = null;
            int bestScore = int.MinValue;

            // Simple Depth 1 Search
            foreach (var piece in pieces)
            {
                // Inefficient: iterating all squares to find pseudo-legal moves
                // Optimally we'd use GetPseudoLegalMoves but then validate check
                var legalDestinations = _rulesEngine.GetLegalDestinations(game.Board, piece.Position!.Value);
                
                foreach (var dest in legalDestinations)
                {
                    // Simulate Move (Lightweight)
                    // Since we don't have a lightweight clone yet, we might have to use heuristic on the move itself
                    // e.g. Capture Value - Victim Value
                    
                    // For a TRUE greedy bot we need to see the result state.
                    // Doing a full Clone & Execute is heavy.
                    // Let's rely on "Static Exchange Evaluation" concept (See what we capture).
                    
                    var target = game.Board.GetPieceAt(dest);
                    int currentMoveScore = 0;
                    
                    if (target != null)
                    {
                        // Capture Score
                        currentMoveScore += SimpleEvaluator.Evaluate(game, myColor) + (GetPieceValue(target.Type) * 10); 
                    }
                    else
                    {
                        // Positional Score (Randomized slightly for variety)
                        currentMoveScore += new Random().Next(0, 10);
                    }
                    
                    if (currentMoveScore > bestScore)
                    {
                        bestScore = currentMoveScore;
                        bestMove = new Move(piece.Position.Value, dest, piece, target);
                    }
                }
            }

            return bestMove ?? throw new Exception("No legal moves");
        }
        
        // Helper to avoid circular dependency on Evaluator for basic values
         private static int GetPieceValue(Domain.Enums.PieceType type)
        {
            return type switch
            {
                Domain.Enums.PieceType.Pawn => 100,
                Domain.Enums.PieceType.Knight => 320,
                Domain.Enums.PieceType.Bishop => 330,
                Domain.Enums.PieceType.Rook => 500,
                Domain.Enums.PieceType.Queen => 900,
                Domain.Enums.PieceType.King => 20000,
                _ => 0
            };
        }

        public bool IsMoveValid(Game game, Move move)
        {
            return _rulesEngine.IsMoveLegal(game.Board, move.From, move.To, game.CurrentTurn);
        }
    }
}
