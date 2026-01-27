using MedievalChess.Domain.Common;

namespace MedievalChess.Domain.Tests.Mocks
{
    public class TestRNGService : IRNGService
    {
        public int Next(int maxValue, int seed) => 0; // Deterministic low value
        public int Next(int minValue, int maxValue, int seed) => minValue;
        public double NextDouble(int seed) => 0.5; // Average luck
        public bool RollChance(double percentage, int seed) => true; // Always succeed for tests
    }
}
