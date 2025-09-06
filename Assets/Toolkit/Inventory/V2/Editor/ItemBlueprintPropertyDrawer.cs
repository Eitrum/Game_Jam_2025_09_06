using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Inventory.V2 {
    //[CustomPropertyDrawer(typeof(ItemBlueprint), true)]
    //[CustomPropertyDrawer(typeof(ItemMetadataBlueprint), true)]
    public class ItemBlueprintPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var result = 0f;
            if(property.objectReferenceValue == null || !property.isExpanded || EditorStylesUtility.IsHoldingAlt || property.depth > 10)
                return EditorGUIUtility.singleLineHeight + 8;
            using(SerializedObject so = new SerializedObject(property.objectReferenceValue)) {
                var iterator = so.GetIterator();
                if(!iterator.NextVisible(true))
                    return EditorGUIUtility.singleLineHeight;
                do {
                    result += EditorGUI.GetPropertyHeight(iterator, true);
                } while(iterator.NextVisible(false));
            }
            return result + 8;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height -= 4;
            var index = property.depth;
            // Draw Colored outline
            var color = ToolkitEditorUtility.GetColor(index, 1f);
            EditorGUI.DrawRect(new Rect(position.x, position.y, 2f, position.height), color);
            EditorGUI.DrawRect(new Rect(position.x, position.y + position.height - 2f, position.width, 2f), color);

            position.x += 4f;
            position.width -= 4f;

            // Handle object not drawn
            if(property.objectReferenceValue == null || EditorStylesUtility.IsHoldingAlt) {
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height - 4f), property, label);
                return;
            }

            var posy = position.y;
            var topArea = new Rect(position.x, posy, position.width-EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

            using(new EditorGUI.DisabledScope(true)) {
                EditorGUI.ObjectField(topArea, property, label);
            }
            property.isExpanded = EditorGUI.Foldout(topArea, property.isExpanded, label, true);
            var clearArea = new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height);
            GUI.Label(clearArea, EditorGUIUtility.IconContent("d_ol_minus"));

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && clearArea.Contains(ev.mousePosition)) {
                // DELETE COMMAND
                Debug.LogWarning($"DELETE OBJECT IF CHILD AND ONLY REFERENCE");
                property.objectReferenceValue = null;
                return;
            }

            if(!property.isExpanded || property.depth > 10)
                return;

            posy += topArea.height;

            EditorGUI.indentLevel++;
            using(var so = new SerializedObject(property.objectReferenceValue)) {
                so.UpdateIfRequiredOrScript();
                var iterator = so.GetIterator();
                if(!iterator.NextVisible(true))
                    return;
                while(iterator.NextVisible(false)) {
                    var height = EditorGUI.GetPropertyHeight(iterator, true);
                    var area = new Rect(position.x, posy, position.width, height);
                    EditorGUI.PropertyField(area, iterator, true);
                    posy += height;
                }
                so.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel--;
        }

        private static GUIContent clearIcon;

        public static void DrawBlock<T>(string name, SerializedProperty reference, ref Editor editor, int colorId = -1) {
            // Initialize GUI Content
            if(clearIcon == null)
                clearIcon = EditorGUIUtility.IconContent("d_Clear");

            // Draw
            EditorGUI.indentLevel++;
            using(var vertScope = new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUILayout.HorizontalScope()) {
                    reference.isExpanded = EditorGUILayout.Foldout(reference.isExpanded, name, true);
                    if(GUILayout.Button(clearIcon, GUILayout.Width(20))) {
                        var obj = reference.objectReferenceValue;
                        DestroyChildren(reference.serializedObject.targetObject, obj);
                        reference.objectReferenceValue = null;
                    }
                }
                if(reference.isExpanded) {
                    if(reference.objectReferenceValue != null) {
                        if(editor == null) {
                            editor = Editor.CreateEditor(reference.objectReferenceValue);
                        }
                        else if(editor.serializedObject.targetObject != reference.objectReferenceValue) {
                            Editor.DestroyImmediate(editor);
                            editor = Editor.CreateEditor(reference.objectReferenceValue);
                        }
                        editor.OnInspectorGUI();
                    }
                    else {
                        //if(AbilityPartDatabase.DrawLayoutPopup<T>("Add", out AbilityPartDatabase.Data t)) {
                        //    var obj = CreateInstance(t.Type);
                        //    obj.name = t.Name;
                        //    AssetDatabase.AddObjectToAsset(obj, reference.serializedObject.targetObject);
                        //    reference.objectReferenceValue = obj;
                        //    AssetDatabase.SaveAssets();
                        //}
                    }
                }
                if(colorId >= 0) {
                    var area = vertScope.rect;
                    area.width = 4f;
                    EditorGUI.DrawRect(area, ToolkitEditorUtility.GetColor(colorId));
                }
            }
            EditorGUI.indentLevel--;
        }



        public static void DestroyChildren(UnityEngine.Object parentObject, UnityEngine.Object obj) {
            DestroyChildrenRecursive(parentObject, obj);
            AssetDatabase.SaveAssets();
        }

        private static void DestroyChildrenRecursive(UnityEngine.Object parentObject, UnityEngine.Object obj) {
            if(obj == null || !AssetDatabase.IsSubAsset(obj) || !AssetDatabase.GetAssetPath(obj).StartsWith(AssetDatabase.GetAssetPath(parentObject))) {
                return;
            }
            SerializedObject so = new SerializedObject(obj);
            var prop = so.GetIterator();
            prop.Next(true);
            while(prop.NextVisible(true)) {
                if(prop.propertyType == SerializedPropertyType.ObjectReference) {
                    DestroyChildrenRecursive(parentObject, prop.objectReferenceValue);
                }
            }
            so.Dispose();
            AssetDatabase.RemoveObjectFromAsset(obj);
            Editor.DestroyImmediate(obj);
        }

    }
}
