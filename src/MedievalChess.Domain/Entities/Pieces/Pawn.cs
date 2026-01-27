using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

    public Pawn(PlayerColor color, Position position) : base(color, position)
    {
        MaxHP = 20;
        CurrentHP = MaxHP;
    }

    private Pawn() { } // EF Core

    public override IEnumerable<Position> GetPseudoLegalMoves(Position from, Board board)
    {
        var direction = Color == PlayerColor.White ? 1 : -1;
        var startRank = Color == PlayerColor.White ? 1 : 6;

        // 1. Move Forward 1
        var f1Rank = from.Rank + direction;
        if (MedievalChess.Domain.Primitives.Position.IsValid(from.File, f1Rank))
        {
            var forward1 = new Position(from.File, f1Rank);
            if (!board.IsOccupied(forward1))
            {
                yield return forward1;

                // 2. Move Forward 2
                if (from.Rank == startRank)
                {
                    var f2Rank = from.Rank + (direction * 2);
                    if (MedievalChess.Domain.Primitives.Position.IsValid(from.File, f2Rank))
                    {
                        var forward2 = new Position(from.File, f2Rank);
                        if (!board.IsOccupied(forward2))
                        {
                            yield return forward2;
                        }
                    }
                }
            }
        }

        // 3. Captures
        var leftFile = from.File - 1;
        var captureRank = from.Rank + direction;
        if (MedievalChess.Domain.Primitives.Position.IsValid(leftFile, captureRank))
        {
            var captureLeft = new Position(leftFile, captureRank);
            if (board.IsOccupiedByEnemy(captureLeft, Color))
            {
                yield return captureLeft;
            }
        }

        var rightFile = from.File + 1;
        if (MedievalChess.Domain.Primitives.Position.IsValid(rightFile, captureRank))
        {
            var captureRight = new Position(rightFile, captureRank);
            if (board.IsOccupiedByEnemy(captureRight, Color))
            {
                yield return captureRight;
            }
        }
        
        // TODO: En Passant logic requires board state (en passant target square)
    }
}
