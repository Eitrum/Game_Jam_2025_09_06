using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

namespace Toolkit.PhysicEx
{
    public static class MeshColliderEditorUtility
    {
        public static void ResetToDefaultSettings(MeshCollider col) {
            col.isTrigger = false;
            col.material = null;
            col.convex = false;
            col.contactOffset = Physics.defaultContactOffset;
            col.cookingOptions = (
                MeshColliderCookingOptions.CookForFasterSimulation |
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase);
        }

        #region Reset Mesh Colliders

        [MenuItem("CONTEXT/MeshFilter/Reset Mesh Colliders", priority = 1000)]
        private static void ResetMeshColliderOnSelectedObject(MenuCommand command) {
            if(command.context is MeshFilter mf) {
                var assetPath = AssetDatabase.GetAssetPath(mf);
                if(string.IsNullOrEmpty(assetPath)) {
                    var colliders = mf.GetComponents<MeshCollider>();

                    if(colliders.Length == 0) {
                        colliders = new MeshCollider[] { mf.gameObject.AddComponent<MeshCollider>() };
                    }
                    else {
                        for(int i = 1, length = colliders.Length; i < length; i++) {
                            GameObject.DestroyImmediate(colliders[i]);
                        }
                    }

                    ResetToDefaultSettings(colliders[0]);
                    colliders[0].sharedMesh = mf.sharedMesh;
                    EditorUtility.SetDirty(mf);
                }
                else {
                    var go = PrefabUtility.InstantiatePrefab(mf.gameObject) as GameObject;
                    mf = go.GetComponent<MeshFilter>();
                    var colliders = mf.GetComponents<MeshCollider>();

                    if(colliders.Length == 0) {
                        colliders = new MeshCollider[] { mf.gameObject.AddComponent<MeshCollider>() };
                    }
                    else {
                        for(int i = 1, length = colliders.Length; i < length; i++) {
                            GameObject.DestroyImmediate(colliders[i]);
                        }
                    }

                    ResetToDefaultSettings(colliders[0]);
                    colliders[0].sharedMesh = mf.sharedMesh;

                    PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
                    GameObject.DestroyImmediate(go);
                }
            }
        }

        [MenuItem("CONTEXT/MeshFilter/Reset Mesh Colliders", priority = 1000, validate = true)]
        private static bool ResetMeshColliderOnSelectedObjectVerification(MenuCommand command) {
            return command.context is MeshFilter;
        }

        [MenuItem("CONTEXT/MeshFilter/Move To Child", priority = 1001)]
        private static void MoveToChildModel(MenuCommand command) {
            if(command.context is MeshFilter mf) {
                var transform = mf.transform;
                // Setup
                var go = new GameObject("model");
                go.transform.SetParent(transform);
                TransformData.Default.ApplyTo(go.transform, Space.Self);

                // Copy Mesh Filter
                go.AddComponent<MeshFilter>(mf);
                GameObject.DestroyImmediate(mf);

                // Copy Mesh Renderer
                var renderer = transform.GetComponent<MeshRenderer>();
                if(renderer) {
                    go.AddComponent<MeshRenderer>(renderer);
                    GameObject.DestroyImmediate(renderer);
                }

                EditorUtility.SetDirty(transform);
            }
        }

        #endregion

        #region Physics // TEMPORARY LOCATION MOVE TO OWN FILE

        [MenuItem("CONTEXT/CharacterJoint/Attach to parent", priority = 1000)]
        private static void AttachCharacterJointToParentRigidbody(MenuCommand command) {
            if(command.context is CharacterJoint cj) {
                var pBody = cj.transform.parent.GetComponentInParent<Rigidbody>();
                cj.connectedBody = pBody;
                EditorUtility.SetDirty(cj);
            }
        }

