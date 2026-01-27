using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities.Pieces;

public class King : Piece
{
    public override PieceType Type => PieceType.King;

    public King(PlayerColor color, Position position) : base(color, position)
    {
        MaxHP = 100;
        CurrentHP = MaxHP;
    }

    private King() { }

    public override IEnumerable<Position> GetPseudoLegalMoves(Position from, Board board)
    {
        var directions = new[] 
        { 
            (0, 1), (0, -1), (1, 0), (-1, 0),
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        foreach (var (dFile, dRank) in directions)
        {
            var f = from.File + dFile;
            var r = from.Rank + dRank;
            
            if (MedievalChess.Domain.Primitives.Position.IsValid(f, r))
            {
                var target = new Position(f, r);
                if (!board.IsOccupied(target) || board.IsOccupiedByEnemy(target, Color))
                {
                    yield return target;
                }
            }
        }

        // TODO: Castling (requires board state/history)
    }
}
