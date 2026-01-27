using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public interface IEngineService
{
    bool IsKingInCheck(Board board, PlayerColor color);
    bool IsCheckmate(Board board, PlayerColor color);
    bool IsStalemate(Board board, PlayerColor color);
    bool IsMoveLegal(Board board, Position from, Position to, PlayerColor turn);
    IEnumerable<Position> GetLegalDestinations(Board board, Position from);
}
