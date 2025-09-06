using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(TerrainTreeCollection))]
    public class TerrainTreeCollectionInspector : Editor
    {
        private SerializedProperty trees;
        private static List<Editor> previewAssets = new List<Editor>();
        private static int items = 2;
        private Texture closeIcon;
        private Texture pickIcon;
        private static GUIContent applyButton = new GUIContent("Apply Terrain", "Applies the tree prototypes to any terrain in scene");
        private static int id;

        private int selectedIndex = -1;

        private void OnEnable() {
            id = GetInstanceID();
            trees = serializedObject.FindProperty("trees");
            foreach(var pa in previewAssets)
                if(pa && pa.target)
                    DestroyImmediate(pa);
            previewAssets.Clear();
            for(int i = 0, length = trees.arraySize; i < length; i++) {
                var element = trees.GetArrayElementAtIndex(i).FindPropertyRelative("prefab");
                if(element.objectReferenceValue != null)
                    previewAssets.Add(Editor.CreateEditor(element.objectReferenceValue));
                else
                    previewAssets.Add(null);
            }
            closeIcon = EditorGUIUtility.IconContent("d_TreeEditor.Trash").image;
            pickIcon = EditorGUIUtility.IconContent("d_pick").image;
        }

        public override void OnInspectorGUI() {
            if(id != GetInstanceID()) {
                EditorGUILayout.HelpBox("Can only render 1 terrain tree collection inspector at a time", MessageType.Error);
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
                    if(EditorUtility.DisplayDialog("Apply Tree Prototypes", activeTerrains, "Yes", "Cancel")) {
                        var prototypesToApply = (target as TerrainTreeCollection).GetTreePrototypes();
                        foreach(var t in terrains) {
                            var data = t.terrainData;
                            if(data != null)
                                data.treePrototypes = prototypesToApply;
                        }
                    }
                }
            }


            var ev = Event.current;
            items = EditorGUILayout.IntSlider(items, 1, 8);
            EditorGUILayout.Space();
            var inspectorWidth = Screen.width - 40f;
            var size = inspectorWidth / (float)items;
            var treesCount = trees.arraySize + 1;
            var rows = Mathf.Max(Mathf.CeilToInt((treesCount) / (float)(items)), 1);
            var area = GUILayoutUtility.GetRect(1, rows * (size + 4f));

            bool isDraggingGameObjects = (ev.type == EventType.DragPerform || ev.type == EventType.DragUpdated) && !DragAndDrop.objectReferences.Any(x => !(x is GameObject));

            var index = 0;
            for(int y = 0; y < rows; y++) {
                for(int x = 0; x < items; x++) {
                    if(index < treesCount - 1) {
                        var obj = trees.GetArrayElementAtIndex(index).FindPropertyRelative("prefab").objectReferenceValue;
                        var previewArea = new Rect(area.x + x * (size + 2f) + 1f, area.y + y * (size + 2f) + 1f, size, size);
                        EditorGUI.DrawRect(previewArea, Color.black);
                        var removeArea = new Rect(previewArea.x + previewArea.width - 22f, previewArea.y + 2f, 20f, 20f);
                        var pickArea = new Rect(previewArea.x + 2f, previewArea.y + 2f, 20f, 20f);
                        var indexArea = new Rect(pickArea);
                        indexArea.y += pickArea.height + 4f;
                        indexArea.width = previewArea.width;

                        if(ev.type == EventType.MouseDown && ev.button == 0) {
                            if(removeArea.Contains(ev.mousePosition)) {
                                previewAssets.RemoveAt(index);
                                trees.DeleteArrayElementAtIndex(index);
                                treesCount--;
                                serializedObject.ApplyModifiedProperties();
                                continue;
                            }
                            else if(pickArea.Contains(ev.mousePosition)) {
                                selectedIndex = index;
                                EditorGUIUtility.ShowObjectPicker<GameObject>(obj, false, "", GetInstanceID());
                            }
                            else if(ev.control && previewArea.Contains(ev.mousePosition) && obj != null) {
                                EditorGUIUtility.PingObject(obj);
                            }
                        }

                        EditorGUI.DrawRect(previewArea.Shrink(2f), EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255));
                        if(previewAssets[index] != null)
                            previewAssets[index].OnPreviewGUI(previewArea.Shrink(2f).Pad(0, 0, 20, 0), EditorStyles.whiteLabel);

                        if(size > 100) {
                            var bend = trees.GetArrayElementAtIndex(index).FindPropertyRelative("bend");
                            var bendArea = previewArea.Pad(24, 24, 2, size - 22);
                            bend.floatValue = EditorGUI.FloatField(bendArea, bend.floatValue);
                            EditorGUI.LabelField(bendArea, "Bend Factor", EditorStyles.centeredGreyMiniLabel);
                        }

                        EditorGUI.DrawRect(removeArea, ColorTable.IndianRed);
                        GUI.DrawTexture(removeArea, closeIcon);
                        GUI.DrawTexture(pickArea, pickIcon);
                        EditorGUI.DropShadowLabel(previewArea.Pad(0, 0, previewArea.height - 16f, 0f), $"{(obj != null ? obj.name : "null")}", EditorStylesUtility.CenterAlignedBoldLabel);
                        EditorGUI.LabelField(indexArea, $"{index}", EditorStyles.boldLabel);

                        if(isDraggingGameObjects && previewArea.Contains(ev.mousePosition)) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                            if(ev.type == EventType.DragPerform) {
                                var objs = DragAndDrop.objectReferences;
                                UpdateProperty(index, objs.FirstOrDefault() as GameObject);
                                DragAndDrop.AcceptDrag();
                            }
                            ev.Use();
                        }
                    }
                    else if(index < treesCount) {
                        var halfSize = size / 2f;
                        var previewArea = new Rect(area.x + x * (size + 2f) + 1f, area.y + y * (size + 2f) + 1f, size, size);
                        EditorGUI.DrawRect(previewArea, Color.black);
                        EditorGUI.DrawRect(previewArea.Shrink(2f), Color.gray);
                        EditorGUI.DrawRect(previewArea.Pad(halfSize * 0.9f, halfSize * 0.9f, halfSize * 0.2f, halfSize * 0.2f), Color.black);
                        EditorGUI.DrawRect(previewArea.Pad(halfSize * 0.2f, halfSize * 0.2f, halfSize * 0.9f, halfSize * 0.9f), Color.black);
                        if(ev.type == EventType.MouseDown && ev.button == 0 && previewArea.Contains(ev.mousePosition)) {
                            var obj = trees.arraySize > 0 ? trees.GetArrayElementAtIndex(trees.arraySize - 1).FindPropertyRelative("prefab").objectReferenceValue : null;
                            trees.arraySize++;
                            previewAssets.Add(obj != null ? Editor.CreateEditor(obj) : null);
                        }
                        if(isDraggingGameObjects && previewArea.Contains(ev.mousePosition)) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            if(ev.type == EventType.DragPerform) {
                                var objs = DragAndDrop.objectReferences;
                                var startIndex = trees.arraySize;
                                trees.arraySize += objs.Length;
                                serializedObject.ApplyModifiedProperties();
                                for(int i = 0; i < objs.Length; i++) {
                                    previewAssets.Add(null);
                                    UpdateProperty(startIndex + i, objs[i] as GameObject);
                                }
                                DragAndDrop.AcceptDrag();
                            }
                            ev.Use();
                        }
                    }
                    index++;
                }
            }
            if(ev.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == GetInstanceID()) {
                if(selectedIndex >= 0) {
                    var obj = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                    UpdateProperty(selectedIndex, obj);
                }
            }
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void UpdateProperty(int index, GameObject obj) {
            if(index < 0 || index >= trees.arraySize)
                return;
            var element = trees.GetArrayElementAtIndex(index);
            if(element == null)
                return;
            var prefab = element.FindPropertyRelative("prefab");
            prefab.objectReferenceValue = obj;
            if(previewAssets[index] != null)
                DestroyImmediate(previewAssets[index]);
            previewAssets[index] = obj != null ? Editor.CreateEditor(obj) : null;
        }
    }
}
