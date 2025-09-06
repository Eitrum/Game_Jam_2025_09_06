using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(TerrainLayerCollection))]
    public class TerrainLayerCollectionInspector : Editor
    {
        #region Variables

        private SerializedProperty layers;
        private ReorderableList list;
        private List<SerializedObject> serializedLayers = new List<SerializedObject>();
        private static GUIContent applyButton = new GUIContent("Apply Terrain", "Applies the serialized layers to any terrain in scene");

        #endregion

        #region Enable

        private void OnEnable() {
            layers = serializedObject.FindProperty("layers");
            list = new ReorderableList(serializedObject, layers);
            list.headerHeight = 0f;
            list.onAddCallback += OnAdd;
            list.onRemoveCallback += OnRemove;
            list.drawElementCallback += DrawElement;
            list.elementHeightCallback += OnElementHeight;
            list.onReorderCallbackWithDetails += Reorder;

            serializedLayers.Clear();
            for(int i = 0, length = layers.arraySize; i < length; i++) {
                serializedLayers.Add(new SerializedObject(layers.GetArrayElementAtIndex(i).objectReferenceValue));
            }
        }

        #endregion

        public override void OnInspectorGUI() {
            serializedObject.Update();

            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button(applyButton, GUILayout.Width(110f))) {
                    var terrains = FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    var activeTerrains = terrains.Select(x => x.name).CombineToString(true);
                    if(EditorUtility.DisplayDialog("Apply Terrain Layers", activeTerrains, "Yes", "Cancel")) {
                        var layersToApply = (target as TerrainLayerCollection).GetLayers();
                        foreach(var t in terrains) {
                            var data = t.terrainData;
                            if(data != null)
                                data.SetTerrainLayersRegisterUndo(layersToApply, "WHAT HAVE I DONE!");
                        }
                    }
                }
                if(GUILayout.Button("Create Texture2DArray", GUILayout.Width(150f))) {
                    CreateTexture2DArray();
                }
            }

            list.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        private void CreateTexture2DArray() {
            var albedoTexures = serializedLayers.Select(x => x.FindProperty("m_DiffuseTexture").objectReferenceValue as Texture2D);
            if(albedoTexures.Any(x => x == null)) {
                EditorUtility.DisplayDialog("Error", "One or more textures is null!", "Ok");
                return;
            }
            var main = albedoTexures.FirstOrDefault();
            if(main == null) {
                EditorUtility.DisplayDialog("Error", "There is no textures in the collection", "Ok");
                return;
            }
            var tex = Texture2DArrayUtility.Create(albedoTexures);
            var current = AssetDatabase.GetAssetPath(target);
            current = current.Replace($"{target.name}.asset", "new texture array.asset");
            var fileName = AssetDatabase.GenerateUniqueAssetPath(current);
            AssetDatabase.CreateAsset(tex, fileName);
            AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }

        #region List callbacks

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var element = layers.GetArrayElementAtIndex(index);
            rect.PadRef(0, 0, 2, 2);
            var foldout = new Rect(rect);
            foldout.height = EditorGUIUtility.singleLineHeight;
            element.isExpanded = EditorGUI.Foldout(foldout, element.isExpanded, element.objectReferenceValue.name);

            if(element.isExpanded) {
                var texObj = serializedLayers[index];
                texObj.Update();

                var nameProp = texObj.FindProperty("m_Name");
                var diffuseProp = texObj.FindProperty("m_DiffuseTexture");
                var normalProp = texObj.FindProperty("m_NormalMapTexture");
                var maskProp = texObj.FindProperty("m_MaskMapTexture");
                var normalScaleProp = texObj.FindProperty("m_NormalScale");
                var specularProp = texObj.FindProperty("m_Specular");
                var metallicProp = texObj.FindProperty("m_Metallic");
                var smoothnessProp = texObj.FindProperty("m_Smoothness");
                var tileSizeProp = texObj.FindProperty("m_TileSize");
                var tileOffsetProp = texObj.FindProperty("m_TileOffset");

                EditorGUI.DelayedTextField(foldout, nameProp, GUIContent.none);
                rect.SplitVertical(out Rect header, out Rect textures, out Rect body, EditorGUIUtility.singleLineHeight / rect.height, (EditorGUIUtility.singleLineHeight * 5f) / rect.height, 0f);
                textures.SplitVertical(out Rect textureHeaders, out Rect textureAreas, 0.2f, 0f);
                var headerSplits = textureHeaders.SplitHorizontal(3, 20f);
                var textureSplits = textureAreas.SplitHorizontal(3, 20f);
                EditorGUI.LabelField(headerSplits[0], "Albedo", EditorStylesUtility.CenterAlignedBoldLabel);
                EditorGUI.LabelField(headerSplits[1], "Normal", EditorStylesUtility.CenterAlignedBoldLabel);
                EditorGUI.LabelField(headerSplits[2], "Mask", EditorStylesUtility.CenterAlignedBoldLabel);

                diffuseProp.objectReferenceValue = EditorGUI.ObjectField(textureSplits[0], diffuseProp.objectReferenceValue, typeof(Texture2D), false);
                normalProp.objectReferenceValue = EditorGUI.ObjectField(textureSplits[1], normalProp.objectReferenceValue, typeof(Texture2D), false);
                maskProp.objectReferenceValue = EditorGUI.ObjectField(textureSplits[2], maskProp.objectReferenceValue, typeof(Texture2D), false);

                int offset = normalProp.objectReferenceValue != null ? 1 : 0;
                int count = offset + 5;
                var bodyItems = body.Pad(0, 0, 2, 0).SplitVertical(count, 3f);
                if(normalProp.objectReferenceValue) {
                    EditorGUI.Slider(bodyItems[0], normalScaleProp, -2f, 2f);
                }
                EditorGUI.PropertyField(bodyItems[offset + 0], specularProp);
                EditorGUI.Slider(bodyItems[offset + 1], metallicProp, 0f, 1f);
                EditorGUI.Slider(bodyItems[offset + 2], smoothnessProp, 0f, 1f);
                EditorGUI.PropertyField(bodyItems[offset + 3], tileSizeProp);
                EditorGUI.PropertyField(bodyItems[offset + 4], tileOffsetProp);

                if(texObj.hasModifiedProperties)
                    texObj.ApplyModifiedProperties();
            }
        }

        private float OnElementHeight(int index) {
            var element = layers.GetArrayElementAtIndex(index);
            if(element.isExpanded) {
                var lay = serializedLayers[index];
                var res = EditorGUIUtility.singleLineHeight * 12f + 4f;
                if(lay.FindProperty("m_NormalMapTexture").objectReferenceValue != null) {
                    res += EditorGUIUtility.singleLineHeight;
                }
                return res;
            }
            return EditorGUIUtility.singleLineHeight + 4f;
        }

        private void OnAdd(ReorderableList list) {
            var layer = new TerrainLayer();
            layer.name = "layer";
            AssetDatabase.AddObjectToAsset(layer, target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target), ImportAssetOptions.ForceUpdate);
            layers.arraySize++;
            layers.GetArrayElementAtIndex(layers.arraySize - 1).objectReferenceValue = layer;
            serializedLayers.Add(new SerializedObject(layer));
        }

        private void OnRemove(ReorderableList list) {
            var index = list.index;
            var prop = layers.GetArrayElementAtIndex(index);
            var obj = prop.objectReferenceValue;
            prop.objectReferenceValue = null;
            if(obj != null) {
                AssetDatabase.RemoveObjectFromAsset(obj);
                DestroyImmediate(obj);
            }
            layers.DeleteArrayElementAtIndex(index);
            serializedLayers.RemoveAt(index);
        }

        private void Reorder(ReorderableList list, int oldIndex, int newIndex) {
            var so = serializedLayers[oldIndex];
            serializedLayers[oldIndex] = serializedLayers[newIndex];
            serializedLayers[newIndex] = so;
        }

        #endregion
    }
}
