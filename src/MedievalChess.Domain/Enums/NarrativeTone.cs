namespace MedievalChess.Domain.Enums
{
    public enum NarrativeTone
    {
        Neutral = 0,
        Heroic = 1,     // Winning, high morale, critical hits
        Grim = 2,       // Losing, low HP, stress state
        Chaotic = 3,    // High variance, unpredictable events
        Strategic = 4   // Slow pace, tactical moves
    }
}
