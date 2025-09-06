using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Bones/Bone References")]
    public class BoneReferences : MonoBehaviour, IReadOnlyList<Transform>, IEnumerable<Transform>, IEnumerable
    {
        #region Variables

        [System.NonSerialized] private Dictionary<int, Transform> lookup = new Dictionary<int, Transform>();
        [System.NonSerialized] private int count;
        [System.NonSerialized] private bool isInitialized = false;

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
        public Transform this[string boneName] => GetBone(boneName);

        #endregion

        #region Initialize

        private void Awake() {
            if(!isInitialized)
                Initialize();
        }

        private void Initialize() {
            if(isInitialized)
                return;
            isInitialized = true;
            RecursiveAdd(transform, lookup);
            count = lookup.Count;
        }

        private static void RecursiveAdd(Transform bone, Dictionary<int, Transform> dictionary) {
            dictionary.Add(bone.name.GetHash32(), bone);
            var children = bone.childCount;
            for(int i = 0; i < children; i++) {
                RecursiveAdd(bone.GetChild(i), dictionary);
            }
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

        public Transform GetBone(string name) {
            return GetBone(name.GetHash32());
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
            public int id;
            public Transform bone;

            public Bone(int id, Transform bone) {
                this.id = id;
                this.bone = bone;
            }
        }
    }
}
