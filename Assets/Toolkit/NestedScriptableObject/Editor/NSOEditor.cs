using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public static class NSOEditor {
        #region Nesting Fixes // To override Unity's Internal rendering tech to allow nested object renderering.

        private static int recursionLimit => 4;

        private static int myNesting = 0;

        private struct Nesting : System.IDisposable {

            private int previousNesting;
            private ScriptableObject scriptableObject;

            public static Nesting Create(SerializedProperty property) {
                var n = new Nesting();
                n.previousNesting = myNesting++;
                n.scriptableObject = property.objectReferenceValue as ScriptableObject;
                return n;
            }

            public void Dispose() {
                myNesting = previousNesting;
            }
        }

        public static void SetNestingLevel(SerializedProperty property, int value) {
            var handler = ScriptAttributeUtility.GetHandler(property);
            UnityInternalUtility.TrySetValue(handler, "m_NestingLevel", value);
        }

        #endregion

        #region EditorGUI Variant

        public static float CalculateHeight(SerializedProperty property) {
            if(NSOEditorSettings.Mode != NSOMode.NestedRendering) // Both default vays will always be 1 line rendered.
                return EditorGUIUtility.singleLineHeight;

            if(property.propertyType != SerializedPropertyType.ObjectReference) {
                if(property.propertyType == SerializedPropertyType.Generic) {
                    var child = property.Copy();
                    child.NextVisible(true);
                    property = child;
                }

                if(property.propertyType != SerializedPropertyType.ObjectReference) {
                    return EditorGUIUtility.singleLineHeight;
                }
            }

            if(property.objectReferenceValue == null || !property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            var result = 0f;

            if(myNesting < recursionLimit) // This is to fix Unity internal rendering
                SetNestingLevel(property, 0);
            using(Nesting.Create(property)) {
                using(SerializedObject so = new SerializedObject(property.objectReferenceValue)) {
                    var iterator = so.GetIterator();
                    if(!iterator.NextVisible(true))
                        return EditorGUIUtility.singleLineHeight;


                    if(iterator.name.Equals("m_Script") && !iterator.NextVisible(false))
                        return EditorGUIUtility.singleLineHeight;

                    do {
                        result += EditorGUI.GetPropertyHeight(iterator, true);
                    } while(iterator.NextVisible(false));
                }
            }
            return result + 8 + EditorGUIUtility.singleLineHeight;
        }

        public static void DrawProperty(Rect position, SerializedProperty property, GUIContent label, System.Type type) {
            if(type == null || type == typeof(ScriptableObject)) {
                EditorGUI.HelpBox(position, $"variable '{label?.text}' is missing a type", MessageType.Error);
                return;
            }

            if(property.propertyType != SerializedPropertyType.ObjectReference) {
                if(property.propertyType == SerializedPropertyType.Generic) {
                    var child = property.Copy();
                    child.NextVisible(true);
                    property = child;
                }

                if(property.propertyType != SerializedPropertyType.ObjectReference) {
                    EditorGUI.HelpBox(position, $"variable '{label?.text}' need to be of either 'ScriptableObject' or 'UnityEngine.Object'", MessageType.Error);
                    return;
                }
            }

            DrawProperty_Internal(position, property, label?.text, type);
        }

        public static void DrawObjectField(Rect position, SerializedProperty property, string label, System.Type type) {
            if(NSOEditorSettings.Mode == NSOMode.Default) {
                DrawProperty_Default_Internal(position, property, label, type);
                return;
            }

            if(property.objectReferenceValue == null) {
                var newIndex = EditorGUI.Popup(position, string.IsNullOrEmpty(label) ? "reference" : label, 0, NSODatabase.GetPopupList(type));
                if(newIndex > 0) {
                    var t = NSODatabase.Find(type)[newIndex - 1];

                    var obj = ScriptableObject.CreateInstance(t.Type);
                    obj.name = t.Name;
                    AssetDatabase.AddObjectToAsset(obj, property.serializedObject.targetObject);
                    property.objectReferenceValue = obj;
                    EditorApplication.delayCall += AssetDatabase.SaveAssets;
                }
            }
            else {
                var objRenderArea = new Rect(position.x, position.y, position.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                var removeObjArea = new Rect(objRenderArea.x + objRenderArea.width, objRenderArea.y, objRenderArea.height, objRenderArea.height);

                using(new EditorGUI.DisabledScope(true))
                    EditorGUI.ObjectField(objRenderArea, label, property.objectReferenceValue, type, false);

                if(NSOEditorSettings.Mode == NSOMode.NestedRendering)
                    property.isExpanded = EditorGUI.Foldout(objRenderArea, property.isExpanded, label, true);

                GUI.Label(removeObjArea, EditorGUIUtility.IconContent("d_ol_minus"));

                var ev = Event.current;
                if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && removeObjArea.Contains(ev.mousePosition)) {
                    NSOUtility.VerifyAndDestroy(property);
                    try {
                        property.objectReferenceValue = null;
                    }
                    catch {
                    }
                }
            }
        }

        private static void DrawProperty_Internal(Rect position, SerializedProperty property, string label, System.Type type) {
            if(property.propertyType != SerializedPropertyType.ObjectReference) {
                EditorGUI.HelpBox(position, $"variable need to be of a correct type", MessageType.Error);
                return;
            }

            DrawObjectField(position, property, label, type);

            if(property.objectReferenceValue != null && NSOEditorSettings.Mode == NSOMode.NestedRendering && property.isExpanded) {
                if(myNesting < recursionLimit) // This is to fix Unity internal rendering
                    SetNestingLevel(property, 0);
                using(Nesting.Create(property))
                    DrawPreview_Internal(position, property);
            }
        }

        private static void DrawPreview_Internal(Rect position, SerializedProperty property) {
            var color = NSOUtility.GetColor(myNesting-1);
            using(new EditorGUI.IndentLevelScope(1)) {
                position.height -= 4;
                position.x += 4f;
                position.width -= 4f;

                var outline = EditorGUI.IndentedRect(position);
                EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + EditorGUIUtility.singleLineHeight, 2f, outline.height - EditorGUIUtility.singleLineHeight), color);
                EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + outline.height - 2f, outline.width, 2f), color);

                var posy = position.y + EditorGUIUtility.singleLineHeight;
                using(var so = new SerializedObject(property.objectReferenceValue)) {
                    so.UpdateIfRequiredOrScript();
                    var iterator = so.GetIterator();
                    if(!iterator.NextVisible(true))
                        return;

                    if(iterator.name.Equals("m_Script") && !iterator.NextVisible(false))
                        return;

                    do {
                        var copy = iterator.Copy();
                        var height = EditorGUI.GetPropertyHeight(copy, true);
                        var area = new Rect(position.x, posy, position.width, height);
                        EditorGUI.PropertyField(area, copy, true);
                        posy += height;
                    } while(iterator.NextVisible(false));
                    so.ApplyModifiedProperties();
                }
            }
        }

        private static void DrawProperty_Default_Internal(Rect position, SerializedProperty property, string label, System.Type type) {

            var objRenderArea = new Rect(position.x, position.y, position.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            property.objectReferenceValue = EditorGUI.ObjectField(objRenderArea, label, property.objectReferenceValue, type, false);

            var modifyArea = new Rect(objRenderArea.x + objRenderArea.width, objRenderArea.y, objRenderArea.height, objRenderArea.height);
            bool hasObject = property.objectReferenceValue != null;
            GUI.Label(modifyArea, hasObject ? EditorGUIUtility.IconContent("d_ol_minus") : EditorGUIUtility.IconContent("d_ol_plus"));

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && modifyArea.Contains(ev.mousePosition)) {
                if(property.objectReferenceValue != null) {
                    property.objectReferenceValue = null;
                }
                else {
                    EditorUtility.DisplayDialog("Not implemented", "Popup create new asset is not yet supported", "Ok");
                }
            }
        }

        #endregion
    }
}
