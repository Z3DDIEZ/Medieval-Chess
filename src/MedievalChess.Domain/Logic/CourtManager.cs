using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Enums;

/// <summary>
/// Manages court-related logic (King's Court vs Queen's Court).
/// </summary>
public static class CourtHelper
{
    /// <summary>
    /// Queen's Court: Files a-d (0-3) - Queen starts on d-file
    /// King's Court: Files e-h (4-7) - King starts on e-file
    /// </summary>
    public static CourtType GetCourt(Position position)
    {
        return position.File < 4 ? CourtType.QueensCourt : CourtType.KingsCourt;
    }

    /// <summary>
    /// Determines if a move crosses from one court to another.
    /// </summary>
    public static bool IsCrossCourtMove(Position from, Position to)
    {
        return GetCourt(from) != GetCourt(to);
    }

    /// <summary>
    /// Returns the bonus AP cost for crossing courts (1 AP).
    /// </summary>
    public static int GetCrossCourtAPCost(Position from, Position to)
    {
        return IsCrossCourtMove(from, to) ? 1 : 0;
    }
}
