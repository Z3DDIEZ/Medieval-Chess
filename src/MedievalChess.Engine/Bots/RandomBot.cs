using System;
using System.Linq;
using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Engine.Bots
{
    public class RandomBot : IMedievalEngine
    {
        private readonly Random _random = new Random();
        private readonly IEngineService _rulesEngine; // Reuse existing rules for validation

        public RandomBot(IEngineService rulesEngine)
        {
            _rulesEngine = rulesEngine;
        }

        public Move CalculateBestMove(Game game, int timeLimitMs)
        {
            // 1. Get all pieces for current player
            var pieces = game.Board.Pieces
                .Where(p => p.Color == game.CurrentTurn && !p.IsCaptured)
                .ToList();

            // 2. Generate all valid moves (naive approach)
            // In a real engine, we'd use a MoveGenerator. ITS in ALPHA --dev
            // Here we rely on the piece's pre-calculated ValidMoves or iterate board.
            // Since Board doesn't pre-calc all moves, we must iterate.
            
            var validMoves = new System.Collections.Generic.List<Move>();

            foreach (var piece in pieces)
            {
                // Try every square? Inefficient but works for Phase 1 Random Bot
                for (int rank = 0; rank < 8; rank++)
                {
                    for (int file = 0; file < 8; file++)
                    {
                        var targetPos = new Position(file, rank);
                        if (_rulesEngine.IsMoveLegal(game.Board, piece.Position!.Value, targetPos, game.CurrentTurn, game.IsAttritionMode))
                        {
                            var targetPiece = game.Board.GetPieceAt(targetPos);
                            var move = new Move(piece.Position.Value, targetPos, piece, targetPiece);
                            
                            if (piece.Type == PieceType.Pawn && MedievalChess.Domain.Entities.Pieces.Pawn.IsPromotionRank(rank, piece.Color))
                            {
                                move.PromotionPiece = PieceType.Queen; // Default to Queen for random bot
                            }

                            validMoves.Add(move);
                        }
                    }
                }
            }

            if (validMoves.Count == 0) return null!; // Resign/Checkmate

            // 3. Pick Random
            return validMoves[_random.Next(validMoves.Count)];
        }

        public bool IsMoveValid(Game game, Move move)
        {
            return _rulesEngine.IsMoveLegal(game.Board, move.From, move.To, game.CurrentTurn);
        }
    }
}
