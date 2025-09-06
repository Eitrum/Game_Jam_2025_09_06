using UnityEditor;
using UnityEngine;

namespace Toolkit.InputSystem {
    [CustomEditor(typeof(CursorObject))]
    public class CursorObjectInspector : Editor {

        private class Styles {
            public static readonly Texture checkerTexture = EditorGUIUtility.TrIconContent("textureCheckerDark").image;
            public static readonly Rect checkerTextureUV;

            public static readonly Texture hotSpot = EditorGUIUtility.TrIconContent("d_Record On").image;

            static Styles() {
                checkerTextureUV = new Rect(0, 0, 16, 16);
            }
        }

        private SerializedProperty texture;
        private SerializedProperty hotSpot;

        private void OnEnable() {
            texture = serializedObject.FindProperty("texture");
            hotSpot = serializedObject.FindProperty("hotSpotNormalized");
        }

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(texture);
                EditorGUILayout.PropertyField(hotSpot);

                var hotspotvalue = hotSpot.vector2Value;
                hotspotvalue = new Vector2(Mathf.Clamp01(hotspotvalue.x), Mathf.Clamp01(hotspotvalue.y));
                hotSpot.vector2Value = hotspotvalue;

                DrawTextureArea();
            }
        }

        private void DrawTextureArea() {
            var width = Screen.width;
            var area = GUILayoutUtility.GetRect(width, width);
            GUI.DrawTextureWithTexCoords(area, Styles.checkerTexture, Styles.checkerTextureUV);

            var cursorTexture = texture.objectReferenceValue as Texture2D;

            if(cursorTexture != null) {
                GUI.DrawTexture(area, cursorTexture);
            }

            var point = Rect.NormalizedToPoint(area, hotSpot.vector2Value);
            var hotSpotArea = new Rect(point.x - 8, point.y -8, 16, 16);
            GUI.DrawTexture(hotSpotArea, Styles.hotSpot);

            var ev = Event.current;

            if(ev.type != EventType.Repaint) {
                if(ev.isMouse && ev.button == 0 && area.Contains(ev.mousePosition)) {
                    hotSpot.vector2Value = Rect.PointToNormalized(area, ev.mousePosition);
                    ev.Use();
                    Repaint();
                }
            }

            switch(ev.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if(!area.Contains(ev.mousePosition))
                        return;

                    if(DragAndDrop.visualMode == DragAndDropVisualMode.None) {
                        hotSpot.vector2Value = Rect.PointToNormalized(area, ev.mousePosition);
                    }

                    if(DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length != 1)
                        return;

                    var mainObj = DragAndDrop.objectReferences[0];
                    if(!(mainObj is Texture2D dragedTexture))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if(ev.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        texture.objectReferenceValue = dragedTexture;
                    }
                    ev.Use();
                    break;
            }
        }
    }
}
