using UnityEditor;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    [CustomPropertyDrawer(typeof(NavMeshBuildRuleMask))]
    public class NavMeshBuildRuleMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.SplitHorizontal(out Rect defaultRect, out Rect newButtonRect, (position.width - 40f) / position.width);
            EditorGUI.ObjectField(defaultRect, property);
            if(GUI.Button(newButtonRect, "New")) {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                var newMaskObject = ScriptableObject.CreateInstance<NavMeshBuildRuleMask>();
                ProjectWindowUtil.CreateAsset(newMaskObject, AssetDatabase.GenerateUniqueAssetPath(path + "new mask.asset"));
                property.objectReferenceValue = newMaskObject;
            }
        }
    }

    [CustomEditor(typeof(NavMeshBuildRuleMask))]
    public class NavMeshBuildRuleMaskEditor : Editor
    {
        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Rules", EditorStyles.boldLabel);
            var defaultProp = serializedObject.FindProperty("Default");

            using(new EditorGUILayout.VerticalScope("box")) {
                do {
                    EditorGUILayout.PropertyField(defaultProp);
                } while(defaultProp.NextVisible(false));
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
