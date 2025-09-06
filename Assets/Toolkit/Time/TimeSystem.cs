using System.Collections;
using System.Collections.Generic;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.DayCycle
{
    public static class TimeSystem
    {
        #region Variables

        public const float START_OF_DAWN = 16200f;
        public const float START_OF_DAY = 25200f;
        public const float START_OF_DUSK = 68400f;
        public const float START_OF_NIGHT = 77400f;

        public const float MID_DAY = 46800f;
        public const float MIDNIGHT = 3600f;

        public const float HALF_DAY = 43200f;
        public const float FULL_DAY = 86400f;

        private static float time = MID_DAY;
        private static OnTimeUpdateCallback onTimeUpdate;

        #endregion

        #region Properties

        public static float Time => time;
        public static float NormalizedTime => time / FULL_DAY;
        public static string FormattedTime => FormatTime(time);

        public static bool IsDawn => time > START_OF_DAWN && time < START_OF_DAY;
        public static bool IsDay => time > START_OF_DAY && time < START_OF_DUSK;
        public static bool IsDusk => time > START_OF_DUSK && time < START_OF_NIGHT;
        public static bool IsNight => time > START_OF_NIGHT || time < START_OF_DAWN;

        public static bool IsSunUp => time > START_OF_DAWN && time < START_OF_NIGHT;
        public static float SunNormalized => Mathf.Clamp01((time - START_OF_DAWN) / (START_OF_NIGHT - START_OF_DAWN));
        public static float SunRotation => 180f * SunNormalized;

        public static float Intesity => CalculateIntensity(time);

        public static int Hour => (int)(time / 3600f);
        public static int Minute => (int)(time / 60f) % 60;
        public static int Second => (int)(time) % 60;

        public static event OnTimeUpdateCallback OnTimeUpdate {
            add {
                onTimeUpdate += value;
                value(0f, time);
            }
            remove => onTimeUpdate -= value;
        }

        public static TimeOfDay TimeOfDay {
            get {
                if(time < START_OF_DAWN)
                    return TimeOfDay.Night;
                if(time < START_OF_DAY)
                    return TimeOfDay.Dawn;
                if(time < START_OF_DUSK)
                    return TimeOfDay.Day;
                if(time < START_OF_NIGHT)
                    return TimeOfDay.Dusk;
                return TimeOfDay.Night;
            }
        }

        #endregion

        #region Initialize

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            // To handle scene changes correctly when time is not updating!
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (s0, s1) => Set(time);
        }

        #endregion

        #region Time Management Methods

        public static void Add(float time) {
            TimeSystem.time = (TimeSystem.time + time) % FULL_DAY;
            onTimeUpdate?.Invoke(time, TimeSystem.time);
        }

        public static void Set(float time) {
            var current = TimeSystem.time;
            var newTime = TimeSystem.time = time % FULL_DAY;
            onTimeUpdate?.Invoke(newTime - current, TimeSystem.time);
        }

        public static void Set(TimeOfDay timeOfDay) {
            switch(timeOfDay) {
                case TimeOfDay.Night: Set(START_OF_NIGHT); break;
                case TimeOfDay.Dawn: Set(START_OF_DAWN); break;
                case TimeOfDay.Day: Set(START_OF_DAY); break;
                case TimeOfDay.Dusk: Set(START_OF_DUSK); break;
            }
        }

        public static void Remove(float time) {
            TimeSystem.time = (TimeSystem.time - time + FULL_DAY) % FULL_DAY;
            onTimeUpdate?.Invoke(-time, TimeSystem.time);
        }

        #endregion

        #region Intensity Calculation

        public static float CalculateIntensity(float time) {
            if(time < START_OF_DAWN)
                return 0f;
            if(time < START_OF_DAY)
                return Ease.Quad.InOut((time - START_OF_DAWN) / (START_OF_DAY - START_OF_DAWN));
            if(time < START_OF_DUSK)
                return 1f;
            if(time < START_OF_NIGHT)
                return Ease.Quad.InOut(1f - ((time - START_OF_DUSK) / (START_OF_NIGHT - START_OF_DUSK)));
            return 0f;
        }

        public static float CalculateIntensity(float time, Ease.EaseFunction function) {
            if(time < START_OF_DAWN)
                return 0f;
            if(time < START_OF_DAY)
                return function((time - START_OF_DAWN) / (START_OF_DAY - START_OF_DAWN));
            if(time < START_OF_DUSK)
                return 1f;
            if(time < START_OF_NIGHT)
                return function(1f - ((time - START_OF_DUSK) / (START_OF_NIGHT - START_OF_DUSK)));
            return 0f;
        }

        #endregion

        #region Formatting

        public static string FormatTime(float time) => $"{((int)(time / 3600f)):00}:{((int)(time / 60f) % 60):00}:{((int)(time) % 60):00}";

        #endregion
    }
}
