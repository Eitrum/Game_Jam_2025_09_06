using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Rendering
{
    [CustomPropertyDrawer(typeof(InstancedRenderingData))]
    public class InstancedRenderingDataPropertyDrawer : PropertyDrawer
    {
        private static Texture errorIcon;
        private static Texture ErrorIcon {
            get {
                if(!errorIcon) {
                    errorIcon = EditorGUIUtility.IconContent("d_console.erroricon").image;
                }
                return errorIcon;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 5f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var mesh = property.FindPropertyRelative("mesh");
            var material = property.FindPropertyRelative("material");
            var materials = property.FindPropertyRelative("materials");
            if(!property.isExpanded) {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, mesh.objectReferenceValue != null ? mesh.objectReferenceValue.name : label.text, true);
                var iconArea = new Rect(position.x + position.width - EditorGUIUtility.singleLineHeight, position.y, EditorGUIUtility.singleLineHeight, position.height);
                if(material.objectReferenceValue is Material mat && !mat.enableInstancing) {
                    GUI.DrawTexture(iconArea, ErrorIcon);
                }
                else if(materials.arraySize > 0) {
                    for(int i = 0, length = materials.arraySize; i < length; i++) {
                        var element = materials.GetArrayElementAtIndex(i);
                        if(element.objectReferenceValue is Material tmat && !tmat.enableInstancing) {
                            GUI.DrawTexture(iconArea, ErrorIcon);
                            break;
                        }
                    }
                }
            }
            else {
                var locations = property.FindPropertyRelative("locations");
                var splits = position.SplitVertical(5, 0f);
                property.isExpanded = EditorGUI.Foldout(splits[0], property.isExpanded, mesh.objectReferenceValue != null ? mesh.objectReferenceValue.name : label.text, true);
                using(new EditorGUI.IndentLevelScope(1)) {
                    using(new EditorGUI.DisabledScope(true)) {
                        EditorGUI.PropertyField(splits[1], mesh);
                        EditorGUI.PropertyField(splits[2], material);
                    }
                    EditorGUI.LabelField(splits[3], $"Locations", $"{locations.arraySize}");

                    EditorGUI.IndentedRect(splits[4]).SplitHorizontal(out Rect helpArea, out Rect buttonArea, 1f - (80f / splits[4].width), 2f);
                    bool foundError = false;
                    if(material.objectReferenceValue is Material mat && !mat.enableInstancing) {
                        foundError = true;
                        EditorGUI.HelpBox(helpArea, $"Material do not have instancing enabled!", MessageType.Error);
                        if(GUI.Button(buttonArea, "Enable"))
                            mat.enableInstancing = true;
                    }
                    else if(materials.arraySize > 0) {
                        for(int i = 0, length = materials.arraySize; i < length; i++) {
                            var element = materials.GetArrayElementAtIndex(i);
                            if(element.objectReferenceValue is Material tmat && !tmat.enableInstancing) {
                                foundError = true;
                                EditorGUI.HelpBox(helpArea, $"Material do not have instancing enabled!", MessageType.Error);
                                if(GUI.Button(buttonArea, "Enable")) {
                                    tmat.enableInstancing = true;
                                    break;
                                }
                            }
                        }
                    }
                    if(!foundError) {
                        EditorGUI.IndentedRect(splits[4]).SplitHorizontal(out Rect bakeArea, out Rect rebuildArea, 0.5f, 2f);
                        if(GUI.Button(bakeArea, "Bake"))
                            Bake(property);
                        if(GUI.Button(rebuildArea, "Rebuild"))
                            Unbake(property);
                    }
                }
            }
        }

        public static void Bake(SerializedProperty property) {
            var source = property.serializedObject.targetObject as MonoBehaviour;
            if(source == null)
                throw new System.NullReferenceException("Property source is null!");
            var meshProp = property.FindPropertyRelative("mesh");
            var materialProp = property.FindPropertyRelative("material");
            var locationsProp = property.FindPropertyRelative("locations");

            var mesh = meshProp.objectReferenceValue as Mesh;
            var material = materialProp.objectReferenceValue as Material;

            if(mesh == null || material == null)
                throw new System.NullReferenceException("Unable to bake this object as both mesh and material is required to make it work!");

            BakeTransform(mesh, material, source.transform, locationsProp);
        }

        private static void BakeTransform(Mesh mesh, Material material, Transform transform, SerializedProperty locationProperty) {
            // Handle recursive children
            var count = transform.childCount;
            for(int i = count - 1; i >= 0; i--) {
                BakeTransform(mesh, material, transform.GetChild(i), locationProperty);
            }

            // Handle location and verification
            var mf = transform.GetComponent<MeshFilter>();
            var mr = transform.GetComponent<MeshRenderer>();

            if(mf == null || mr == null || (mf.sharedMesh != mesh || mr.sharedMaterial != material))
                return;

            if(locationProperty.arraySize >= 1023)
                return;
            locationProperty.arraySize++;
            var element = locationProperty.GetArrayElementAtIndex(locationProperty.arraySize - 1);
            var matrix = mf.transform.localToWorldMatrix;
            RenderingBakeUtility.SetMatrix(element, matrix);

            // Handle Disable/Destroy Object
            if(transform.childCount != 0 || transform.GetComponents<Component>().Length > 3) { // If children exists or multiple components, disable only
                mr.enabled = false;
                return;
            }

            GameObject.DestroyImmediate(transform.gameObject);
        }

        public static void Unbake(SerializedProperty property) {
            var source = property.serializedObject.targetObject as MonoBehaviour;
            if(source == null)
                throw new System.NullReferenceException("Property source is null!");
            var meshProp = property.FindPropertyRelative("mesh");
            var materialProp = property.FindPropertyRelative("material");
            var locationsProp = property.FindPropertyRelative("locations");

            var mesh = meshProp.objectReferenceValue as Mesh;
            var material = materialProp.objectReferenceValue as Material;

            if(mesh == null || material == null)
                throw new System.NullReferenceException("Unable to rebuild this object as both mesh and material is required to make it work!");


            var parent = source.transform;
            var length = locationsProp.arraySize;
            var name = mesh.name;
            for(int i = 0; i < length; i++) {
                var go = new GameObject($"{name} ({i})");
                go.transform.SetParent(parent);
                var element = locationsProp.GetArrayElementAtIndex(i);
                var matrix = RenderingBakeUtility.GetMatrix(element);
                go.transform.position = matrix.GetPosition();
                go.transform.rotation = matrix.GetRotation();
                go.transform.SetLossyScale(matrix.GetScale());
                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();
                mf.sharedMesh = mesh;
                mr.sharedMaterial = material;
            }
            locationsProp.arraySize = 0;
        }
    }
}
