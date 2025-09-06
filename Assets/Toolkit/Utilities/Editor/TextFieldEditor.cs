using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(TextField))]
    public class TextFieldEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            var refProperty = property.FindPropertyRelative("reference");
            var newObj = EditorGUI.ObjectField(position, label, refProperty.objectReferenceValue, typeof(UnityEngine.Object), true);

            if(EditorGUI.EndChangeCheck()) {
                TextField tf = default;
                if(newObj is GameObject go) {
                    tf = TextField.FindInChildren(go.transform);
                }
                else if(newObj is Component com) {
                    tf = TextField.FindInChildren(com);
                }

                if(tf.TextType != TextType.None) {
                    refProperty.objectReferenceValue = tf.Reference;
                }
                else {
                    refProperty.objectReferenceValue = null;
                }
            }
        }

        //[InitializeOnLoadMethod]
        //private static void CheckForTMP() {
        //    var list = UnityEditor.PackageManager.Client.List(true);
        //    EditorApplication.delayCall += () => {
        //        bool hasTMP = false;
        //        if(list.Status != UnityEditor.PackageManager.StatusCode.Success)
        //            return;
        //        list.Result.Foreach(x => { if((x?.packageId?.StartsWith("com.unity.textmeshpro") ?? false)) hasTMP = true; });
        //        var selected = EditorUserBuildSettings.selectedBuildTargetGroup;
        //        PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(selected), out string[] defines);
        //        var groupContainsTMP = defines.Any(x => x == "TEXTMESHPRO");
        //
        //        if(hasTMP && !groupContainsTMP) {
        //            List<string> temp = new List<string>(defines);
        //            temp.Add("TEXTMESHPRO");
        //            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(selected), temp.ToArray());
        //        }
        //        else if(!hasTMP && groupContainsTMP) {
        //            List<string> temp = new List<string>(defines);
        //            temp.Remove("TEXTMESHPRO");
        //            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(selected), temp.ToArray());
        //        }
        //    };
        //}
    }
}
