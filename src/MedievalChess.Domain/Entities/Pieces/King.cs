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
        // Standard king moves (one square in any direction)
        var directions = new[] 
        { 
            (0, 1), (0, -1), (1, 0), (-1, 0),
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        foreach (var (dFile, dRank) in directions)
        {
            var f = from.File + dFile;
            var r = from.Rank + dRank;
            
            if (Primitives.Position.IsValid(f, r))
            {
                var target = new Position(f, r);
                if (!board.IsOccupied(target) || board.IsOccupiedByEnemy(target, Color))
                {
                    yield return target;
                }
            }
        }

        // Castling moves (pseudo-legal - full legality checked in MoveGenerator)
        if (!HasMoved)
        {
            var backRank = Color == PlayerColor.White ? 0 : 7;
            
            // Kingside castling (e1->g1 or e8->g8)
            if (board.CanCastle(Color, kingside: true))
            {
                var f1 = new Position(5, backRank); // f1/f8
                var g1 = new Position(6, backRank); // g1/g8
                
                // Check path is clear
                if (!board.IsOccupied(f1) && !board.IsOccupied(g1))
                {
                    // Check rook is still there and hasn't moved
                    var rookPos = new Position(7, backRank);
                    var rook = board.GetPieceAt(rookPos);
                    if (rook != null && rook.Type == PieceType.Rook && !rook.HasMoved)
                    {
                        yield return g1; // Castling destination
                    }
                }
            }
            
            // Queenside castling (e1->c1 or e8->c8)
            if (board.CanCastle(Color, kingside: false))
            {
                var d1 = new Position(3, backRank); // d1/d8
                var c1 = new Position(2, backRank); // c1/c8
                var b1 = new Position(1, backRank); // b1/b8
                
                // Check path is clear
                if (!board.IsOccupied(d1) && !board.IsOccupied(c1) && !board.IsOccupied(b1))
                {
                    // Check rook is still there and hasn't moved
                    var rookPos = new Position(0, backRank);
                    var rook = board.GetPieceAt(rookPos);
                    if (rook != null && rook.Type == PieceType.Rook && !rook.HasMoved)
                    {
                        yield return c1; // Castling destination
                    }
                }
            }
        }
    }
}
