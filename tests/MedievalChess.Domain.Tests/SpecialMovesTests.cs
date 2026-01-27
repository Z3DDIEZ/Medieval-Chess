using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Entities.Pieces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Tests;

public class SpecialMovesTests
{
    private readonly EngineService _engine = new();

    // ====== CASTLING TESTS ======

    [Fact]
    public void King_CanCastleKingside_WhenPathClear()
    {
        // Arrange: Clear path for kingside castling
        var board = new Board();
        var king = new King(PlayerColor.White, new Position(4, 0));
        var rook = new Rook(PlayerColor.White, new Position(7, 0));
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7))); // Need black king
        
        // Act
        var moves = king.GetPseudoLegalMoves(king.Position!.Value, board).ToList();
        
        // Assert: g1 should be a valid destination (kingside castle)
        Assert.Contains(moves, m => m.File == 6 && m.Rank == 0);
    }

    [Fact]
    public void King_CanCastleQueenside_WhenPathClear()
    {
        // Arrange: Clear path for queenside castling
        var board = new Board();
        var king = new King(PlayerColor.White, new Position(4, 0));
        var rook = new Rook(PlayerColor.White, new Position(0, 0));
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        
        // Act
        var moves = king.GetPseudoLegalMoves(king.Position!.Value, board).ToList();
        
        // Assert: c1 should be a valid destination (queenside castle)
        Assert.Contains(moves, m => m.File == 2 && m.Rank == 0);
    }

    [Fact]
    public void King_CannotCastle_AfterKingMoves()
    {
        // Arrange
        var board = new Board();
        var king = new King(PlayerColor.White, new Position(4, 0));
        var rook = new Rook(PlayerColor.White, new Position(7, 0));
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        
        // Simulate king having moved
        king.MarkAsMoved();
        
        // Act
        var moves = king.GetPseudoLegalMoves(king.Position!.Value, board).ToList();
        
        // Assert: g1 should NOT be a valid destination after king moves
        Assert.DoesNotContain(moves, m => m.File == 6 && m.Rank == 0);
    }

    [Fact]
    public void King_CannotCastle_WhenPathBlocked()
    {
        // Arrange
        var board = new Board();
        var king = new King(PlayerColor.White, new Position(4, 0));
        var rook = new Rook(PlayerColor.White, new Position(7, 0));
        var bishop = new Bishop(PlayerColor.White, new Position(5, 0)); // Blocks f1
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(bishop);
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        
        // Act
        var moves = king.GetPseudoLegalMoves(king.Position!.Value, board).ToList();
        
        // Assert: g1 should NOT be available (blocked by bishop on f1)
        Assert.DoesNotContain(moves, m => m.File == 6 && m.Rank == 0);
    }

    [Fact]
    public void King_CannotCastle_ThroughCheck()
    {
        // Arrange: Enemy rook attacks f1 (king would pass through check)
        var board = new Board();
        var king = new King(PlayerColor.White, new Position(4, 0));
        var rook = new Rook(PlayerColor.White, new Position(7, 0));
        var enemyRook = new Rook(PlayerColor.Black, new Position(5, 7)); // Attacks f-file
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(enemyRook);
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        
        // Act
        var isLegal = _engine.IsMoveLegal(board, new Position(4, 0), new Position(6, 0), PlayerColor.White);
        
        // Assert: Castling should be illegal (king passes through check on f1)
        Assert.False(isLegal);
    }

    // ====== EN PASSANT TESTS ======

    [Fact]
    public void Pawn_HasEnPassantMove_WhenTargetSet()
    {
        // Arrange: White pawn on e5, black just played d7-d5
        var board = new Board();
        var whitePawn = new Pawn(PlayerColor.White, new Position(4, 4)); // e5
        var blackPawn = new Pawn(PlayerColor.Black, new Position(3, 4)); // d5 (just moved)
        board.AddPiece(whitePawn);
        board.AddPiece(blackPawn);
        board.AddPiece(new King(PlayerColor.White, new Position(4, 0)));
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        
        // Set en passant target (the square the capturing pawn moves to)
        board.SetEnPassantTarget(new Position(3, 5)); // d6
        
        // Act
        var moves = whitePawn.GetPseudoLegalMoves(whitePawn.Position!.Value, board).ToList();
        
        // Assert: d6 should be a valid en passant capture
        Assert.Contains(moves, m => m.File == 3 && m.Rank == 5);
    }

    [Fact]
    public void Pawn_NoEnPassant_WhenTargetNotSet()
    {
        // Arrange: White pawn on e5, black pawn on d5 but no en passant target
        var board = new Board();
        var whitePawn = new Pawn(PlayerColor.White, new Position(4, 4)); // e5
        var blackPawn = new Pawn(PlayerColor.Black, new Position(3, 4)); // d5
        board.AddPiece(whitePawn);
        board.AddPiece(blackPawn);
        board.AddPiece(new King(PlayerColor.White, new Position(4, 0)));
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        
        // No en passant target set
        
        // Act
        var moves = whitePawn.GetPseudoLegalMoves(whitePawn.Position!.Value, board).ToList();
        
        // Assert: d6 should NOT be available (no en passant without target)
        Assert.DoesNotContain(moves, m => m.File == 3 && m.Rank == 5);
    }

    // ====== PAWN PROMOTION TESTS ======

    [Fact]
    public void Pawn_IsPromotionRank_DetectsCorrectly()
    {
        // White pawns promote on rank 7, black on rank 0
        Assert.True(Pawn.IsPromotionRank(7, PlayerColor.White));
        Assert.True(Pawn.IsPromotionRank(0, PlayerColor.Black));
        Assert.False(Pawn.IsPromotionRank(0, PlayerColor.White));
        Assert.False(Pawn.IsPromotionRank(7, PlayerColor.Black));
    }

    // ====== CHECKMATE DETECTION TESTS ======

    [Fact]
    public void IsCheckmate_FoolsMate_DetectedCorrectly()
    {
        // Fool's Mate: 1.f3 e5 2.g4 Qh4#
        var game = Game.StartNew();
        
        // 1. f3 e5
        game.ExecuteMove(Position.FromAlgebraic("f2"), Position.FromAlgebraic("f3"), _engine);
        game.ExecuteMove(Position.FromAlgebraic("e7"), Position.FromAlgebraic("e5"), _engine);
        
        // 2. g4 Qh4#
        game.ExecuteMove(Position.FromAlgebraic("g2"), Position.FromAlgebraic("g4"), _engine);
        game.ExecuteMove(Position.FromAlgebraic("d8"), Position.FromAlgebraic("h4"), _engine);
        
        // Assert
        Assert.Equal(GameStatus.Checkmate, game.Status);
    }

    [Fact]
    public void IsStalemate_DetectedWhenNoLegalMoves()
    {
        // Simplified stalemate position: Black king alone, no legal moves, not in check
        var board = new Board();
        board.AddPiece(new King(PlayerColor.Black, new Position(0, 7))); // a8
        board.AddPiece(new King(PlayerColor.White, new Position(1, 5))); // b6 - blocks escape
        board.AddPiece(new Queen(PlayerColor.White, new Position(1, 6))); // b7 - traps king
        
        // Act
        var isStalemate = _engine.IsStalemate(board, PlayerColor.Black);
        var isCheck = _engine.IsKingInCheck(board, PlayerColor.Black);
        
        // Assert
        Assert.False(isCheck);
        Assert.True(isStalemate);
    }
}
