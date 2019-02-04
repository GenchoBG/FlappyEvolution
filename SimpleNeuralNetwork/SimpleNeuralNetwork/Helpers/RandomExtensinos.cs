using System;

namespace SimpleNeuralNetwork.Helpers
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static bool NextBool(this Random random)
        {
            return random.Next(0, 2) == 0;
        }
    }
}
