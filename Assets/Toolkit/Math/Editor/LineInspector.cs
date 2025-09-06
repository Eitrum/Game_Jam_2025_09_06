using UnityEngine;
using UnityEditor;

namespace Toolkit.Mathematics
{
    [CustomPropertyDrawer(typeof(Line))]
    public class LineInspector : PropertyDrawer
    {
        private static Mode mode;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 3f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var px = property.FindPropertyRelative("px");
            var py = property.FindPropertyRelative("py");
            var pz = property.FindPropertyRelative("pz");

            var dx = property.FindPropertyRelative("dx");
            var dy = property.FindPropertyRelative("dy");
            var dz = property.FindPropertyRelative("dz");

            position.height = EditorGUIUtility.singleLineHeight;
            position.SplitHorizontal(out Rect foldoutArea, out Rect modeArea, (position.width - 180f) / position.width, 2f);

            property.isExpanded = EditorGUI.Foldout(property.isExpanded ? foldoutArea : position, property.isExpanded, label, true);
            if(property.isExpanded) {
                mode = (Mode)EditorGUI.EnumPopup(modeArea, mode);
                var pos = new Vector3(px.floatValue, py.floatValue, pz.floatValue);
                var vel = new Vector3(dx.floatValue, dy.floatValue, dz.floatValue);
                var end = pos + vel;

                using(new EditorGUI.IndentLevelScope(1)) {
                    // Origin
                    EditorGUI.BeginChangeCheck();
                    position.y += EditorGUIUtility.singleLineHeight;
                    var positionResult = EditorGUI.Vector3Field(position, "Origin", pos);
                    if(EditorGUI.EndChangeCheck()) {
                        px.floatValue = positionResult.x;
                        py.floatValue = positionResult.y;
                        pz.floatValue = positionResult.z;
                    }

                    // Velocity
                    position.y += EditorGUIUtility.singleLineHeight;
                    Vector3 velRes = new Vector3();
                    switch(mode) {
                        case Mode.Velocity:
                            velRes = EditorGUI.Vector3Field(position, "Velocity", vel);
                            break;
                        case Mode.Destination:
                            end = EditorGUI.Vector3Field(position, "Destination", end);
                            velRes = end - positionResult;
                            break;
                    }
                    if(!vel.Equals(velRes, Mathf.Epsilon)) {
                        dx.floatValue = velRes.x;
                        dy.floatValue = velRes.y;
                        dz.floatValue = velRes.z;
                    }
                }
            }
        }

        public enum Mode
        {
            [InspectorName("Origin + Velocity")]
            Velocity,
            [InspectorName("Origin + Destination")]
            Destination
        }
    }
}
