using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngineInternal;

namespace Toolkit {
    public static class NSOUtility {

        private const string TAG = "[Toolkit.NSOUtility] - ";

        #region Rectangle Splitting

        public static void SplitRectHorizontal(Rect rect, out Rect left, out Rect right, float leftPercentage)
            => SplitRectHorizontal(rect, out left, out right, leftPercentage, 0f);

        public static void SplitRectHorizontal(Rect rect, out Rect left, out Rect right, float leftPercentage, float spacing) {
            float leftWidth = rect.width * leftPercentage - spacing / 2f;
            left = new Rect(rect.x, rect.y, leftWidth, rect.height);
            right = new Rect(rect.x + leftWidth + spacing, rect.y, rect.width - (leftWidth + spacing), rect.height);
        }

        public static void SplitVertical(Rect rect, out Rect up, out Rect down, float upPercentage)
            => SplitVertical(rect, out up, out down, upPercentage, 0f);

        public static void SplitVertical(Rect rect, out Rect up, out Rect down, float upPercentage, float spacing) {
            float upHeight = rect.height * upPercentage - spacing / 2f;
            up = new Rect(rect.x, rect.y, rect.width, upHeight);
            down = new Rect(rect.x, rect.y + upHeight + spacing, rect.width, rect.height - (upHeight + spacing));
        }

        #endregion

        #region Padding

        public static Rect Pad(Rect rect, float left, float right, float up, float down) {
            return new Rect(rect.x + left, rect.y + up, rect.width - (right + left), rect.height - (down + up));
        }

        #endregion

        #region Colors

        public static void DrawEdges(Rect area, int depth) {
            var col = GetColor(depth, NSOEditorSettings.ColorMode);
            EditorGUI.DrawRect(Pad(area, 0, area.width - 2, 0, 0), col);
            EditorGUI.DrawRect(Pad(area, 0, 0, area.height - 2, 0), col);
        }

        public static Color GetColor(int depth)
            => GetColor(depth, NSOEditorSettings.ColorMode);

        public static Color GetColor(int depth, NSOColor c) {
            switch(c) {
                case NSOColor.BlueVariant:
                case NSOColor.SepiaVariant:
                    depth = (depth / 2) + (depth % 2) * 4;
                    break;
            }
            switch(c) {
                case NSOColor.None: return Color.clear;
                case NSOColor.Default: return GetDefaultColor(depth);
                case NSOColor.Grayscale: return new Color(0f, 0f, 0f, (depth % 4 + 3) / 7f);
                case NSOColor.SepiaVariant:
                case NSOColor.Sepia: return Color.Lerp(new Color(0.13176f, 0.07764f, 0.0235f), new Color(0.64666f, 0.44f, 0.13333f), (depth % 8) / 7f);
                case NSOColor.BlueVariant:
                case NSOColor.Blue: return Color.Lerp(new Color(0f, 0f, 0.5f), new Color(135f / 255f, 206f / 255f, 235f / 255f), (depth % 8) / 7f);
            }
            return UnityEngine.Color.clear;
        }

        private static Color GetDefaultColor(int i) {
            if(i == 0)
                return new Color(0, 0.75f, 1.0f, 1f);
            int r = (Bit(i, 4) + Bit(i, 1) * 2 + 1) * 63;
            int g = (Bit(i, 3) + Bit(i, 2) * 2 + 1) * 63;
            int b = (Bit(i, 5) + Bit(i, 0) * 2 + 1) * 63;
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 1f);
        }

        private static int Bit(int a, int b) => (a & (1 << b)) >> b;

        #endregion

        #region Destroy

        public static void VerifyAndDestroy(SerializedProperty property) {
            if(property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue == null) {
                Debug.LogError(TAG + $"SerializedProperty '{property.propertyPath}' did not have an object reference");
                return;
            }

            // Fix a recursive find and destroy all child objects

            AssetDatabase.RemoveObjectFromAsset(property.objectReferenceValue);
            ScriptableObject.DestroyImmediate(property.objectReferenceValue);
            AssetDatabase.SaveAssetIfDirty(property.serializedObject.targetObject);
        }

        #endregion
    }
}
