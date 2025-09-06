using System;
using System.Diagnostics;

namespace Toolkit {
    public static class StopwatchExtensions {

        #region Variables

        public static readonly float TICKS_TO_MILLISECONDS = 1f / (Stopwatch.Frequency / 1000f);
        public static readonly float TICKS_TO_SECONDS = 1f / (Stopwatch.Frequency);

        #endregion

        #region Float Timers

        public static float GetMilliseconds(this Stopwatch stopwatch) => stopwatch.ElapsedTicks * TICKS_TO_MILLISECONDS;
        public static float GetSeconds(this Stopwatch stopwatch) => stopwatch.ElapsedTicks * TICKS_TO_SECONDS;

        public static string GetMillisecondsFormatted(this Stopwatch stopwatch) => $"{stopwatch.ElapsedTicks * TICKS_TO_MILLISECONDS:0.00}ms";
        public static string GetSecondsFormatted(this Stopwatch stopwatch) => $"{stopwatch.ElapsedTicks * TICKS_TO_SECONDS:0.00}s";

        #endregion
    }
}
