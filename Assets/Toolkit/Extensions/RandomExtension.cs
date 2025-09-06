using System;

namespace Toolkit
{
    public static class RandomExtension
    {
        #region Float

        public static float NextFloat(this System.Random random) => (float)random.NextDouble();
        public static float NextFloat(this System.Random random, float min, float max) => min + ((max - min) * (float)random.NextDouble());

        #endregion

        #region Bool

        public static bool NextBool(this System.Random random) => random.Next(2) == 1;
        public static bool NextBool(this System.Random random, float chance) => random.NextDouble() <= chance;
        public static bool NextBool(this System.Random random, double chance) => random.NextDouble() <= chance;
        /// <summary>
        /// Returns whether the random value is below the chance value, a range between 0->99 (included). 100 means it always succeedes, a 0 always fails.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static bool NextBool(this System.Random random, int chance) => random.Next(100) < chance;

        #endregion
    }
}
