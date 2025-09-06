using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Health
{
    public static class HealthEditorUtility
    {
        #region Debug

        public static void DrawHealthComponentReference<T>(T component, bool searchInParents = true) where T : Component {
            var h = searchInParents ? component.GetComponentInParent<IHealth>() : component.GetComponent<IHealth>();
            if(h == null) {
                using(new EditorGUILayout.VerticalScope("box"))
                    EditorGUILayout.HelpBox("Missing Health Component!", MessageType.Error);
            }
            else if(h is UnityEngine.Object obj) {
                using(new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField("Health Component", obj, typeof(IHealth), true);
            }
        }

        #endregion
    }
}
