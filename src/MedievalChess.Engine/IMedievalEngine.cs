using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Engine
{
    public interface IMedievalEngine
    {
        // Primary method for AI thinking
        Move CalculateBestMove(Game game, int timeLimitMs);
        
        // Validation support for Phase 1
        bool IsMoveValid(Game game, Move move);
    }
}
