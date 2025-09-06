using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    [CustomEditor(typeof(PanelStorageRegister))]
    public class PanelStorageRegisterInspector : Editor {


        public override void OnInspectorGUI() {
            using(var scope = new ToolkitEditorUtility.InspectorScope(this)) {
                scope.DrawAll();
            }
        }
    }

    [CustomPropertyDrawer(typeof(PanelStorageRegister.Entry))]
    public class PanelStorageRegisterEntryPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var nameProp = property.FindPropertyRelative("name");
            var prefabProp = property.FindPropertyRelative("panelPrefab");

            position.SplitHorizontal(out var nameFieldArea, out var prefabArea, 0.3f, 5);
            EditorGUI.PropertyField(nameFieldArea, nameProp, GUIContent.none);
            EditorGUI.PropertyField(prefabArea, prefabProp, GUIContent.none);

            if(string.IsNullOrEmpty(nameProp.stringValue) && prefabProp.objectReferenceValue) {
                EditorGUI.LabelField(nameFieldArea.Pad(2f, 2f, 1, 1), prefabProp.objectReferenceValue.name, EditorStylesUtility.GrayItalicLabel);
            }
        }
    }
}
