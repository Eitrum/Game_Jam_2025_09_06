using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    public static partial class ExperienceUtility
    {
        #region Variablees

        private static IExperienceTable defaultTable;

        #endregion

        #region Properties

        public static IExperienceTable Table {
            get => defaultTable;
            set {
                if(value == null)
                    defaultTable = new SimpleExperienceTable(experienceTable);
                else
                    defaultTable = value;
            }
        }

        public static int Levels => defaultTable.Levels;
        public static IReadOnlyList<double> DifferenceTable => defaultTable.Difference;
        public static IReadOnlyList<double> TotalExperienceTable => defaultTable.TotalExperience;

        #endregion

        #region Init

        static ExperienceUtility() {
            defaultTable = new SimpleExperienceTable(experienceTable);
        }

        #endregion

        #region Clamp

        public static int ClampLevel(int level) => UnityEngine.Mathf.Clamp(level, 0, Levels);
        public static int ClampLevel(this IExperienceTable table, int level) => Mathf.Clamp(level, 0, table.Levels);
        private static int ClampLevelInternal(int level) => UnityEngine.Mathf.Clamp(level, 0, Levels - 1);
        private static int ClampLevelInternal(this IExperienceTable table, int level) => UnityEngine.Mathf.Clamp(level, 0, table.Levels - 1);

        #endregion

        #region Experience Needed

        public static double ExperienceNeededForLevel(int level) => DifferenceTable[ClampLevelInternal(level)];
        public static double ExperienceNeededForLevel(this IExperienceTable table, int level) => table.Difference[table.ClampLevel(level)];

        #endregion

        #region Total Experience Needed

        public static double TotalExperienceNeededForLevel(int level) => TotalExperienceTable[ClampLevel(level)];
        public static double TotalExperienceNeededForLevel(this IExperienceTable table, int level) => table.TotalExperience[table.ClampLevel(level)];

        #endregion

        #region LevelFromTotal Experience

        public static int GetLevelFromTotalExperience(double experience) {
            if(experience < 0)
                return 0;
            for(int i = 0; i < Levels; i++)
                if(experience < TotalExperienceTable[i])
                    return (i - 1);
            return Levels;

        }

        public static int GetLevelFromTotalExperience(this IExperienceTable table, double experience) {
            if(experience < 0)
                return 0;
            for(int i = 0; i < table.Levels; i++)
                if(experience < table.TotalExperience[i])
                    return (i - 1);
            return table.Levels;
        }

        #endregion
    }
}
