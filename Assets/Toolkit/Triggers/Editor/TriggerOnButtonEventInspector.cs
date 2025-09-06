using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Toolkit.UI;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(TriggerOnButtonEvent))]
    public class TriggerOnButtonEventInspector : Editor
    {
        #region Variables

        private SerializedProperty repeatable;

        #endregion

        #region Init

        private void OnEnable() {
            repeatable = serializedObject.FindProperty("repeatable");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                CheckForButtonComponents(target as Component);
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(repeatable);
                }

                TriggerEditorUtility.DrawEditorDebug(this, target);
            }
        }

        private static void CheckForButtonComponents(Component c) {
            var ibutton = c.GetComponentInParent<IButton>();
            var ubutton = c.GetComponentInParent<UnityEngine.UI.Button>();
            if(ibutton == null && ubutton == null) {
                EditorGUILayout.HelpBox("Missing button component in parents", MessageType.Warning);
            }
        }

        #endregion
    }
}
