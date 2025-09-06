using UnityEngine;
using UnityEditor;

namespace Toolkit.Mathematics
{
    [CustomPropertyDrawer(typeof(Arc))]
    public class ArcInspector : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 14f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var previewHeight = position.height - EditorGUIUtility.singleLineHeight * 4f;

            var px = property.FindPropertyRelative("px");
            var py = property.FindPropertyRelative("py");
            var pz = property.FindPropertyRelative("pz");

            var dx = property.FindPropertyRelative("dx");
            var dy = property.FindPropertyRelative("dy");
            var dz = property.FindPropertyRelative("dz");

            var gx = property.FindPropertyRelative("gx");
            var gy = property.FindPropertyRelative("gy");
            var gz = property.FindPropertyRelative("gz");

            var expanded = property.isExpanded;

            position.height = EditorGUIUtility.singleLineHeight;
            expanded = EditorGUI.Foldout(position, expanded, label, true);

            property.isExpanded = expanded;
            if(expanded) {
                GUI.Box(position.Pad(0, 0, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight * -3f), "");
                using(new EditorGUI.IndentLevelScope(1)) {
                    EditorGUI.BeginChangeCheck();
                    position.y += EditorGUIUtility.singleLineHeight;
                    var positionResult = EditorGUI.Vector3Field(position, "Position", new Vector3(px.floatValue, py.floatValue, pz.floatValue));
                    if(EditorGUI.EndChangeCheck()) {
                        px.floatValue = positionResult.x;
                        py.floatValue = positionResult.y;
                        pz.floatValue = positionResult.z;
                    }

                    EditorGUI.BeginChangeCheck();
                    position.y += EditorGUIUtility.singleLineHeight;
                    var deltaResult = EditorGUI.Vector3Field(position, "Velocity", new Vector3(dx.floatValue, dy.floatValue, dz.floatValue));
                    if(EditorGUI.EndChangeCheck()) {
                        dx.floatValue = deltaResult.x;
                        dy.floatValue = deltaResult.y;
                        dz.floatValue = deltaResult.z;
                    }

                    EditorGUI.BeginChangeCheck();
                    position.y += EditorGUIUtility.singleLineHeight;
                    var gravity = new Vector3(gx.floatValue, gy.floatValue, gz.floatValue);
                    var isDefault = gravity.Equals(Physics.gravity, Mathf.Epsilon);
                    var gravityResult = EditorGUI.Vector3Field(position, isDefault ? "Gravity (default)" : "Gravity", gravity);
                    if(EditorGUI.EndChangeCheck()) {
                        gx.floatValue = gravityResult.x;
                        gy.floatValue = gravityResult.y;
                        gz.floatValue = gravityResult.z;
                    }
                    position.y += EditorGUIUtility.singleLineHeight;
                    position.height = previewHeight;
                    ArcEditor.Draw2DPreview(position, new Arc(positionResult, deltaResult, gravityResult));
                }
            }
        }
    }
}
