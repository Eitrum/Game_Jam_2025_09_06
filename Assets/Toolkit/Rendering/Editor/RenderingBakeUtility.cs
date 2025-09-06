using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.Rendering
{
    public static class RenderingBakeUtility
    {
        #region Rendering Instance Data

        public static void BakeInstanceDataArray(SerializedProperty property) {
            if(!property.isArray)
                throw new System.ArgumentException("Wrong freaking property!");

            // Setup
            var transform = (property.serializedObject.targetObject as Component).transform;
            InstancedRenderingData[] data = new InstancedRenderingData[property.arraySize];

            // Extract
            for(int i = 0, length = property.arraySize; i < length; i++) {
                var element = property.GetArrayElementAtIndex(i);
                var mesh = element.FindPropertyRelative("mesh").objectReferenceValue as Mesh;
                var mat = element.FindPropertyRelative("material").objectReferenceValue as Material;
                var locationsProp = element.FindPropertyRelative("locations");
                Matrix4x4[] locations = new Matrix4x4[locationsProp.arraySize];
                for(int ii = 0, locLength = locationsProp.arraySize; ii < locLength; ii++) {
                    locations[ii] = GetMatrix(locationsProp.GetArrayElementAtIndex(ii));
                }
                data[i] = new InstancedRenderingData(mesh, mat, locations);
            }

            // Create new
            if(data == null)
                data = RenderingBakeUtility.CreateRenderingInstanceData(transform);
            else
                data = RenderingBakeUtility.CreateRenderingInstanceData(transform, data);
            RenderingBakeUtility.DestroyChildren(transform, data);

#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayProgressBar("Baking", "", 0f);
#endif

            // Insert
            property.arraySize = data.Length;
            for(int i = 0, length = property.arraySize; i < length; i++) {
                var d = data[i];
                var element = property.GetArrayElementAtIndex(i);
                var mesh = d.Mesh;
                var mat = d.Material;
                var locations = d.Locations;

#if UNITY_EDITOR
                if(!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayProgressBar("Baking", $"{d.Mesh.name} -- 0/{locations.Count}", i / (float)length);
#endif

                element.FindPropertyRelative("mesh").objectReferenceValue = mesh;
                element.FindPropertyRelative("material").objectReferenceValue = mat;
                var locationsProp = element.FindPropertyRelative("locations");
                locationsProp.arraySize = Mathf.Min(1024, locations.Count);
                for(int ii = 0, locLength = locationsProp.arraySize; ii < locLength; ii++) {
#if UNITY_EDITOR
                    if(!Application.isPlaying)
                        UnityEditor.EditorUtility.DisplayProgressBar("Baking", $"{d.Mesh.name} -- {ii}/{locLength}", i / (float)length);
#endif
                    SetMatrix(locationsProp.GetArrayElementAtIndex(ii), locations[ii]);
                }
            }

#if UNITY_EDITOR
            if(!Application.isPlaying) {
                UnityEditor.EditorUtility.ClearProgressBar();
                if(transform.gameObject.scene.IsValid())
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(transform.gameObject.scene);
                else
                    UnityEditor.EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
#endif
        }

        public static InstancedRenderingData[] CreateRenderingInstanceData(Transform root) {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayProgressBar("Rendering Data", "", 0f);
#endif
            int index = 0;
            MeshFilter[] meshes = new MeshFilter[root.childCount];
            for(int i = 0, length = root.childCount; i < length; i++) {
#if UNITY_EDITOR
                if(!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayProgressBar("Rendering Data", $"extracting mesh filters ({index})", index++ / (float)meshes.Length);
#endif
                meshes[i] = root.GetChild(i).GetComponentInChildren<MeshFilter>();
            }
            index = 0;
            List<TemporaryStorage> storage = new List<TemporaryStorage>();
            foreach(var mf in meshes) {
#if UNITY_EDITOR
                if(!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayProgressBar("Rendering Data", mf.name, index++ / (float)meshes.Length);
#endif
                var mr = mf.GetComponent<MeshRenderer>();
                if(mr == null)
                    continue;
                var lod = mf.GetComponentInParent<LODGroup>();
                if(lod && lod.transform != mf.transform)
                    continue;
                var mesh = mf.sharedMesh;
                var mat = mr.sharedMaterial;
                var loc = mf.transform.localToWorldMatrix;
                bool found = false;
                foreach(var s in storage) {
                    if(s.IsEqualTo(mesh, mat)) {
                        s.Add(loc);
                        found = true;
                        break;
                    }
                }
                if(!found)
                    storage.Add(new TemporaryStorage(mesh, mat, loc));
            }
            var instances = storage.Sum(x => x.SubContainers);
            InstancedRenderingData[] instanceData = new InstancedRenderingData[instances];
            index = 0;
            foreach(var s in storage) {
                var totalArray = s.Locations.Split(s.SubContainers);
                for(int i = 0, length = s.SubContainers; i < length; i++) {
                    instanceData[index] = new InstancedRenderingData(s.Mesh, s.Material, totalArray[i]);
                    index++;
                }
            }
#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
            return instanceData;
        }

        public static InstancedRenderingData[] CreateRenderingInstanceData(Transform root, IReadOnlyList<InstancedRenderingData> existingData) {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayProgressBar("Rendering Data", "", 0f);
#endif
            int index = 0;
            MeshFilter[] meshes = new MeshFilter[root.childCount];
            for(int i = 0, length = root.childCount; i < length; i++) {
#if UNITY_EDITOR
                if(!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayProgressBar("Rendering Data", $"extracting mesh filters ({index})", index++ / (float)meshes.Length);
#endif
                meshes[i] = root.GetChild(i).GetComponentInChildren<MeshFilter>();
            }
            index = 0;

            List<TemporaryStorage> storage = new List<TemporaryStorage>();

            foreach(var d in existingData) {
                var mesh = d.Mesh;
                var mat = d.Material;
                if(mesh == null || mat == null)
                    continue;

                var found = false;
                foreach(var s in storage) {
                    if(s.IsEqualTo(mesh, mat)) {
                        s.AddRange(d.Locations);
                        found = true;
                        break;
                    }
                }
                if(!found) {
                    storage.Add(new TemporaryStorage(mesh, mat, d.Locations));
                }
            }

            foreach(var mf in meshes) {
#if UNITY_EDITOR
                if(!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayProgressBar("Rendering Data", mf.name, index++ / (float)meshes.Length);
#endif
                var mr = mf.GetComponent<MeshRenderer>();
                if(mr == null)
                    continue;
                var lod = mf.GetComponentInParent<LODGroup>();
                if(lod && lod.transform != mf.transform)
                    continue;
                var mesh = mf.sharedMesh;
                var mat = mr.sharedMaterial;
                var loc = mf.transform.localToWorldMatrix;
                bool found = false;
                foreach(var s in storage) {
                    if(s.IsEqualTo(mesh, mat)) {
                        s.Add(loc);
                        found = true;
                        break;
                    }
                }
                if(!found)
                    storage.Add(new TemporaryStorage(mesh, mat, loc));
            }
            var instances = storage.Sum(x => x.SubContainers);
            InstancedRenderingData[] instanceData = new InstancedRenderingData[instances];
            index = 0;
            foreach(var s in storage) {
                var totalArray = s.Locations.Split(s.SubContainers);
                for(int i = 0, length = s.SubContainers; i < length; i++) {
                    instanceData[index] = new InstancedRenderingData(s.Mesh, s.Material, totalArray[i]);
                    index++;
                }
            }
#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
            return instanceData;
        }

        public static void DestroyChildren(Transform root, IReadOnlyList<InstancedRenderingData> existingData) {
            PrefabUnpack(root);
            foreach(var data in existingData) {
                DestroyChildren(root, data.Mesh, data.Material);
            }
        }

        private static void PrefabUnpack(Transform transform) {
            if(UnityEditor.PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject)) {
                var newObjects = UnityEditor.PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(transform.gameObject, UnityEditor.PrefabUnpackMode.Completely);
                foreach(var go in newObjects) {
                    PrefabUnpack(go.transform);
                }
                return;
            }
            var count = transform.childCount;
            for(int i = count - 1; i >= 0; i--) {
                PrefabUnpack(transform.GetChild(i));
            }
        }

        private static void DestroyChildren(Transform transform, Mesh mesh, Material material) {
            // Handle recursive children
            var count = transform.childCount;
            for(int i = count - 1; i >= 0; i--) {
                DestroyChildren(transform.GetChild(i), mesh, material);
            }

            // Handle location and verification
            var mf = transform.GetComponent<MeshFilter>();
            var mr = transform.GetComponent<MeshRenderer>();

            if(mf == null || mr == null || (mf.sharedMesh != mesh || mr.sharedMaterial != material))
                return;

            var lod = transform.GetComponent<LODGroup>();
            if(lod != null) {
                HandleLodGroup(lod, mesh, material);
            }

            // Handle Disable/Destroy Object
            if(transform.childCount != 0 || transform.GetComponents<Component>().Length > 3) { // If children exists or multiple components, disable only
                mr.enabled = false;
                return;
            }
#if UNITY_EDITOR
            if(Application.isPlaying)
                GameObject.Destroy(transform.gameObject);
            else
                GameObject.DestroyImmediate(transform.gameObject);
#else
            GameObject.Destroy(transform.gameObject);
#endif
        }

        private static void HandleLodGroup(LODGroup group, Mesh mesh, Material material) {
            var children = group.transform.childCount;
            bool allDestroyed = true;
            for(int i = children - 1; i >= 0; i--) {
                var t = group.transform.GetChild(i);
                var mf = t.GetComponent<MeshFilter>();
                var mr = t.GetComponent<MeshRenderer>();
                if(mf == null || mr == null)
                    continue;

                if(t.GetComponents<Component>().Length == 3) {
#if UNITY_EDITOR
                    if(Application.isPlaying)
                        GameObject.Destroy(t.gameObject);
                    else
                        GameObject.DestroyImmediate(t.gameObject);
#else
                    GameObject.Destroy(t.gameObject);
#endif
                }
                else {
                    mr.enabled = false;
                    allDestroyed = false;
                }
            }
            if(allDestroyed) {
#if UNITY_EDITOR
                if(Application.isPlaying)
                    GameObject.Destroy(group);
                else
                    GameObject.DestroyImmediate(group);
#else
                    GameObject.Destroy(group);
#endif
            }
            else {
                group.enabled = false;
            }
        }

        private class TemporaryStorage
        {
            private Mesh mesh = null;
            private Material material = null;
            private List<Matrix4x4> locations = new List<Matrix4x4>();

            public Mesh Mesh => mesh;
            public Material Material => material;
            public IReadOnlyList<Matrix4x4> Locations => locations;
            public int LocationCount => locations.Count;
            public int SubContainers => (locations.Count - 1) / 1023 + 1;

            public TemporaryStorage(Mesh mesh, Material material) {
                this.mesh = mesh;
                this.material = material;
            }

            public TemporaryStorage(Mesh mesh, Material material, Matrix4x4 location) {
                this.mesh = mesh;
                this.material = material;
                this.locations.Add(location);
            }

            public TemporaryStorage(Mesh mesh, Material material, IReadOnlyList<Matrix4x4> locations) {
                this.mesh = mesh;
                this.material = material;
                this.locations.AddRange(locations);
            }

            public bool IsEqualTo(Mesh mesh, Material material) {
                return this.mesh == mesh && this.material == material;
            }

            public void Add(Matrix4x4 location) => locations.Add(location);
            public void AddRange(IReadOnlyList<Matrix4x4> locations) => this.locations.AddRange(locations);
        }

        #endregion

        #region Matrix

        public unsafe static void SetMatrix(SerializedProperty property, Matrix4x4 matrix) {
            if(property.type == "Matrix4x4")
                throw new System.InvalidCastException($"Unable to get matrix from property as it is not of correct type '{property.type}'");

            property.NextVisible(true);
            property.floatValue = matrix.m00;
            property.NextVisible(true);
            property.floatValue = matrix.m01;
            property.NextVisible(true);
            property.floatValue = matrix.m02;
            property.NextVisible(true);
            property.floatValue = matrix.m03;

            property.NextVisible(true);
            property.floatValue = matrix.m10;
            property.NextVisible(true);
            property.floatValue = matrix.m11;
            property.NextVisible(true);
            property.floatValue = matrix.m12;
            property.NextVisible(true);
            property.floatValue = matrix.m13;

            property.NextVisible(true);
            property.floatValue = matrix.m20;
            property.NextVisible(true);
            property.floatValue = matrix.m21;
            property.NextVisible(true);
            property.floatValue = matrix.m22;
            property.NextVisible(true);
            property.floatValue = matrix.m23;

            property.NextVisible(true);
            property.floatValue = matrix.m30;
            property.NextVisible(true);
            property.floatValue = matrix.m31;
            property.NextVisible(true);
            property.floatValue = matrix.m32;
            property.NextVisible(true);
            property.floatValue = matrix.m33;
        }

        public unsafe static Matrix4x4 GetMatrix(SerializedProperty property) {
            if(property.type == "Matrix4x4")
                throw new System.InvalidCastException($"Unable to get matrix from property as it is not of correct type '{property.type}'");
            Matrix4x4 result = Matrix4x4.identity;
            result.m00 = property.FindPropertyRelative("e00").floatValue;
            result.m01 = property.FindPropertyRelative("e01").floatValue;
            result.m02 = property.FindPropertyRelative("e02").floatValue;
            result.m03 = property.FindPropertyRelative("e03").floatValue;

            result.m10 = property.FindPropertyRelative("e10").floatValue;
            result.m11 = property.FindPropertyRelative("e11").floatValue;
            result.m12 = property.FindPropertyRelative("e12").floatValue;
            result.m13 = property.FindPropertyRelative("e13").floatValue;

            result.m20 = property.FindPropertyRelative("e20").floatValue;
            result.m21 = property.FindPropertyRelative("e21").floatValue;
            result.m22 = property.FindPropertyRelative("e22").floatValue;
            result.m23 = property.FindPropertyRelative("e23").floatValue;

            result.m30 = property.FindPropertyRelative("e30").floatValue;
            result.m31 = property.FindPropertyRelative("e31").floatValue;
            result.m32 = property.FindPropertyRelative("e32").floatValue;
            result.m33 = property.FindPropertyRelative("e33").floatValue;

            return result;
        }

        #endregion
    }
}
