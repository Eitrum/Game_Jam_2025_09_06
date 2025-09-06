using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Utility
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    [AddComponentMenu("Toolkit/Bones/Skinned Mesh Bones")]
    public class SkinnedMeshBones : MonoBehaviour
    {
        #region Variables

        private SkinnedMeshRenderer smr;
        [SerializeField, HideInInspector] private int rootBone;
        [SerializeField, HideInInspector] private int[] bones = { };

        #endregion

        #region Properties

        public IReadOnlyList<int> Bones => bones;
        public SkinnedMeshRenderer SkinnedMeshRenderer {
            get {
                if(!smr)
                    smr = GetComponent<SkinnedMeshRenderer>();
                return smr;
            }
        }

        #endregion

        #region Reconstruction

        [ContextMenu("Reconstruct")]
        public void Reconstruct() {
            if(!SkinnedMeshRenderer.rootBone) {
                Debug.LogError("Unable to reconstruct bones as no root bone exists on skinned mesh renderer @" + gameObject.name);
                return;
            }
            var bones = SkinnedMeshRenderer.rootBone.GetComponentInParent<BoneReferences>();
            if(bones)
                Reconstruct(bones);
            else
                Debug.LogError("Unable to reconstruct bones as no bones class exists on the root bone");
        }

        public void Reconstruct(BoneReferences bones) {
            var length = this.bones.Length;
            var newBones = new Transform[length];
            for(int i = 0; i < length; i++) {
                newBones[i] = bones.GetBone(this.bones[i]);
                if(newBones[i] == null) {
                    Debug.LogError("Could not find bone: " + this.bones[i]);
                }
            }
            SkinnedMeshRenderer.bones = newBones;
            var newRootBone = bones.GetBone(rootBone);
            if(newRootBone)
                SkinnedMeshRenderer.rootBone = newRootBone;
        }

        #endregion

        #region Bake Validation

        [ContextMenu("Bake")]
        public void Bake() {
            var smrbones = SkinnedMeshRenderer.bones;
            this.rootBone = SkinnedMeshRenderer.rootBone.name.GetHash32();
            this.bones = new int[smrbones.Length];
            for(int i = 0, length = smrbones.Length; i < length; i++) {
                if(smrbones[i] == null)
                    this.bones[i] = 0;
                else
                    this.bones[i] = smrbones[i].name.GetHash32();
            }
#if UNITY_EDITOR
            if(gameObject.scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            else
                UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public bool IsValid() {
            return bones.Length > 0 && !bones.Any(x => x == 0);
        }

        #endregion
    }
}
