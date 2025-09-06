using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    public class NSOPreviewWindow : PopupWindowContent
    {
        #region Variables

        private const float DEFAULT_HEIGHT = 300f;
        private const float DEFAULT_WIDTH = 150f;

        private Editor editor;
        private GUIContent header;
        private Type type;
        private float width;
        private float height;

        public UnityEngine.Object Object { get; private set; }
        public int Depth { get; private set; }

        #endregion

        #region Constructor

        public NSOPreviewWindow(UnityEngine.Object o) : this(o, EditorGUI.indentLevel) { }

        public NSOPreviewWindow(UnityEngine.Object o, int depth) : this(o, depth, DEFAULT_WIDTH) { }

        public NSOPreviewWindow(UnityEngine.Object o, int depth, float width) {
            Object = o;
            Depth = depth;
            type = o.GetType();
            this.width = width;
            this.height = DEFAULT_HEIGHT;
        }

        #endregion

        #region Init

        public override void OnClose() {
            if(editor)
                Editor.DestroyImmediate(editor);
        }

        public override void OnOpen() {
            Editor.CreateCachedEditor(Object, null, ref editor);
            header = EditorGUIUtility.TrTextContent($"Preview - {type.Name}");
        }

        #endregion

        public override Vector2 GetWindowSize() {
            return new Vector2(width, height);
        }

        public override void OnGUI(Rect rect) {
            DrawBorders(rect);
            rect = NSOUtility.Pad(rect, 6, 6, 6, 6);
            NSOUtility.DrawEdges(rect, Depth);
            GUILayout.BeginArea(rect);
            DrawHeader();
            if(editor)
                editor.OnInspectorGUI();
            GUILayout.EndArea();
        }

        private void DrawHeader() {
            EditorGUILayout.LabelField(header, EditorStyles.centeredGreyMiniLabel);
            var headerArea = GUILayoutUtility.GetLastRect();

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && headerArea.Contains(ev.mousePosition)) {

            }
        }

        private void DrawBorders(Rect rect) {
            EditorGUI.DrawRect(rect, new Color(0.05f, 0.05f, 0.05f, 1f));
            EditorGUI.DrawRect(rect.Pad(5, 5, 5, 5), new Color32(56, 56, 56, 255));
        }
    }
}
