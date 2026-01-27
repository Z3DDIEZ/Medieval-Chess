using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities.Pieces;

public class Knight : Piece
{
    public override PieceType Type => PieceType.Knight;

    public Knight(PlayerColor color, Position position) : base(color, position)
    {
        MaxHP = 40;
        CurrentHP = MaxHP;
    }

    private Knight() { }

    public override IEnumerable<Position> GetPseudoLegalMoves(Position from, Board board)
    {
        var offsets = new[]
        {
            (1, 2), (1, -2), (-1, 2), (-1, -2),
            (2, 1), (2, -1), (-2, 1), (-2, -1)
        };

        foreach (var (fileOffset, rankOffset) in offsets)
        {
            var f = from.File + fileOffset;
            var r = from.Rank + rankOffset;
            
            if (!MedievalChess.Domain.Primitives.Position.IsValid(f, r)) continue;

            var target = new Position(f, r);
            if (!board.IsOccupied(target) || board.IsOccupiedByEnemy(target, Color))
            {
                yield return target;
            }
        }
    }
}
