using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Toolkit.QuickInspect {
    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class ScriptableObjectReferencePropertyDrawer : PropertyDrawer {

        #region Nesting Fixes

        private static int recursionLimit => QuickInspectSettings.RecursionLimit;

        private static HashSet<ScriptableObject> alreadyRendering = new HashSet<ScriptableObject>();
        private static int myNesting = 0;

        private struct Nesting : System.IDisposable {

            private int previousNesting;
            private ScriptableObject scriptableObject;

            public static Nesting Create(SerializedProperty property) {
                var n = new Nesting();
                n.previousNesting = myNesting++;
                n.scriptableObject = property.objectReferenceValue as ScriptableObject;
                alreadyRendering.Add(n.scriptableObject);
                return n;
            }

            public void Dispose() {
                myNesting = previousNesting;
                alreadyRendering.Remove(scriptableObject);
            }
        }

        #endregion

        #region Height Calc

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var result = 0f;

            if(property.objectReferenceValue == property.serializedObject.targetObject)
                return EditorGUIUtility.singleLineHeight;

            if(property.objectReferenceValue == null || !property.isExpanded || !QuickInspectSettings.Enabled)
                return EditorGUIUtility.singleLineHeight;

            if(myNesting < recursionLimit)
                QuickInspectUtil.SetNestingLevel(property, 0);
            using(Nesting.Create(property)) {
                using(SerializedObject so = new SerializedObject(property.objectReferenceValue)) {
                    var iterator = so.GetIterator();
                    if(!iterator.NextVisible(true))
                        return EditorGUIUtility.singleLineHeight;


                    if(iterator.name.Equals("m_Script"))
                        iterator.NextVisible(false);

                    do {
                        result += EditorGUI.GetPropertyHeight(iterator, true);
                    } while(iterator.NextVisible(false));
                }
            }
            return result + 8 + EditorGUIUtility.singleLineHeight;
        }

        #endregion

        #region Draw 

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(QuickInspectSettings.RenderShadow)
                EditorGUI.DrawRect(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight), new Color(0.1f, 0.1f, 0.1f, 0.1f));
            var topArea = new Rect(position.x, position.y, position.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            // Handle object not drawn
            if(property.objectReferenceValue == null || (property.objectReferenceValue == property.serializedObject.targetObject) || !QuickInspectSettings.Enabled) {
                EditorGUI.PropertyField(topArea, property, label);
                if(property.objectReferenceValue == null)
                    DrawAdd(new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height), property);
                else
                    DrawRemove(new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height), property);
                return;
            }

            EditorGUI.ObjectField(topArea, property, label);
            property.isExpanded = EditorGUI.Foldout(topArea, property.isExpanded, label, true);
            bool didRemove = DrawRemove(new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height), property);

            if(!property.isExpanded || didRemove)
                return;

            if(myNesting < recursionLimit)
                QuickInspectUtil.SetNestingLevel(property, 0);
            using(Nesting.Create(property))
                DrawShowcase(position, property);
        }

        private void DrawShowcase(Rect position, SerializedProperty property) {
            using(new EditorGUI.DisabledScope(!QuickInspectSettings.AllowEdit)) {
                var color = QuickInspectUtil.GetColor(EditorGUI.indentLevel);
                EditorGUI.indentLevel++;

                position.height -= 4;
                position.x += 4f;
                position.width -= 4f;
                if(QuickInspectSettings.RenderColor) {
                    var outline = EditorGUI.IndentedRect(position);
                    EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + EditorGUIUtility.singleLineHeight, 2f, outline.height - EditorGUIUtility.singleLineHeight), color);
                    EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + outline.height - 2f, outline.width, 2f), color);
                }

                var posy = position.y + EditorGUIUtility.singleLineHeight;
                using(var so = new SerializedObject(property.objectReferenceValue)) {
                    so.UpdateIfRequiredOrScript();
                    var iterator = so.GetIterator();
                    if(!iterator.NextVisible(true))
                        return;

                    if(iterator.name.Equals("m_Script"))
                        iterator.NextVisible(false);

                    do {
                        var height = EditorGUI.GetPropertyHeight(iterator, true);
                        var area = new Rect(position.x, posy, position.width, height);
                        EditorGUI.PropertyField(area, iterator, true);
                        posy += height;
                    } while(iterator.NextVisible(false));
                    so.ApplyModifiedProperties();
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawAdd(Rect addArea, SerializedProperty property) {
            if(typeof(ScriptableObject) == fieldInfo.FieldType || fieldInfo.FieldType.IsAbstract)
                return;

            GUI.Label(addArea, EditorGUIUtility.IconContent("d_ol_plus"));

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && addArea.Contains(ev.mousePosition) && (!QuickInspectSettings.DialogueCreateAsset || EditorUtility.DisplayDialog("Create", $"Create a new '{fieldInfo.FieldType.Name}' asset.", "Yes", "Cancel"))) {
                var so = ScriptableObject.CreateInstance(fieldInfo.FieldType);
                var upath = AssetDatabase.GenerateUniqueAssetPath($"Assets/new {fieldInfo.FieldType.Name}.asset");
                ProjectWindowUtil.CreateAsset(so, upath);
                property.objectReferenceValue = so;
            }
        }

        private bool DrawRemove(Rect clearArea, SerializedProperty property) {
            GUI.Label(clearArea, EditorGUIUtility.IconContent("d_ol_minus"));

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && clearArea.Contains(ev.mousePosition)) {
                property.objectReferenceValue = null;
                return true;
            }
            return false;
        }

        #endregion
    }
}