        [MenuItem("CONTEXT/CharacterJoint/Attach to parent", priority = 1000, validate = true)]
        private static bool AttachCharacterJointToParentRigidbodyValidation(MenuCommand command) {
            if(command.context is CharacterJoint mf) {
                var assetPath = AssetDatabase.GetAssetPath(mf);
                if(string.IsNullOrEmpty(assetPath)) {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Transform // TEMPORARY LOCATION MOVE TO OWN FILE

        [MenuItem("CONTEXT/Transform/Detach all children", priority = 1000)]
        private static void ReleaseAllChildren(MenuCommand command) {
            if(command.context is Transform t) {
                var parent = t.parent;
                var count = t.childCount;
                for(int i = count - 1; i >= 0; i--) {
                    t.GetChild(i).SetParent(parent);
                }
                EditorUtility.SetDirty(t);
            }
        }

        [MenuItem("CONTEXT/Transform/Reset (But keep children world position)", priority = 1001)]
        private static void ResetKeepChildrenWorldPosition(MenuCommand command) {
            if(command.context is Transform t) {
                var count = t.childCount;
                TransformData[] data = new TransformData[count];
                for(int i = count - 1; i >= 0; i--) {
                    data[i] = new TransformData(t.GetChild(i), Space.World);
                }
                TransformData.Default.ApplyTo(t, Space.Self);
                for(int i = count - 1; i >= 0; i--) {
                    data[i].ApplyTo(t.GetChild(i), Space.World);
                }

                EditorUtility.SetDirty(t);
            }
        }

        [System.Serializable]
        public struct TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;

            public static TransformData Default => new TransformData() { scale = Vector3.one };

            public TransformData(Transform transform, Space space) {
                if(space == Space.Self) {
                    position = transform.localPosition;
                    rotation = transform.localRotation;
                    scale = transform.localScale;
                }
                else {
                    position = transform.position;
                    rotation = transform.rotation;
                    scale = transform.lossyScale;
                }
            }

            public void ApplyTo(Transform t, Space space) {
                if(space == Space.Self) {
                    t.localPosition = position;
                    t.localRotation = rotation;
                    t.localScale = scale;
                }
                else {
                    t.SetPositionAndRotation(position, rotation);
                    t.SetLossyScale(scale);
                }
            }
        }

        #endregion

        #region LOG GROUP // TEMPORARY LOCATION MOVE TO OWN FILE

        [MenuItem("CONTEXT/LODGroup/100-30-20-5 Split", priority = 1000)]
        private static void BalanceLodGroup(MenuCommand command) {
            if(command.context is LODGroup t) {
                var lods = t.GetLODs();
                for(int i = 0, length = lods.Length; i < length; i++) {
                    lods[i].screenRelativeTransitionHeight = LodBalance(i);
                }
                t.SetLODs(lods);
                EditorUtility.SetDirty(t);
            }
        }
        [MenuItem("CONTEXT/LODGroup/100-60-10 Split", priority = 1000)]
        private static void BalanceLod2Group(MenuCommand command) {
            if(command.context is LODGroup t) {
                var lods = t.GetLODs();
                for(int i = 0, length = lods.Length; i < length; i++) {
                    lods[i].screenRelativeTransitionHeight = LodBalance2(i);
                }
                t.SetLODs(lods);
                EditorUtility.SetDirty(t);
            }
        }

        [MenuItem("CONTEXT/LODGroup/Remove Last", priority = 1000)]
        private static void RemoveLastGroup(MenuCommand command) {
            if(command.context is LODGroup t) {
                var lods = t.GetLODs();
                LOD[] newLods = new LOD[lods.Length];
                for(int i = 0, length = lods.Length - 1; i < length; i++) {
                    newLods[i] = lods[i];
                }
                t.SetLODs(newLods);
                var toRemove = t.transform.GetChild(t.transform.childCount - 1);
                GameObject.DestroyImmediate(toRemove.gameObject);
                EditorUtility.SetDirty(t);
            }
        }

        private static float LodBalance(int index) {
            switch(index) {
                case 0: return 0.3f;
                case 1: return 0.2f;
                case 2: return 0.05f;
            }
            return 0.02f;
        }

        private static float LodBalance2(int index) {
            switch(index) {
                case 0: return 0.6f;
                case 1: return 0.1f;
            }
            return 0.05f;
        }

        #endregion
    }
}
