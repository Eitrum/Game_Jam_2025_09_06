
using UnityEditor;
using UnityEngine;

namespace Toolkit.QuickInspect {
    public static class QuickInspectUtil {

        public static bool IsHoldingAlt => Event.current?.alt ?? false;

        #region PropHandler

        public static void SetNestingLevel(SerializedProperty property, int value) {
            var handler = ScriptAttributeUtility.GetHandler(property);
            UnityInternalUtility.TrySetValue(handler, "m_NestingLevel", value);
        }

        #endregion

        #region Color

        private static int Bit(int a, int b) => (a & (1 << b)) >> b;

        public static Color GetColor(int i)
            => GetColor(i, 1f);

        public static Color GetColor(int i, float alpha) {
            if(i == 0)
                return new Color(0, 0.75f, 1.0f, 0.5f);
            int r = (Bit(i, 4) + Bit(i, 1) * 2 + 1) * 63;
            int g = (Bit(i, 3) + Bit(i, 2) * 2 + 1) * 63;
            int b = (Bit(i, 5) + Bit(i, 0) * 2 + 1) * 63;
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, alpha);
        }

        #endregion
    }
}
