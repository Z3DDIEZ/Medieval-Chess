namespace MedievalChess.Domain.Common
{
    public interface IRNGService
    {
        int Next(int maxValue, int seed);
        int Next(int minValue, int maxValue, int seed);
        double NextDouble(int seed);
        // Helper specifically for our "Initiative Contest" or checks
        bool RollChance(double percentage, int seed);
    }
}
