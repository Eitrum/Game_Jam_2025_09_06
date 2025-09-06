using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(AttachmentPointsBehaviour))]
    public class AttachmentPointsBehaviourEditor : Editor
    {

        private GUIStyle style;
        private static AttachmentPoint[] list;
        private static string[] propNames;
        private static GUIContent[] formattedName;

        private void OnEnable() {
            if(list == null) {
                list = AttachmentPoint.None.GetArray();
                propNames = list.Select(x => x.ToString().ToLower()).ToArray();
                formattedName = list.Select(x => new GUIContent($"{AttachmentPointsUtility.ToString(x)} <color=#87878787>{(AttachmentPointsUtility.IsEquipmentSlot(x) ? "Equipment" : "Custom")}</color>")).ToArray();
            }
        }

        public override void OnInspectorGUI() {
            if(style == null) {
                style = new GUIStyle(EditorStyles.label);
                style.richText = true;
            }
            var attachments = target as IAttachmentPoints;
            for(int i = 1, length = list.Length; i < length; i++) {
                var a = list[i];
                var prop = serializedObject.FindProperty(propNames[i]);
                using(new EditorGUILayout.HorizontalScope("box")) {
                    EditorGUI.BeginChangeCheck();
                    var temp = EditorGUILayout.ObjectField(" ", prop.objectReferenceValue, typeof(Transform), true) as Transform;
                    var rect = GUILayoutUtility.GetLastRect();
                    EditorGUI.LabelField(rect, formattedName[i], style);

                    if(EditorGUI.EndChangeCheck()) {
                        prop.objectReferenceValue = temp;
                    }
                }
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnSceneGUI() {
            var attachments = target as IAttachmentPoints;
            for(int i = 1, length = list.Length; i < length; i++) {
                var a = list[i];
                var t = attachments.GetAttachment(a);
                if(t != null) {
                    t.position = Handles.PositionHandle(t.position, t.rotation);
                    t.rotation = Handles.RotationHandle(t.rotation, t.position);
                }
            }
        }
    }
}
