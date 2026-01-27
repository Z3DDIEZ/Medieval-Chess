using System;
using MedievalChess.Domain.Common;

namespace MedievalChess.Infrastructure.Services
{
    public class RNGService : IRNGService
    {
        public int Next(int maxValue, int seed)
        {
            var random = new Random(seed);
            return random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue, int seed)
        {
            var random = new Random(seed);
            return random.Next(minValue, maxValue);
        }

        public double NextDouble(int seed)
        {
            var random = new Random(seed);
            return random.NextDouble();
        }

        public bool RollChance(double percentage, int seed)
        {
            var random = new Random(seed);
            return random.NextDouble() < percentage;
        }
    }
}
