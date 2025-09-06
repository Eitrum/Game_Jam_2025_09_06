using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(EditLockAttribute))]
    public class EditLockAttributeDrawer : PropertyDrawer {

        private class Styles {
            public static GUIContent Unlocked = EditorGUIUtility.TrIconContent("IN LockButton");
            public static GUIContent Locked = EditorGUIUtility.TrIconContent("IN LockButton On");
            public static GUIStyle LockButton = new GUIStyle(GUI.skin.button);

            static Styles() {
                LockButton.padding = new RectOffset();
            }
        }

        private bool locked = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var singleline = EditorGUIUtility.singleLineHeight;
            var lockarea = new Rect(position);
            lockarea.width = singleline;
            lockarea.height = singleline;
            lockarea.x -= singleline;

            if(GUI.Button(lockarea, locked ? Styles.Locked : Styles.Unlocked, Styles.LockButton))
                locked = !locked;

            using(new EditorGUI.DisabledScope(locked))
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
