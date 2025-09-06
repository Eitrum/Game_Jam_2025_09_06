using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.Utility;
using UnityEngine;
using System.Linq;

namespace Toolkit.PhysicEx
{
    [AddComponentMenu("Toolkit/Physics/Ragdoll (Static)")]
    public class RagdollStatic : MonoBehaviour, IRagdoll
    {
        #region Variables

        [SerializeField] private Instance[] parts = { };
        [Header("Restore")]
        [SerializeField] private Mathematics.Ease.Function easeFunction = Mathematics.Ease.Function.Linear;
        [SerializeField] private Mathematics.Ease.Type easeType = Mathematics.Ease.Type.InOut;
        [SerializeField] private bool disableHighSpeedCheck = false;

        private Animator animatorReference = null;
        private Animation animationReference = null;
        private BoneReferences boneReference = null;

        private Coroutine checkError = null;
        private Coroutine routine = null;
        private bool isInitialized = false;
        private float initialSpeed = 0f;

        #endregion

        #region Properties

        public bool IsInitialized => isInitialized;
        public Rigidbody Root => parts.Length > 0 ? parts[0].Body : null;
        public IReadOnlyList<Instance> Parts => parts;
        public BoneReferences Bones => boneReference;

        #endregion

        #region Init

        void Awake() {
            animatorReference = GetComponentInParent<Animator>();
            animationReference = GetComponentInParent<Animation>();
            boneReference = GetComponentInChildren<BoneReferences>();

            foreach(var p in parts)
                p.Unload();
        }

        void OnDestroy() {
            if(routine != null)
                Timer.Stop(routine);
        }

        #endregion

        #region Initialize

        public Rigidbody Initialize() {
            if(isInitialized)
                return Root;
            isInitialized = true;
            if(parts.Length == 0)
                return GetComponentInChildren<Rigidbody>();
            var root = parts[0];

            Timer.Stop(routine);

            if(animationReference)
                animationReference.enabled = false;
            if(animatorReference)
                animatorReference.enabled = false;

            foreach(var p in parts)
                p.Load();

            return root.Body;
        }

        public Rigidbody Initialize(Vector3 direction, float impulseForce) {
            var root = Initialize();
            root.AddForce(direction * impulseForce, ForceMode.Impulse);

            if(!disableHighSpeedCheck) {
                initialSpeed = direction.magnitude * impulseForce;
                Timer.NextFrame(CheckForHighspeed, 1, ref checkError);
            }

            return root;
        }
        #endregion

        #region Restore

        public void Restore(bool enableAnimation = true) {
            if(!isInitialized)
                return;
            isInitialized = false;

            Timer.Stop(routine);

            if(animationReference)
                animationReference.enabled = enableAnimation;
            if(animatorReference)
                animatorReference.enabled = enableAnimation;

            foreach(var p in parts)
                p.Unload();
        }

        public void Restore(AnimationClip clip, float duration) {
            if(!isInitialized)
                return;
            isInitialized = false;

            var current = boneReference.Snapshot;
            clip.SampleAnimation(this.gameObject, 0f);
            var target = boneReference.Snapshot;

            foreach(var p in parts)
                p.Unload();

            Timer.Animate(duration, AnimateRestore, new AnimationData() {
                current = current,
                target = target,
            }, Mathematics.Ease.GetEaseFunctionAsFunc(easeFunction, easeType), AnimateEnableReferences, ref routine);
        }

        private void AnimateEnableReferences() {
            if(animationReference)
                animationReference.enabled = true;
            if(animatorReference)
                animatorReference.enabled = true;
        }

        private void AnimateRestore(float t, AnimationData data) {
            if(boneReference == null)
                return;
            var boneCount = data.current.Count;
            for(int i = 0; i < boneCount; i++) {
                var current = data.current.GetBone(i);
                var target = data.target.GetBone(i);

                var transform = boneReference.GetBone(current.BoneId);
                transform.localPosition = Vector3.Lerp(current.Position, target.Position, t);
                transform.localRotation = Quaternion.Slerp(current.Rotation, target.Rotation, t);
            }
        }

