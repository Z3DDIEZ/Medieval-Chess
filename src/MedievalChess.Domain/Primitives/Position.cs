using System.Diagnostics.CodeAnalysis;

namespace MedievalChess.Domain.Primitives;

public readonly struct Position : IEquatable<Position>
{
    public int Rank { get; } // 0-7 (Rows, 1-8 in chess)
    public int File { get; } // 0-7 (Cols, a-h in chess)

    public Position(int file, int rank)
    {
        if (!IsValid(file, rank))
            throw new ArgumentOutOfRangeException("Position out of board bounds");
        
        File = file;
        Rank = rank;
    }

    public static bool IsValid(int file, int rank) => 
        file is >= 0 and <= 7 && rank is >= 0 and <= 7;

    public static Position FromAlgebraic(string algebraic)
    {
        if (string.IsNullOrEmpty(algebraic) || algebraic.Length != 2)
            throw new ArgumentException("Invalid algebraic notation");

        int file = algebraic[0] - 'a';
        int rank = algebraic[1] - '1';
        
        return new Position(file, rank);
    }

    public string ToAlgebraic()
    {
        return $"{(char)('a' + File)}{(char)('1' + Rank)}";
    }

    public bool IsAdjacentTo(Position other)
    {
        int fileDiff = Math.Abs(File - other.File);
        int rankDiff = Math.Abs(Rank - other.Rank);
        return fileDiff <= 1 && rankDiff <= 1;
    }

    public bool IsEquivalent(Position other) => File == other.File && Rank == other.Rank;
    public override bool Equals(object? obj) => obj is Position other && Equals(other);
    public bool Equals(Position other) => File == other.File && Rank == other.Rank;
    public override int GetHashCode() => HashCode.Combine(File, Rank);
    
    public static bool operator ==(Position left, Position right) => left.Equals(right);
    public static bool operator !=(Position left, Position right) => !left.Equals(right);
    public override string ToString() => ToAlgebraic();
}
