using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    public enum TimeOfDay
    {
        [InspectorName("Dawn (4.30 - 7.00)")]
        Dawn,
        [InspectorName("Day (7.00 - 19.00)")]
        Day,
        [InspectorName("Dusk (19.00 - 21.30)")]
        Dusk,
        [InspectorName("Night (21.30 - 4.30)")]
        Night,
    }

    public static class TimeOfDayUtility
    {
        public static string GetName(this TimeOfDay tod) {
            switch(tod) {
                case TimeOfDay.Dawn: return "Dawn";
                case TimeOfDay.Day: return "Day";
                case TimeOfDay.Dusk: return "Dusk";
                case TimeOfDay.Night: return "Night";
            }
            return "Unknown";
        }

        public static float GetStartTime(this TimeOfDay tod) {
            switch(tod) {
                case TimeOfDay.Dawn: return TimeSystem.START_OF_DAWN;
                case TimeOfDay.Day: return TimeSystem.START_OF_DAY;
                case TimeOfDay.Dusk: return TimeSystem.START_OF_DUSK;
                case TimeOfDay.Night: return TimeSystem.START_OF_NIGHT;
            }
            return 0f;
        }

        public static float GetEndTime(this TimeOfDay tod) {
            switch(tod) {
                case TimeOfDay.Dawn: return TimeSystem.START_OF_DAY;
                case TimeOfDay.Day: return TimeSystem.START_OF_DUSK;
                case TimeOfDay.Dusk: return TimeSystem.START_OF_NIGHT;
                case TimeOfDay.Night: return TimeSystem.START_OF_DAWN;
            }
            return 0f;
        }
    }
}