        #endregion

        #region Utility

        private void CheckForHighspeed() {
            for(int i = 0, length = parts.Length; i < length; i++) {
                parts[i].Body.linearVelocity = parts[i].Body.linearVelocity.ClampMagnitude(initialSpeed);
            }
        }

        #endregion

        #region Editor

        [ContextMenu("Add Parts")]
        internal void AddParts() {
            boneReference = GetComponentInChildren<BoneReferences>();
            parts = boneReference
                 .GetAllBones()
                 .Where(x => x.bone.GetComponent<Collider>() != null)
                 .Select(x => new Instance(x.bone))
                 .ToArray();
        }

        internal void AddParts(RagdollProfile profile) {
            boneReference = GetComponentInChildren<BoneReferences>();
            foreach(var bone in boneReference.GetAllBones()) {
                var col = bone.bone.GetComponent<Collider>();
                if(col) {
                    var joint = bone.bone.GetComponent<Joint>();
                    if(joint) {
                        DestroyImmediate(joint);
                    }
                    var body = bone.bone.GetComponent<Rigidbody>();
                    DestroyImmediate(body);
                    DestroyImmediate(col);
                }
            }
            foreach(var info in profile.Information) {
                var transform = boneReference.GetBone(info.BoneId);
                info.AssignTo(transform);
            }
            parts = boneReference
                 .GetAllBones()
                 .Where(x => x.bone.GetComponent<Collider>() != null)
                 .Select(x => new Instance(x.bone))
                 .ToArray();
        }

        [ContextMenu("Clear Parts")]
        internal void ClearParts() {
            boneReference = GetComponentInChildren<BoneReferences>();
            boneReference
                 .GetAllBones()
                 .Foreach(x => {
                     var col = x.bone.GetComponent<Collider>();
                     if(col) {
                         var joint = col.GetComponent<Joint>();
                         if(joint)
                             DestroyImmediate(joint);
                         var body = col.GetComponent<Rigidbody>();
                         if(body)
                             DestroyImmediate(body);
                         DestroyImmediate(col);
                     }
                 });
            parts = new Instance[0];
        }

        [ContextMenu("Log non-part active colliders")]
        internal void FindNonPartActiveColliders() {
            var colliders = GetComponentsInChildren<Collider>();
            List<Collider> nonPartColliders = new List<Collider>();

            foreach(var c in colliders) {
                if(!parts.Any(x => x.Collider == c) && (c.enabled)) {
                    nonPartColliders.Add(c);
                }
            }

            if(nonPartColliders.Count > 0) {
                Debug.LogError($"{name} found non part colliders:\n{nonPartColliders.Select(x => x.name).CombineToString(true)}");
            }
        }

        #endregion

        private class AnimationData
        {
            public BonesSnapshot current;
            public BonesSnapshot target;
        }

        [System.Serializable]
        public class Instance
        {
            #region Variables

            [SerializeField] private Transform bone = null;

            [SerializeField] private bool disableCollider = true;
            [SerializeField] private Collider collider = null;

            [SerializeField] private bool disableRigidbody = true;
            [SerializeField] private Rigidbody body = null;

            #endregion

            #region Properties

            public Transform Bone => bone;
            public Collider Collider => collider;
            public Rigidbody Body => body;

            #endregion

            #region Constructor

            public Instance(Transform bone) {
                this.bone = bone;
                collider = bone.GetComponent<Collider>();
                body = bone.GetComponent<Rigidbody>();
            }

            #endregion

            #region Methods

            public void Unload() {
                if(disableCollider && collider)
                    collider.enabled = false;
                if(disableRigidbody && body) {
                    body.Freeze(true);
                }
            }

            public void Load() {
                if(disableCollider && collider)
                    collider.enabled = true;
                if(disableRigidbody && body) {
                    body.constraints = RigidbodyConstraints.None;
                }
            }

            #endregion
        }
    }
}
