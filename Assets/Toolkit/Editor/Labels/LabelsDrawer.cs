using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public static class LabelsDrawer {
        public enum LabelColor {
            Gray,
            Blue,
            Cyan,
            Green,
            Yellow,
            Orange,
            Red,
            Purple,
        }

        public class Textures {
            #region Variables

            public readonly static Texture2D Gray = EditorGUIUtility.IconContent("sv_label_0").image as Texture2D;
            public readonly static Texture2D Blue = EditorGUIUtility.IconContent("sv_label_1").image as Texture2D;
            public readonly static Texture2D Cyan = EditorGUIUtility.IconContent("sv_label_2").image as Texture2D;
            public readonly static Texture2D Green = EditorGUIUtility.IconContent("sv_label_3").image as Texture2D;
            public readonly static Texture2D Yellow = EditorGUIUtility.IconContent("sv_label_4").image as Texture2D;
            public readonly static Texture2D Orange = EditorGUIUtility.IconContent("sv_label_5").image as Texture2D;
            public readonly static Texture2D Red = EditorGUIUtility.IconContent("sv_label_6").image as Texture2D;
            public readonly static Texture2D Purple = EditorGUIUtility.IconContent("sv_label_7").image as Texture2D;

            #endregion

            #region Get

            public static Texture2D Get(int index) {
                index = Mathf.Clamp(index, 0, 7);
                switch(index) {
                    case 0: return Gray;
                    case 1: return Blue;
                    case 2: return Cyan;
                    case 3: return Green;
                    case 4: return Yellow;
                    case 5: return Orange;
                    case 6: return Red;
                    case 7: return Purple;
                }
                return Gray;
            }

            public static Texture2D Get(LabelColor color) {
                switch(color) {
                    case LabelColor.Gray: return Gray;
                    case LabelColor.Blue: return Blue;
                    case LabelColor.Cyan: return Cyan;
                    case LabelColor.Green: return Green;
                    case LabelColor.Yellow: return Yellow;
                    case LabelColor.Orange: return Orange;
                    case LabelColor.Red: return Red;
                    case LabelColor.Purple: return Purple;
                }
                return Gray;
            }

            #endregion
        }

        public class Styles {
            #region Variables

            public static readonly GUIStyle Gray = new GUIStyle();
            public static readonly GUIStyle Blue = new GUIStyle();
            public static readonly GUIStyle Cyan = new GUIStyle();
            public static readonly GUIStyle Green = new GUIStyle();
            public static readonly GUIStyle Yellow = new GUIStyle();
            public static readonly GUIStyle Orange = new GUIStyle();
            public static readonly GUIStyle Red = new GUIStyle();
            public static readonly GUIStyle Purple = new GUIStyle();

            #endregion

            #region Init

            private static void Configure(GUIStyle style, int styleindex) {
                style.imagePosition = ImagePosition.ImageLeft;
                style.normal = new GUIStyleState() {
                    background = Textures.Get(styleindex),
                };
                style.padding = new RectOffset(6, 6, 2, 2);
                style.alignment = TextAnchor.MiddleCenter;
                style.wordWrap = false;
                style.richText = true;
                style.fontSize = 11;
                style.border = new RectOffset(8, 8, 8, 8);
                style.stretchWidth = true;
                style.stretchHeight = false;
            }

            static Styles() {
                Configure(Gray, 0);
                Configure(Blue, 1);
                Configure(Cyan, 2);
                Configure(Green, 3);
                Configure(Yellow, 4);
                Configure(Orange, 5);
                Configure(Red, 6);
                Configure(Purple, 7);
            }

            #endregion

            #region Get

            public static GUIStyle GetStyle(LabelColor color) {
                return GetStyle((int)color);
            }

            public static GUIStyle GetStyle(int styleindex) {
                switch(styleindex) {
                    case 0: return Gray;
                    case 1: return Blue;
                    case 2: return Cyan;
                    case 3: return Green;
                    case 4: return Yellow;
                    case 5: return Orange;
                    case 6: return Red;
                    case 7: return Purple;
                }
                return Gray;
            }

            #endregion
        }

        #region Calculate Size

        public static float CalculateWidth(string text)
            => CalculateSize(text).x;

        public static float CalculateHeight(string text)
            => CalculateSize(text).y;

        public static Vector2 CalculateSize(string text) {
            temp.text = text;
            temp.tooltip = string.Empty;
            temp.image = null;
            return CalculateSize(temp);
        }

        public static float CalculateWidth(GUIContent content)
            => CalculateSize(content).x;

        public static float CalculateHeight(GUIContent content)
            => CalculateSize(content).y;

        public static Vector2 CalculateSize(GUIContent content) {
            return Styles.Gray.CalcSize(content);
        }

        public static float CalculateWidth(LabelData data)
            => CalculateSize(data).x;

        public static float CalculateHeight(LabelData data)
            => CalculateSize(data).y;

        public static Vector2 CalculateSize(LabelData data) {
            temp.text = data.Name;
            temp.tooltip = string.Empty;
            temp.image = null;
            return CalculateSize(temp);
        }

        #endregion

        #region Calculate Grids

        public static float CalculateHeight(IReadOnlyList<string> labels, float width, bool offsetScroll = true) {
            var calculatedWidth = offsetScroll ? (width - 14) : width;
            var maxWidth = calculatedWidth;
            float x = 0;
            float y = 0;
            var heightToIncrease = 0f;
            for(int i = 0, len = labels.Count; i < len; i++) {
                var label = labels[i];
                var size = CalculateSize(label);
                if(size.x + x > maxWidth) { // Move to next line
                    x = 0;
                    y += heightToIncrease;
                    heightToIncrease = 0;
                }
                heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                x += size.x;
            }
            return y + heightToIncrease;
        }

        public static float CalculateHeight(IReadOnlyList<string> labels, float width, bool offsetScroll, System.Func<string, bool> filter) {
            var calculatedWidth = offsetScroll ? (width - 14) : width;
            var maxWidth = calculatedWidth;
            float x = 0;
            float y = 0;
            var heightToIncrease = 0f;
            for(int i = 0, len = labels.Count; i < len; i++) {
                var label = labels[i];
                if(!filter(label))
                    continue;
                var size = CalculateSize(label);
                if(size.x + x > maxWidth) { // Move to next line
                    x = 0;
                    y += heightToIncrease;
                    heightToIncrease = 0;
                }
                heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                x += size.x;
            }
            return y + heightToIncrease;
        }

        public static float CalculateHeight(IEnumerable<string> labels, float width, bool offsetScroll = true) {
            var calculatedWidth = offsetScroll ? (width - 14) : width;
            var maxWidth = calculatedWidth;
            float x = 0;
            float y = 0;
            var heightToIncrease = 0f;
            foreach(var label in labels) {
                var size = CalculateSize(label);
                if(size.x + x > maxWidth) { // Move to next line
                    x = 0;
                    y += heightToIncrease;
                    heightToIncrease = 0;
                }
                heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                x += size.x;
            }
            return y + heightToIncrease;
        }

        public static float CalculateHeight(IEnumerable<string> labels, float width, bool offsetScroll, System.Func<string, bool> filter) {
            var calculatedWidth = offsetScroll ? (width - 14) : width;
            var maxWidth = calculatedWidth;
            float x = 0;
            float y = 0;
            var heightToIncrease = 0f;
            foreach(var label in labels) {
                if(!filter(label))
                    continue;
                var size = CalculateSize(label);
                if(size.x + x > maxWidth) { // Move to next line
                    x = 0;
                    y += heightToIncrease;
                    heightToIncrease = 0;
                }
                heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                x += size.x;
            }
            return y + heightToIncrease;
        }

        public static float CalculateHeight(IEnumerable<LabelData> labels, float width, bool offsetScroll = true) {
            var calculatedWidth = offsetScroll ? (width - 14) : width;
            var maxWidth = calculatedWidth;
            float x = 0;
            float y = 0;
            var heightToIncrease = 0f;
            foreach(var label in labels) {
                var size = CalculateSize(label);
                if(size.x + x > maxWidth) { // Move to next line
                    x = 0;
                    y += heightToIncrease;
                    heightToIncrease = 0;
                }
                heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                x += size.x;
            }
            return y + heightToIncrease;
        }

        public static float CalculateHeight(IEnumerable<LabelData> labels, float width, bool offsetScroll, System.Func<LabelData, bool> filter) {
            var calculatedWidth = offsetScroll ? (width - 14) : width;
            var maxWidth = calculatedWidth;
            float x = 0;
            float y = 0;
            var heightToIncrease = 0f;
            foreach(var label in labels) {
                if(!filter(label))
                    continue;
                var size = CalculateSize(label);
                if(size.x + x > maxWidth) { // Move to next line
                    x = 0;
                    y += heightToIncrease;
                    heightToIncrease = 0;
                }
                heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                x += size.x;
            }
            return y + heightToIncrease;
        }

        #endregion

        // TODO: Stramline grids

        #region Grid Arrays

        public static void Grid(Rect position, IReadOnlyList<string> labels, IList<bool> active) {
            using(new GUI.GroupScope(position)) {
                var maxWidth = position.width;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    active[i] = Toggle(new Rect(x, y, size.x, size.y), label, active[i]);
                    x += size.x;
                }
            }
        }

        public static void Grid(Rect position, IReadOnlyList<string> labels, IList<bool> active, ref Vector2 scroll) {
            scroll = Grid(position, labels, active, scroll);
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<string> labels, IList<bool> active, Vector2 scroll) {
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    active[i] = Toggle(new Rect(x, y, size.x, size.y), label, active[i]);
                    x += size.x;
                }
            }
            return scroll;
        }

        public static void Grid(Rect position, IReadOnlyList<string> labels, System.Action<string> click, ref Vector2 scroll) {
            scroll = Grid(position, labels, click, scroll);
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<string> labels, System.Action<string> click, Vector2 scroll) {
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label)) {
                        click?.Invoke(label);
                    }
                    x += size.x;
                }
            }
            return scroll;
        }

        public static int Grid(Rect position, IReadOnlyList<string> labels, IList<bool> active, ref Vector2 scroll, System.Func<string, bool> filter) {
            int count = 0;
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true, filter);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    if(!filter(label))
                        continue;
                    count++;
                    var size = CalculateSize(label);
                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    active[i] = Toggle(new Rect(x, y, size.x, size.y), label, active[i]);
                    x += size.x;
                }
            }

            return count;
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<string> labels, IList<bool> active, Vector2 scroll, System.Func<string, bool> filter) {
            Grid(position, labels, active, ref scroll, filter);
            return scroll;
        }

        public static int Grid(Rect position, IReadOnlyList<string> labels, System.Action<string> click, ref Vector2 scroll, System.Func<string, bool> filter) {
            int count = 0;
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true, filter);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    if(!filter(label))
                        continue;
                    var size = CalculateSize(label);
                    count++;
                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label))
                        click?.Invoke(label);
                    x += size.x;
                }
            }
            return count;
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<string> labels, System.Action<string> click, Vector2 scroll, System.Func<string, bool> filter) {
            Grid(position, labels, click, ref scroll, filter);
            return scroll;
        }

        #endregion

        #region Grid Arrays (LabelData)

        public static void Grid(Rect position, IReadOnlyList<LabelData> labels, IList<bool> active) {
            using(new GUI.GroupScope(position)) {
                var maxWidth = position.width;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    active[i] = Toggle(new Rect(x, y, size.x, size.y), label, active[i]);
                    x += size.x;
                }
            }
        }

        public static void Grid(Rect position, IReadOnlyList<LabelData> labels, IList<bool> active, ref Vector2 scroll) {
            scroll = Grid(position, labels, active, scroll);
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<LabelData> labels, IList<bool> active, Vector2 scroll) {
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    active[i] = Toggle(new Rect(x, y, size.x, size.y), label, active[i]);
                    x += size.x;
                }
            }
            return scroll;
        }

        public static void Grid(Rect position, IReadOnlyList<LabelData> labels, System.Action<LabelData> click, ref Vector2 scroll) {
            scroll = Grid(position, labels, click, scroll);
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<LabelData> labels, System.Action<LabelData> click, Vector2 scroll) {
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label)) {
                        click?.Invoke(label);
                    }
                    x += size.x;
                }
            }
            return scroll;
        }

        public static int Grid(Rect position, IReadOnlyList<LabelData> labels, System.Action<LabelData> click, ref Vector2 scroll, System.Func<LabelData, bool> filter) {
            int count = 0;
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true, filter);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                for(int i = 0, len = labels.Count; i < len; i++) {
                    var label = labels[i];
                    if(!filter(label))
                        continue;
                    var size = CalculateSize(label);
                    count++;
                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label))
                        click?.Invoke(label);
                    x += size.x;
                }
            }
            return count;
        }

        public static Vector2 Grid(Rect position, IReadOnlyList<LabelData> labels, System.Action<LabelData> click, Vector2 scroll, System.Func<LabelData, bool> filter) {
            Grid(position, labels, click, ref scroll, filter);
            return scroll;
        }

        #endregion

        #region Grid Enumerable

        public static void Grid(Rect position, IEnumerable<string> labels, System.Action<string> click, ref Vector2 scroll) {
            scroll = Grid(position, labels, click, scroll);
        }

        public static Vector2 Grid(Rect position, IEnumerable<string> labels, System.Action<string> click, Vector2 scroll) {
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                foreach(var label in labels) {
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label)) {
                        click?.Invoke(label);
                    }
                    x += size.x;
                }
            }
            return scroll;
        }

        public static int Grid(Rect position, IEnumerable<string> labels, System.Action<string> click, ref Vector2 scroll, System.Func<string, bool> filter) {
            int count = 0;
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true, filter);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                foreach(var label in labels) {
                    if(!filter(label))
                        continue;
                    var size = CalculateSize(label);
                    count++;
                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label))
                        click?.Invoke(label);
                    x += size.x;
                }
            }
            return count;
        }

        public static Vector2 Grid(Rect position, IEnumerable<string> labels, System.Action<string> click, Vector2 scroll, System.Func<string, bool> filter) {
            Grid(position, labels, click, ref scroll, filter);
            return scroll;
        }

        #endregion

        #region Grid Enumerable (LabelData)

        public static void Grid(Rect position, IEnumerable<LabelData> labels, System.Action<LabelData> click, ref Vector2 scroll) {
            scroll = Grid(position, labels, click, scroll);
        }

        public static Vector2 Grid(Rect position, IEnumerable<LabelData> labels, System.Action<LabelData> click, Vector2 scroll) {
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                foreach(var label in labels) {
                    var size = CalculateSize(label);

                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label)) {
                        click?.Invoke(label);
                    }
                    x += size.x;
                }
            }
            return scroll;
        }

        public static int Grid(Rect position, IEnumerable<LabelData> labels, System.Action<LabelData> click, ref Vector2 scroll, System.Func<LabelData, bool> filter) {
            int count = 0;
            var calculatedWidth = position.width - 14;
            var internalScrollHeight = CalculateHeight(labels, position.width, true, filter);

            using(var scope = new GUI.ScrollViewScope(position, scroll, new Rect(0f, 0f, calculatedWidth, internalScrollHeight), false, true)) {
                scroll = scope.scrollPosition;
                var maxWidth = calculatedWidth;
                float x = 0;
                float y = 0;
                var heightToIncrease = 0f;
                foreach(var label in labels) {
                    if(!filter(label))
                        continue;
                    var size = CalculateSize(label);
                    count++;
                    if(size.x + x > maxWidth) { // Move to next line
                        x = 0;
                        y += heightToIncrease;
                        heightToIncrease = 0;
                    }

                    heightToIncrease = Mathf.Max(size.y, heightToIncrease);
                    if(Button(new Rect(x, y, size.x, size.y), label))
                        click?.Invoke(label);
                    x += size.x;
                }
            }
            return count;
        }

        public static Vector2 Grid(Rect position, IEnumerable<LabelData> labels, System.Action<LabelData> click, Vector2 scroll, System.Func<LabelData, bool> filter) {
            Grid(position, labels, click, ref scroll, filter);
            return scroll;
        }

        #endregion

        #region Toggle

        public static bool Toggle(Rect position, LabelData content, bool active) {
            if(Button(position, content.Name, active ? content.Color : LabelColor.Gray))
                active = !active;
            return active;
        }

        public static bool Toggle(Rect position, GUIContent content, bool active) {
            if(Button(position, content, active ? LabelColor.Blue : LabelColor.Gray))
                active = !active;
            return active;
        }

        public static bool Toggle(Rect position, string text, bool active) {
            if(Button(position, text, active ? LabelColor.Blue : LabelColor.Gray))
                active = !active;
            return active;
        }

        public static bool Toggle(Rect position, GUIContent content, bool active, LabelColor activeColor = LabelColor.Blue, LabelColor inactiveColor = LabelColor.Gray) {
            if(Button(position, content, active ? activeColor : inactiveColor))
                active = !active;
            return active;
        }

        public static bool Toggle(Rect position, string text, bool active, LabelColor activeColor = LabelColor.Blue, LabelColor inactiveColor = LabelColor.Gray) {
            if(Button(position, text, active ? activeColor : inactiveColor))
                active = !active;
            return active;
        }


        public static bool ToggleLayout(GUIContent content, bool active) {
            if(ButtonLayout(content, active ? LabelColor.Blue : LabelColor.Gray))
                active = !active;
            return active;
        }

        public static bool ToggleLayout(string text, bool active) {
            if(ButtonLayout(text, active ? LabelColor.Blue : LabelColor.Gray))
                active = !active;
            return active;
        }

        public static bool ToggleLayout(GUIContent content, bool active, LabelColor activeColor = LabelColor.Blue, LabelColor inactiveColor = LabelColor.Gray) {
            if(ButtonLayout(content, active ? activeColor : inactiveColor))
                active = !active;
            return active;
        }

        public static bool ToggleLayout(string text, bool active, LabelColor activeColor = LabelColor.Blue, LabelColor inactiveColor = LabelColor.Gray) {
            if(ButtonLayout(text, active ? activeColor : inactiveColor))
                active = !active;
            return active;
        }

        #endregion

        #region Button

        public static bool Button(Rect position, LabelData label) {
            return Button(position, label.Name, label.Color);
        }

        public static bool Button(Rect position, GUIContent content) {
            return DoButton(position, content, Styles.Gray);
        }

        public static bool Button(Rect position, GUIContent content, LabelColor color) {
            return DoButton(position, content, Styles.GetStyle(color));
        }

        public static bool Button(Rect position, string text) {
            temp.text = text;
            temp.tooltip = string.Empty;
            temp.image = null;
            return DoButton(position, temp, Styles.Gray);
        }

        public static bool Button(Rect position, string text, LabelColor color) {
            temp.text = text;
            temp.tooltip = string.Empty;
            temp.image = null;
            return DoButton(position, temp, Styles.GetStyle(color));
        }

        public static bool ButtonLayout(GUIContent content, params GUILayoutOption[] options)
            => ButtonLayout(content, LabelColor.Gray, options);

        public static bool ButtonLayout(GUIContent content, LabelColor color, params GUILayoutOption[] options) {
            var style = Styles.GetStyle(color);
            var position = GUILayoutUtility.GetRect(content, style, options);
            return DoButton(position, content, style);
        }

        public static bool ButtonLayout(string text, params GUILayoutOption[] options)
            => ButtonLayout(text, LabelColor.Gray, options);

        public static bool ButtonLayout(string text, LabelColor color, params GUILayoutOption[] options) {
            temp.text = text;
            temp.tooltip = string.Empty;
            temp.image = null;
            var style = Styles.GetStyle(color);
            var position = GUILayoutUtility.GetRect(temp, style, options);
            return DoButton(position, temp, style);
        }

        #endregion

        #region Button Low-Level Implementation

        private static GUIContent temp = new GUIContent();

        private static System.Reflection.MethodInfo grabMouseControlMethod;
        private static System.Reflection.MethodInfo hasMouseControlMethod;
        private static System.Reflection.MethodInfo releaseMouseControlMethod;
        private static readonly int labelsDrawerHash = "labelsDrawer".GetHash32();

        private static bool DoButton(Rect position, GUIContent content, GUIStyle style) {
            int id = GUIUtility.GetControlID(labelsDrawerHash, FocusType.Passive, position);
            Event current = Event.current;
            var hover = position.Contains(current.mousePosition);
            switch(current.type) {
                case EventType.Repaint:
                    var col = GUI.backgroundColor;
                    UnityInternalUtility.TryCacheMethod(typeof(GUI), "HasMouseControl", ref hasMouseControlMethod);
                    if(hasMouseControlMethod.InvokeStatic<int, bool>(id))
                        GUI.backgroundColor = col.MultiplyRGB(0.7f);
                    else if(hover)
                        GUI.backgroundColor = col.MultiplyRGB(0.9f);

                    style.Draw(position, content, id, false, hover);
                    GUI.backgroundColor = col;
                    break;
                case EventType.MouseDown:
                    if(position.Contains(current.mousePosition)) {
                        UnityInternalUtility.TryCacheMethod(typeof(GUI), "GrabMouseControl", ref grabMouseControlMethod);
                        grabMouseControlMethod.InvokeStatic(id);
                        //GrabMouseControl(id);
                        current.Use();
                    }

                    break;
                case EventType.KeyDown: {
                        bool flag = current.alt || current.shift || current.command || current.control;
                        if((current.keyCode == KeyCode.Space || current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter) && !flag && GUIUtility.keyboardControl == id) {
                            current.Use();
                            GUI.changed = true;
                            return true;
                        }

                        break;
                    }
                case EventType.MouseUp:
                    UnityInternalUtility.TryCacheMethod(typeof(GUI), "HasMouseControl", ref hasMouseControlMethod);
                    if(hasMouseControlMethod.InvokeStatic<int, bool>(id)) { //  HasMouseControl(id)
                        UnityInternalUtility.TryCacheMethod(typeof(GUI), "ReleaseMouseControl", ref releaseMouseControlMethod);
                        releaseMouseControlMethod.InvokeStatic(); //ReleaseMouseControl();
                        current.Use();
                        if(position.Contains(current.mousePosition)) {
                            GUI.changed = true;
                            return true;
                        }
                    }

                    break;
                case EventType.MouseDrag:
                    UnityInternalUtility.TryCacheMethod(typeof(GUI), "HasMouseControl", ref hasMouseControlMethod);
                    if(hasMouseControlMethod.InvokeStatic<int, bool>(id)) { //  HasMouseControl(id)
                        current.Use();
                    }

                    break;
            }

            return false;
        }

        #endregion
    }
}
