using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type => PieceType.Bishop;

    public Bishop(PlayerColor color, Position position) : base(color, position)
    {
        MaxHP = 40;
        CurrentHP = MaxHP;
    }

    private Bishop() { }

    public override IEnumerable<Position> GetPseudoLegalMoves(Position from, Board board)
    {
        var directions = new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) };

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
