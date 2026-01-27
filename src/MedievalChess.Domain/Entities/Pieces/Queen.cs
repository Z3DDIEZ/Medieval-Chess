using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities.Pieces;

public class Queen : Piece
{
    public override PieceType Type => PieceType.Queen;

    public Queen(PlayerColor color, Position position) : base(color, position)
    {
        MaxHP = 80;
        CurrentHP = MaxHP;
    }

    private Queen() { }

    public override IEnumerable<Position> GetPseudoLegalMoves(Position from, Board board)
    {
        // Combination of Rook and Bishop directions
        var directions = new[] 
        { 
            (0, 1), (0, -1), (1, 0), (-1, 0), // Rook
            (1, 1), (1, -1), (-1, 1), (-1, -1) // Bishop
        };

        foreach (var (dFile, dRank) in directions)
        {
            for (int i = 1; i < 8; i++)
            {
                var f = from.File + (dFile * i);
                var r = from.Rank + (dRank * i);

                if (!MedievalChess.Domain.Primitives.Position.IsValid(f, r)) break;

                var target = new Position(f, r);

                if (board.IsOccupied(target))
                {
                    if (board.IsOccupiedByEnemy(target, Color))
                    {
                        yield return target;
                    }
                    break; // Blocked
                }

                yield return target;
            }
        }
    }
}
