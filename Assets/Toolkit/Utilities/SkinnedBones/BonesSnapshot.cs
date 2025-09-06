using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [System.Serializable]
    public class BonesSnapshot : IReadOnlyList<BonesSnapshot.Bone>
    {
        #region Variables

        [SerializeField] private Space space = Space.Self;
        [SerializeField] private Bone[] bones = { };

        #endregion

        #region Properties

        public Space Space => space;
        public IReadOnlyList<Bone> Bones => bones;
        public int Count => bones.Length;
        public Bone this[int index] => bones[index];

        #endregion

        #region Constructor

        public BonesSnapshot() { }
        public BonesSnapshot(Space space) { this.space = space; }
        internal BonesSnapshot(Space space, Dictionary<int, Transform> bones) {
            this.space = space;
            Assign(bones);
        }

        #endregion

        #region Methods

        public void Assign(Dictionary<int, Transform> bones) {
            var count = bones.Count;
            this.bones = new Bone[count];
            int index = 0;
            foreach(var b in bones)
                this.bones[index++] = new Bone(b.Key, b.Value, space);
        }

        public Bone GetBone(int index) => bones[index];

        public void Restore(BoneReferences references) {
            for(int i = 0, length = bones.Length; i < length; i++) {
                var bone = references.GetBone(bones[i].BoneId);
                if(bone)
                    bone.SetPositionAndRotation(bones[i].Pose, space);
            }
        }

        #endregion

        #region Enumerator

        public IEnumerator<Bone> GetEnumerator() {
            return bones.GetEnumerator() as IEnumerator<Bone>;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return bones.GetEnumerator();
        }

        #endregion

        [System.Serializable]
        public struct Bone
        {
            #region Variables

            [SerializeField] private int boneId;
            [SerializeField] private Vector3 position;
            [SerializeField] private Quaternion rotation;

            #endregion

            #region Properties

            public int BoneId => boneId;
            public Vector3 Position => position;
            public Quaternion Rotation => rotation;
            public Pose Pose => new Pose(position, rotation);

            #endregion

            #region Constructor

            public Bone(int boneId, Transform bone) {
                this.boneId = boneId;
                this.position = bone.localPosition;
                this.rotation = bone.localRotation;
            }

            public Bone(int boneId, Transform bone, Space space) {
                this.boneId = boneId;
                if(space == Space.Self) {
                    this.position = bone.localPosition;
                    this.rotation = bone.localRotation;
                }
                else {
                    this.position = bone.position;
                    this.rotation = bone.rotation;
                }
            }

            #endregion
        }
    }
}
