using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    /// <summary>
    /// NOT COMPLETE
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Bones/Bone References (Animator - Human Bones)")]
    public class BoneReferencesAnimatorHumanBones : MonoBehaviour, IReadOnlyList<Transform>, IEnumerable<Transform>, IEnumerable
    {
        #region Variables

        [System.NonSerialized] private Dictionary<int, Transform> lookup = new Dictionary<int, Transform>();
        [System.NonSerialized] private int count;
        [System.NonSerialized] private bool isInitialized = false;
        [System.NonSerialized] private Animator anim;

        #endregion

        #region Properties

        public int Count => count;
        public BonesSnapshot Snapshot {
            get {
                Initialize();
                return new BonesSnapshot(Space.Self, lookup);
            }
        }
        public BonesSnapshot WorldSnapshot {
            get {
                Initialize();
                return new BonesSnapshot(Space.World, lookup);
            }
        }
        public Transform Root => transform;

        public Transform this[int boneId] => GetBone(boneId);
        public Transform this[HumanBodyBones boneName] => GetBone(boneName);

        #endregion

        #region Initialize

        private void Awake() {
            if(!isInitialized)
                Initialize();
        }

        private void Initialize() {
            if(isInitialized)
                return;
            anim = GetComponentInParent<Animator>();
            isInitialized = true;
            for(int i = 0, length = HumanBodyBones.LastBone.ToInt(); i < length; i++) {
                var t = anim.GetBoneTransform((HumanBodyBones)i);
                if(t)
                    lookup.Add(i, t);
            }

            count = lookup.Count;
        }

        #endregion

        #region GetBone

        public Transform GetBone(int id) {
            if(!isInitialized)
                Initialize();
            if(lookup.TryGetValue(id, out Transform bone))
                return bone;
            return null;
        }

        public Transform GetBone(HumanBodyBones name) {
            return GetBone((int)name);
        }

        public IEnumerable<Bone> GetAllBones() {
            if(!isInitialized)
                Initialize();
            foreach(var pair in lookup)
                yield return new Bone(pair.Key, pair.Value);
        }

        #endregion

        #region Enumerators

        public IEnumerator<Transform> GetEnumerator() {
            return lookup.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return lookup.Values.GetEnumerator();
        }

        #endregion

        public struct Bone
        {
            #region Variables

            public HumanBodyBones bodyBone;
            public int id;
            public Transform bone;

            #endregion

            #region Properties

            public int Id => id;
            public Transform Transform {
                get => bone;
                set {
                    bone = value;
                    id = bone ? bone.name.GetHashCode() : 0;
                }
            }

            #endregion

            #region Constructor

            public Bone(Transform bone) {
                this.bodyBone = (HumanBodyBones)(-1);
                this.id = bone.name.GetHash32();
                this.bone = bone;
            }

            public Bone(HumanBodyBones bodyBone, Transform bone) {
                this.bodyBone = bodyBone;
                this.id = bone.name.GetHash32();
                this.bone = bone;
            }

            public Bone(int id, Transform bone) {
                this.bodyBone = (HumanBodyBones)(-1);
                this.id = id;
                this.bone = bone;
            }

            #endregion
        }
    }
}
