using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(PrefabCollection))]
    public class PrefabCollectionInspector : Editor
    {
        private SerializedProperty overrideName;
        private SerializedProperty category;
        private SerializedProperty entries;

        private static int items = 3;
        private Texture closeIcon;
        private Texture editIcon;
        private string[] categories;
        private bool isCustom;
        private int selectedIndex = -1;
        private static Toolkit.PreviewRenderer renderer;

        private void OnEnable() {
            overrideName = serializedObject.FindProperty("overrideName");
            category = serializedObject.FindProperty("category");
            entries = serializedObject.FindProperty("entries");
            PrefabCollectionUtility.Refresh();
            categories = PrefabCollectionUtility.GetCategories().Insert(0, "None").Insert(1, "Custom").ToArray();
            closeIcon = EditorGUIUtility.IconContent("d_TreeEditor.Trash").image;
            editIcon = EditorGUIUtility.IconContent("d_CustomTool").image;
            if(selectedIndex >= entries.arraySize) {
                selectedIndex = -1;
            }
        }

        private void OnDisable() {
            if(renderer != null) {
                renderer.Dispose();
                renderer = null;
            }
        }

        public override void OnInspectorGUI() {
            if(renderer == null)
                renderer = new PreviewRenderer();
            serializedObject.Update();
            HandleHeader();
            EditorGUILayout.Space();
            HandleGrid();
            EditorGUILayout.Space();
            var area = GUILayoutUtility.GetRect(1, 2);
            EditorGUI.DrawRect(area, Color.gray);
            HandleEditSelected();
            var ev = Event.current;
            bool isDraggingGameObjects = (ev.type == EventType.DragPerform || ev.type == EventType.DragUpdated) && !DragAndDrop.objectReferences.Any(x => !(x is GameObject));
            if(isDraggingGameObjects) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if(ev.type == EventType.DragPerform) {
                    var gos = DragAndDrop.objectReferences.Select(x => x as GameObject).Where(x => x != null);
                    var res = gos.Count() > 1 ? EditorUtility.DisplayDialogComplex("Add objects", "Create multiple entries or variants", "Create Entries", "Cancel", "Create Variants") : 0;
                    switch(res) {
                        case 0: // Multiple entries
                            foreach(var go in gos) {
                                var prop = AddEntry();
                                prop.FindPropertyRelative("variants").GetArrayElementAtIndex(0).FindPropertyRelative("prefab").objectReferenceValue = go;
                            }
                            break;
                        case 2: // Variants
                            var en = AddEntry();
                            var arr = en.FindPropertyRelative("variants");
                            gos.Foreach((x, i) => {
                                var variant = i == 0 ? arr.GetArrayElementAtIndex(0) : AddVariant(arr);
                                variant.FindPropertyRelative("prefab").objectReferenceValue = x;
                            });
                            break;
                    }
                    DragAndDrop.AcceptDrag();
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        SerializedProperty AddEntry() {
            var index = entries.arraySize;
            entries.arraySize++;
            serializedObject.ApplyModifiedProperties();
            var prop = entries.GetArrayElementAtIndex(index);
            prop.FindPropertyRelative("name").stringValue = "";
            prop.FindPropertyRelative("weight").floatValue = 1f;
            prop.FindPropertyRelative("variants").arraySize = 0;
            serializedObject.ApplyModifiedProperties();
            AddVariant(prop.FindPropertyRelative("variants"));
            return prop;
        }

        void DeleteEntry(int index) {
            if(index == selectedIndex)
                selectedIndex = -1;
            else if(selectedIndex > index)
                selectedIndex--;
            entries.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty AddVariant(SerializedProperty variantsArrayProperty) {
            var index = variantsArrayProperty.arraySize;
            variantsArrayProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();
            var variantProp = variantsArrayProperty.GetArrayElementAtIndex(index);

            variantProp.FindPropertyRelative("weight").floatValue = 1f;
            variantProp.FindPropertyRelative("minimumSize").vector3Value = Vector3.one;
            variantProp.FindPropertyRelative("maximumSize").vector3Value = Vector3.one;
            variantProp.FindPropertyRelative("sizeWeight").animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            variantProp.FindPropertyRelative("orientation").intValue = PrefabCollection.Orientation.Normal.ToInt();
            variantProp.FindPropertyRelative("direction").vector3Value = Vector3.zero;
            var tiltX = variantProp.FindPropertyRelative("tiltX");
            tiltX.FindPropertyRelative("min").floatValue = 0f;
            tiltX.FindPropertyRelative("max").floatValue = 5f;
            var tiltZ = variantProp.FindPropertyRelative("tiltZ");
            tiltZ.FindPropertyRelative("min").floatValue = 0f;
            tiltZ.FindPropertyRelative("max").floatValue = 5f;
            variantProp.FindPropertyRelative("yAxisRotation").boolValue = true;
            variantProp.FindPropertyRelative("normalOffset").floatValue = 0f;
            variantProp.FindPropertyRelative("originOffset").vector3Value = Vector3.zero;
            serializedObject.ApplyModifiedProperties();
            return variantProp;
        }

        void HandleEditSelected() {
            if(selectedIndex == -1) {
                EditorGUILayout.HelpBox("Click the edit icon on an entry to start editing it", MessageType.Info);
                return;
            }
            var entryProp = entries.GetArrayElementAtIndex(selectedIndex);
            var entry = (target as PrefabCollection).GetEntry(selectedIndex);

            // Handle Name
            var entryName = entryProp.FindPropertyRelative("name");
            EditorGUILayout.PropertyField(entryName, GUIContent.none);
            var nameArea = GUILayoutUtility.GetLastRect();
            if(string.IsNullOrEmpty(entryName.stringValue))
                EditorGUI.LabelField(nameArea, entry.Name, EditorStylesUtility.GrayItalicLabel);

            // Weight
            var weightProp = entryProp.FindPropertyRelative("weight");
            weightProp.floatValue = Mathf.Max(0.001f, EditorGUILayout.FloatField("Entry Weight", weightProp.floatValue));

            // Default Variant
            var variantsProp = entryProp.FindPropertyRelative("variants");
            var variants = entry.Variants;
            if(variantsProp.arraySize == 0) {
                AddVariant(variantsProp);
            }
            HandleVariant(0, variantsProp.GetArrayElementAtIndex(0), variants[0]);
            EditorGUILayout.Space();
            using(new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.Space(20);
                using(new EditorGUILayout.VerticalScope()) {
                    EditorGUILayout.LabelField("Variants", EditorStylesUtility.BoldLabel);
                    for(int i = 1; i < variantsProp.arraySize; i++) {
                        var variant = variantsProp.GetArrayElementAtIndex(i);
                        HandleVariant(i, variant, variants[i]);
                        if(variant.FindPropertyRelative("prefab").objectReferenceValue == null) {
                            variantsProp.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            i--;
                        }
                        EditorGUILayout.Space();
                    }
                    using(new EditorGUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("Add Variant", GUILayout.Width(100f))) {
                            AddVariant(variantsProp);
                        }
                    }
                }
            }
        }

        void HandleVariant(int index, SerializedProperty variantProp, PrefabCollection.Variant variant) {
            var ev = Event.current;
            var prefab = variantProp.FindPropertyRelative("prefab");
            var weight = variantProp.FindPropertyRelative("weight");
            var minimumSize = variantProp.FindPropertyRelative("minimumSize");
            var maximumSize = variantProp.FindPropertyRelative("maximumSize");
            var sizeWeight = variantProp.FindPropertyRelative("sizeWeight");
            var orientation = variantProp.FindPropertyRelative("orientation");
            var direction = variantProp.FindPropertyRelative("direction");
            var tiltX = variantProp.FindPropertyRelative("tiltX");
            var tiltZ = variantProp.FindPropertyRelative("tiltZ");
            var yAxisRotation = variantProp.FindPropertyRelative("yAxisRotation");
            var normalOffset = variantProp.FindPropertyRelative("normalOffset");
            var originOffset = variantProp.FindPropertyRelative("originOffset");
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUILayout.HorizontalScope()) {
                    var previewArea = GUILayoutUtility.GetRect(EditorGUIUtility.singleLineHeight * 5, EditorGUIUtility.singleLineHeight * 5);
                    var removeArea = new Rect(previewArea.x + previewArea.width - 22f, previewArea.y + 2f, 20f, 20f);

                    // Handle Input
                    if(index > 0) {
                        if(ev.type == EventType.MouseDown && ev.button == 0) {
                            if(removeArea.Contains(ev.mousePosition)) {
                                prefab.objectReferenceValue = null;
                            }
                        }
                    }

                    // Preview
                    var rend = variant.Renderer;
                    if(rend != null) {
                        var instances = rend.Instances;
                        var b = rend.Bounds;
                        var s = (1f / b.extents.magnitude);
                        var centerTranslate = -b.center;
                        var trs = Matrix4x4.TRS(centerTranslate * s, Quaternion.identity, s.To_Vector3());
                        renderer.BeginRender(previewArea);
                        foreach(var inst in instances) {
                            renderer.RenderCustom(trs * inst.Offset, inst.Mesh, inst.Material);
                        }
                        renderer.EndRender(previewArea);
                    }
                    else {
                        EditorGUI.DrawRect(previewArea, Color.black);
                    }

                    // Preview Icons
                    if(index > 0) {
                        EditorGUI.DrawRect(removeArea, ColorTable.IndianRed);
                        GUI.DrawTexture(removeArea, closeIcon);
                    }
                    EditorGUI.DropShadowLabel(previewArea.Pad(0, 0, 0, previewArea.height - 16), index == 0 ? "Default" : $"{index}", EditorStylesUtility.BoldLabel);

                    // Fields
                    using(new EditorGUILayout.VerticalScope()) {
                        EditorGUILayout.PropertyField(prefab);
                        EditorGUILayout.PropertyField(weight);
                        EditorGUILayout.PropertyField(minimumSize);
                        EditorGUILayout.PropertyField(maximumSize);
                        EditorGUILayout.PropertyField(sizeWeight);
                    }
                }
                // Orientation
                EditorGUILayout.PropertyField(orientation);
                using(new EditorGUI.IndentLevelScope(1)) {
                    var ori = orientation.intValue.ToEnum<PrefabCollection.Orientation>();
                    switch(ori) {
                        case PrefabCollection.Orientation.Direction:
                            EditorGUILayout.PropertyField(direction);
                            EditorGUILayout.PropertyField(tiltX);
                            EditorGUILayout.PropertyField(yAxisRotation);
                            break;
                        case PrefabCollection.Orientation.Normal:
                            EditorGUILayout.PropertyField(direction, new GUIContent("Offset Rotation"));
                            EditorGUILayout.PropertyField(tiltX);
                            EditorGUILayout.PropertyField(yAxisRotation);
                            EditorGUILayout.PropertyField(normalOffset);
                            break;
                        case PrefabCollection.Orientation.Tilt:
                            EditorGUILayout.PropertyField(tiltX);
                            EditorGUILayout.PropertyField(yAxisRotation);
                            break;
                        case PrefabCollection.Orientation.Default:
                            EditorGUILayout.HelpBox("Default orientation will always spawn with Quaternion.identity", MessageType.Info);
                            break;
                        case PrefabCollection.Orientation.Random:
                            EditorGUILayout.HelpBox("Random orientation will always spawn with a complete random rotation", MessageType.Info);
                            break;
                    }
                }
                EditorGUILayout.PropertyField(originOffset);
                tiltZ.FindPropertyRelative("min").floatValue = tiltX.FindPropertyRelative("min").floatValue;
                tiltZ.FindPropertyRelative("max").floatValue = tiltX.FindPropertyRelative("max").floatValue;
            }
        }

        void HandleGrid() {
            var collection = target as PrefabCollection;
            var ev = Event.current;
            items = EditorGUILayout.IntSlider(items, 1, 8);
            EditorGUILayout.Space();
            var inspectorWidth = Screen.width - 40f;
            var size = inspectorWidth / (float)items;
            var entryCount = this.entries.arraySize + 1;
            var rows = Mathf.Max(Mathf.CeilToInt((entryCount) / (float)(items)), 1);
            var area = GUILayoutUtility.GetRect(1, rows * (size + 4f));

            //EditorGUI.DrawRect(area, Color.red);

            int index = 0;
            for(int y = 0; y < rows; y++) {
                for(int x = 0; x < items; x++) {
                    var previewArea = new Rect(area.x + x * (size + 2f) + 1f, area.y + y * (size + 2f) + 1f, size, size);
                    if(index < entryCount - 1) {

                        var removeArea = new Rect(previewArea.x + previewArea.width - 22f, previewArea.y + 2f, 20f, 20f);
                        var editArea = new Rect(previewArea.x + 2f, previewArea.y + 2f, 20f, 20f);

                        // Handle Input
                        if(ev.type == EventType.MouseDown && ev.button == 0) {
                            if(removeArea.Contains(ev.mousePosition)) {
                                DeleteEntry(index);
                                entryCount--;
                                continue;
                            }
                            else if(editArea.Contains(ev.mousePosition)) {
                                selectedIndex = index;
                            }
                        }

                        // Preview
                        var entry = this.entries.GetArrayElementAtIndex(index);
                        var variants = entry.FindPropertyRelative("variants");
                        if(variants.arraySize == 0) {
                            variants.arraySize++;
                            serializedObject.ApplyModifiedProperties();
                        }
                        var rend = collection.GetEntry(index).DefaultRenderer;
                        if(rend != null) {
                            var instances = rend.Instances;
                            var b = rend.Bounds;
                            var s = (1f / b.extents.magnitude);
                            var centerTranslate = -b.center;
                            var trs = Matrix4x4.TRS(centerTranslate * s, Quaternion.identity, s.To_Vector3());
                            renderer.BeginRender(previewArea);
                            foreach(var inst in instances) {
                                renderer.RenderCustom(trs * inst.Offset, inst.Mesh, inst.Material);
                            }
                            renderer.EndRender(previewArea);
                        }
                        else {
                            EditorGUI.DrawRect(previewArea, Color.black);
                        }

                        // Icons
                        EditorGUI.DrawRect(removeArea, ColorTable.IndianRed);
                        GUI.DrawTexture(removeArea, closeIcon);
                        GUI.DrawTexture(editArea, editIcon);
                        EditorGUI.DropShadowLabel(previewArea.Pad(0, 0, previewArea.height - 18f, 0f), collection.GetEntry(index).Name, EditorStylesUtility.CenterAlignedBoldLabel);
                        EditorGUI.DropShadowLabel(previewArea.Pad(0, 0, 4f, previewArea.height - 16f), $"{index}", EditorStylesUtility.CenterAlignedBoldLabel);

                    }
                    else if(index < entryCount) {
                        var halfSize = size / 2f;
                        EditorGUI.DrawRect(previewArea, Color.black);
                        EditorGUI.DrawRect(previewArea.Shrink(2f), Color.gray);
                        EditorGUI.DrawRect(previewArea.Pad(halfSize * 0.9f, halfSize * 0.9f, halfSize * 0.2f, halfSize * 0.2f), Color.black);
                        EditorGUI.DrawRect(previewArea.Pad(halfSize * 0.2f, halfSize * 0.2f, halfSize * 0.9f, halfSize * 0.9f), Color.black);
                        if(ev.type == EventType.MouseDown && ev.button == 0 && previewArea.Contains(ev.mousePosition)) {
                            AddEntry();
                        }
                    }
                    index++;
                }
            }
        }

        void HandleHeader() {
            using(new EditorGUILayout.VerticalScope("box")) {
                overrideName.stringValue = EditorGUILayout.TextField(overrideName.stringValue);
                if(string.IsNullOrEmpty(overrideName.stringValue)) {
                    var nameArea = GUILayoutUtility.GetLastRect();
                    EditorGUI.LabelField(nameArea, target.name, EditorStylesUtility.GrayItalicLabel);
                }

                EditorGUI.BeginChangeCheck();
                var cat = category.stringValue;
                var index = isCustom ? 1 : 0;
                if(index != 1) {
                    if(!string.IsNullOrEmpty(cat)) {
                        for(int i = 2, length = categories.Length; i < length; i++) {
                            if(categories[i] == cat) {
                                index = i;
                                break;
                            }
                        }
                    }
                }

                if(isCustom) {
                    cat = EditorGUILayout.TextField("Category", cat);
                }
                else {
                    index = EditorGUILayout.Popup("Category", index, categories);
                }

                if(EditorGUI.EndChangeCheck()) {
                    if(isCustom) {
                        category.stringValue = cat;
                        if(string.IsNullOrEmpty(cat))
                            isCustom = false;
                    }
                    else {
                        if(index == 1) {
                            isCustom = true;
                            category.stringValue = "";
                        }
                        else {
                            category.stringValue = categories[index];
                        }
                    }
                }
            }
        }
    }
}
