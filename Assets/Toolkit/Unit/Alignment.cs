using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    /*
     *             Alignment Chart
     *  ======================================
     *  Lawful Good - Good      - Chaotic Good
     *  Lawful      - Neutral   - Chaotic
     *  Lawful Evil - Evil      - Chaotic Evil
     * 
     *                 Grid Id
     *  ======================================
     *     (1, 1)   -   (0, 1)  -   (-1, 1)
     *     (1, 0)   -   (0, 0)  -   (-1, 0)
     *     (1,-1)   -   (0,-1)  -   (-1,-1)
     */

    public enum Alignment
    {
        [InspectorName("Neutral")] Neutral = 0,
        [InspectorName("Lawful")] Lawful = 1,
        [InspectorName("Chaotic")] Chaotic = 2,
        [InspectorName("Good")] Good = 4,
        [InspectorName("Evil")] Evil = 8,
        [InspectorName("Lawful Good")] LawfulGood = Lawful | Good,
        [InspectorName("Lawful Evil")] LawfulEvil = Lawful | Evil,
        [InspectorName("Chaotic Good")] ChaoticGood = Chaotic | Good,
        [InspectorName("Chaotic Evil")] ChaoticEvil = Chaotic | Evil,
    }

    public static class AlignmentUtility
    {
        #region Constants

        private const int LAWFUL_CHAOTIC_MASK = 3;
        private const int GOOD_EVIL_MASK = 12;

        #endregion

        #region Validity

        public static bool IsValid(this Alignment alignment) {
            return !(alignment.HasFlag(Alignment.Lawful) && alignment.HasFlag(Alignment.Chaotic)) || !(alignment.HasFlag(Alignment.Good) && alignment.HasFlag(Alignment.Evil));
        }

        public static void Repair(this ref Alignment alignment) {
            if(alignment.HasFlag(Alignment.Lawful) && alignment.HasFlag(Alignment.Chaotic)) {
                alignment = (Alignment)(((int)alignment) ^ LAWFUL_CHAOTIC_MASK);
            }
            if(alignment.HasFlag(Alignment.Good) && alignment.HasFlag(Alignment.Evil)) {
                alignment = (Alignment)(((int)alignment) ^ GOOD_EVIL_MASK);
            }
        }

        #endregion

        #region Checks

        public static bool IsLawful(this Alignment alignment) => alignment.HasFlag(Alignment.Lawful) && !alignment.HasFlag(Alignment.Chaotic);
        public static bool IsChaotic(this Alignment alignment) => alignment.HasFlag(Alignment.Chaotic) && !alignment.HasFlag(Alignment.Lawful);

        public static bool IsGood(this Alignment alignment) => alignment.HasFlag(Alignment.Good) && !alignment.HasFlag(Alignment.Evil);
        public static bool IsEvil(this Alignment alignment) => alignment.HasFlag(Alignment.Evil) && !alignment.HasFlag(Alignment.Good);

        public static bool IsTrueNeutral(this Alignment alignment) => alignment == Alignment.Neutral;

        #endregion

        #region GridId

        public static Vector2Int GetGridId(this Alignment alignment) {
            Vector2Int gridId = new Vector2Int(0, 0);
            if(alignment.HasFlag(Alignment.Lawful))
                gridId.x++;
            if(alignment.HasFlag(Alignment.Chaotic))
                gridId.x--;
            if(alignment.HasFlag(Alignment.Good))
                gridId.y++;
            if(alignment.HasFlag(Alignment.Evil))
                gridId.y--;

            return gridId;
        }

        public static Alignment GetAlignmentFromGridId(Vector2Int gridId) {
            Alignment alignment = Alignment.Neutral;

            if(gridId.x > 0)
                alignment |= Alignment.Lawful;
            else if(gridId.x < 0)
                alignment |= Alignment.Chaotic;

            if(gridId.y > 0)
                alignment |= Alignment.Good;
            else if(gridId.y < 0)
                alignment |= Alignment.Evil;

            return alignment;
        }

        #endregion

        #region GetFlags

        public static List<Alignment> GetFlags(this Alignment alignment) {
            List<Alignment> flags = new List<Alignment>();

            if(alignment.HasFlag(Alignment.Lawful))
                flags.Add(Alignment.Lawful);
            if(alignment.HasFlag(Alignment.Chaotic))
                flags.Add(Alignment.Chaotic);
            if(alignment.HasFlag(Alignment.Good))
                flags.Add(Alignment.Good);
            if(alignment.HasFlag(Alignment.Evil))
                flags.Add(Alignment.Evil);

            if(flags.Count == 0)
                flags.Add(Alignment.Neutral);

            return flags;
        }

        public static void GetFlags(this Alignment alignment, List<Alignment> list) {
            list.Clear();

            if(alignment.HasFlag(Alignment.Lawful))
                list.Add(Alignment.Lawful);
            if(alignment.HasFlag(Alignment.Chaotic))
                list.Add(Alignment.Chaotic);
            if(alignment.HasFlag(Alignment.Good))
                list.Add(Alignment.Good);
            if(alignment.HasFlag(Alignment.Evil))
                list.Add(Alignment.Evil);

            if(list.Count == 0)
                list.Add(Alignment.Neutral);
        }

        public static int GetFlags(this Alignment alignment, Alignment[] list) {
            int index = 0;
            int size = list.Length;

            if(index < size && alignment.HasFlag(Alignment.Lawful))
                list[index++] = Alignment.Lawful;
            if(index < size && alignment.HasFlag(Alignment.Chaotic))
                list[index++] = Alignment.Chaotic;
            if(index < size && alignment.HasFlag(Alignment.Good))
                list[index++] = Alignment.Good;
            if(index < size && alignment.HasFlag(Alignment.Evil))
                list[index++] = Alignment.Evil;

            if(index < size && index == 0)
                list[index++] = Alignment.Neutral;

            return index;
        }

        #endregion

        #region String conversions

        public static string ToStringOptimized(this Alignment alignment) {
            Repair(ref alignment);
            switch(alignment) {
                case Alignment.Neutral: return "Neutral";

                case Alignment.Lawful: return "Lawful";
                case Alignment.LawfulGood: return "Lawful Good";
                case Alignment.LawfulEvil: return "Lawful Evil";

                case Alignment.Chaotic: return "Chaotic";
                case Alignment.ChaoticGood: return "Chaotic Good";
                case Alignment.ChaoticEvil: return "Chaotic Evil";

                case Alignment.Good: return "Good";
                case Alignment.Evil: return "Evil";
            }
            return "Neutral";
        }

        public static string ToStringOptimized(this Alignment alignment, bool repair) {
            if(repair)
                Repair(ref alignment);
            switch(alignment) {
                case Alignment.Neutral: return "Neutral";

                case Alignment.Lawful: return "Lawful";
                case Alignment.LawfulGood: return "Lawful Good";
                case Alignment.LawfulEvil: return "Lawful Evil";

                case Alignment.Chaotic: return "Chaotic";
                case Alignment.ChaoticGood: return "Chaotic Good";
                case Alignment.ChaoticEvil: return "Chaotic Evil";

                case Alignment.Good: return "Good";
                case Alignment.Evil: return "Evil";
            }
            return "Neutral";
        }

        public static string ToStringFlagged(this Alignment alignment) {
            return $"[{string.Join(" | ", alignment.GetFlags())}]";
        }

        #endregion
    }
}
