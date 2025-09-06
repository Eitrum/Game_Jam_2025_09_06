using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(TerrainDetailCollection))]
    public class TerrainDetailCollectionInspector : Editor
    {
        private SerializedProperty details;
        private ReorderableList list;
        private float iconSize;
        private int selectedIcon = -1;

        private Texture textureIcon;
        private Texture prefabIcon;

        private static GUIContent applyButton = new GUIContent("Add Details", "Adds the details to the Terrain object");
        private static List<Editor> previewEditors = new List<Editor>();
        private static int id;

        private void OnEnable() {
            id = GetInstanceID();
            iconSize = EditorGUIUtility.singleLineHeight * 6f;
            details = serializedObject.FindProperty("details");
            list = new ReorderableList(serializedObject, details);
            list.drawElementCallback += OnDrawElement;
            list.elementHeightCallback += OnElementHeight;
            list.headerHeight = 0f;
            list.onAddCallback += OnAdd;
            list.onRemoveCallback += OnRemove;

            textureIcon = EditorGUIUtility.TrIconContent("d_Texture Icon").image;
            prefabIcon = EditorGUIUtility.TrIconContent("d_Prefab Icon").image;

            foreach(var pe in previewEditors)
                if(pe)
                    DestroyImmediate(pe);

            previewEditors.Clear();

            for(int i = 0, length = details.arraySize; i < length; i++) {
                var prefab = details.GetArrayElementAtIndex(i).FindPropertyRelative("prefab").objectReferenceValue;
                if(prefab)
                    previewEditors.Add(Editor.CreateEditor(prefab));
                else
                    previewEditors.Add(null);
            }
        }

        private void OnAdd(ReorderableList list) {
            details.arraySize++;
            serializedObject.ApplyModifiedProperties();
            var tdc = target as TerrainDetailCollection;
            var field = typeof(TerrainDetailCollection).GetField("details", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            TerrainDetailCollection.Detail[] detailsArray = (TerrainDetailCollection.Detail[])field.GetValue(tdc);
            detailsArray[detailsArray.Length - 1] = new TerrainDetailCollection.Detail();
            serializedObject.Update();
            previewEditors.Add(null);
        }

        private void OnRemove(ReorderableList list) {
            var index = list.index;
            previewEditors.RemoveAt(index);
            details.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        private float OnElementHeight(int index) {
            return EditorGUIUtility.singleLineHeight * 6f + 4;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var element = details.GetArrayElementAtIndex(index);
            var textureProp = element.FindPropertyRelative("texture");
            var prefabProp = element.FindPropertyRelative("prefab");
            var widthProp = element.FindPropertyRelative("width");
            var heightProp = element.FindPropertyRelative("height");
            var noiseSpreadProp = element.FindPropertyRelative("noiseSpread");
            var healthyColorProp = element.FindPropertyRelative("healthyColor");
            var dryColorProp = element.FindPropertyRelative("dryColor");
            var bendFactorProp = element.FindPropertyRelative("bendFactor");
            var billboardProp = element.FindPropertyRelative("billboard");

            var texture = textureProp.objectReferenceValue as Texture2D;
            var prefab = prefabProp.objectReferenceValue as GameObject;
            var ev = Event.current;

            rect.PadRef(2f, 2f, 2f, 2f);
            rect.SplitHorizontal(out Rect iconArea, out Rect bodyArea, iconSize / rect.width, 4f);
            var texIconArea = new Rect(iconArea.x + 2, iconArea.y + 2, 16, 16);
            var prefabIconArea = new Rect(iconArea.x + 20, iconArea.y + 2, 16, 16);

            if(ev.type == EventType.MouseDown && ev.button == 0) {
                if(texIconArea.Contains(ev.mousePosition)) {
                    selectedIcon = index;
                    EditorGUIUtility.ShowObjectPicker<Texture2D>(texture, false, "", GetInstanceID());
                }
                else if(prefabIconArea.Contains(ev.mousePosition)) {
                    selectedIcon = index;
                    EditorGUIUtility.ShowObjectPicker<GameObject>(prefab, false, "", GetInstanceID());
                }
            }

            if(ev.type == EventType.DragUpdated || ev.type == EventType.DragPerform && iconArea.Contains(ev.mousePosition)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if(ev.type == EventType.DragPerform) {
                    var references = DragAndDrop.objectReferences.Where(x => x is GameObject || x is Texture2D);
                    var reference = references.FirstOrDefault();
                    if(reference is GameObject go)
                        prefabProp.objectReferenceValue = go;
                    else if(reference is Texture2D tex)
                        textureProp.objectReferenceValue = tex;
                    DragAndDrop.AcceptDrag();
                    ev.Use();
                }
            }

            EditorGUI.DrawRect(iconArea, Color.black);
            string name = "null";
            if(prefab) {
                var editor = previewEditors[index];
                if(!editor)
                    editor = previewEditors[index] = Editor.CreateEditor(prefab);
                editor.OnPreviewGUI(iconArea.Shrink(2f), EditorStyles.whiteLabel);
                name = prefab.name;
            }
            else if(texture) {
                GUI.DrawTexture(iconArea.Shrink(2f), texture);
                name = texture.name;
            }
            GUI.DrawTexture(texIconArea, textureIcon);
            GUI.DrawTexture(prefabIconArea, prefabIcon);
            EditorGUI.DrawRect(iconArea.Pad(2, 2, iconArea.height - 16, 2), new Color32(25, 25, 25, 205));
            EditorGUI.LabelField(iconArea.Pad(2, 2, iconArea.height - 16, 2), name, EditorStylesUtility.CenterAlignedMiniLabel);

            var lines = bodyArea.SplitVertical(6, 1f);
            EditorGUI.PropertyField(lines[0], widthProp);
            EditorGUI.PropertyField(lines[1], heightProp);

            EditorGUI.PropertyField(lines[2], noiseSpreadProp);
            EditorGUI.PropertyField(lines[3], healthyColorProp);
            EditorGUI.PropertyField(lines[4], dryColorProp);
            if(prefab)
                EditorGUI.PropertyField(lines[5], bendFactorProp);
            else if(texture)
                EditorGUI.PropertyField(lines[5], billboardProp);
        }

        public override void OnInspectorGUI() {
            if(id != GetInstanceID()) {
                EditorGUILayout.HelpBox("Can only render 1 terrain detail collection inspector at a time", MessageType.Error);
                if(GUILayout.Button("Make this inspector main", GUILayout.Width(200f))) {
                    OnEnable();
                }
                return;
            }
            serializedObject.Update();

            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button(applyButton, GUILayout.Width(110f))) {
                    var terrains = FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    var activeTerrains = terrains.Select(x => x.name).CombineToString(true);
                    if(EditorUtility.DisplayDialog("Add details to Terrain", activeTerrains, "Yes", "Cancel")) {
                        var prototypesToApply = (target as TerrainDetailCollection).GetDetailPrototypes();
                        foreach(var t in terrains) {
                            var data = t.terrainData;
                            if(data != null)
                                data.detailPrototypes = data.detailPrototypes.AddRange(prototypesToApply).ToArray();
                        }
                    }
                }
            }

            var ev = Event.current;
            list.DoLayoutList();

            if(ev.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == GetInstanceID()) {
                var obj = EditorGUIUtility.GetObjectPickerObject();
                var element = details.GetArrayElementAtIndex(selectedIcon);
                if(obj is GameObject go) {
                    element.FindPropertyRelative("prefab").objectReferenceValue = go;
                    element.FindPropertyRelative("texture").objectReferenceValue = null;
                    if(previewEditors[selectedIcon])
                        DestroyImmediate(previewEditors[selectedIcon]);
                }
                else if(obj is Texture2D texture) {
                    element.FindPropertyRelative("texture").objectReferenceValue = texture;
                    element.FindPropertyRelative("prefab").objectReferenceValue = null;
                }
            }

            if(ev.type == EventType.DragUpdated || ev.type == EventType.DragPerform) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if(ev.type == EventType.DragPerform) {
                    var references = DragAndDrop.objectReferences.Where(x => x is GameObject || x is Texture2D);
                    var count = references.Count();
                    int index = details.arraySize;
                    foreach(var reference in references) {
                        OnAdd(list);
                        if(reference is GameObject go)
                            details.GetArrayElementAtIndex(index).FindPropertyRelative("prefab").objectReferenceValue = go;
                        else if(reference is Texture2D texture)
                            details.GetArrayElementAtIndex(index).FindPropertyRelative("texture").objectReferenceValue = texture;
                        index++;
                    }
                    DragAndDrop.AcceptDrag();
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
