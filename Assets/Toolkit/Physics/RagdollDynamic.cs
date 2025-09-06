using System.Collections;
using System.Collections.Generic;
using Toolkit.Utility;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [AddComponentMenu("Toolkit/Physics/Ragdoll (Dynamic)")]
    public class RagdollDynamic : MonoBehaviour, IRagdoll
    {
        #region Variables

        [SerializeField] private RagdollProfile profile = null;
        [Header("Restore")]
        [SerializeField] private Mathematics.Ease.Function easeFunction = Mathematics.Ease.Function.Linear;
        [SerializeField] private Mathematics.Ease.Type easeType = Mathematics.Ease.Type.InOut;

        private List<Instance> instances = new List<Instance>();

        private Animator animatorReference;
        private Animation animationReference;
        private BoneReferences boneReference;

        private Coroutine routine;

        #endregion

        #region Properties

        public bool IsInitialized => instances.Count > 0;
        public Rigidbody Root => instances.Count > 0 ? instances[0].Body : null;

        #endregion

        #region Init

        private void Awake() {
            animatorReference = GetComponentInParent<Animator>();
            animationReference = GetComponentInParent<Animation>();
            boneReference = GetComponentInChildren<BoneReferences>();

            foreach(var b in boneReference.GetAllBones()) {
                var col = b.bone.GetComponent<Collider>();
                if(col) {
                    var insta = new Instance(b.bone);
                    insta.AddCollider(col);
                    var body = col.GetComponent<Rigidbody>();
                    if(body)
                        insta.AddBody(body);
                    var joint = col.GetComponent<Joint>();
                    if(joint)
                        insta.AddJoint(joint);
                    insta.Destroy();
                }
            }
        }

        void OnDestroy() {
            if(routine != null)
                Timer.Stop(routine);
        }

        #endregion

        private void Update() {
            if(Input.GetKeyDown(KeyCode.Space)) {
                if(instances.Count == 0)
                    Initialize();
                else
                    Restore();
            }
        }

        #region Initialize

        public Rigidbody Initialize() {
            if(IsInitialized)
                return Root;
            if(animatorReference != null)
                animatorReference.enabled = false;
            if(animationReference != null)
                animationReference.enabled = false;

            foreach(var d in profile.Information) {
                var t = boneReference.GetBone(d.BoneId);
                var insta = new Instance(t);
                if(d.UseCollider)
                    insta.AddCollider(d.ColliderSettings.AddColliderToGameObject(t.gameObject));
                if(d.UseRigidbody) {
                    var body = t.gameObject.AddComponent<Rigidbody>();
                    d.RigidbodySettings.ApplyTo(body);
                    insta.AddBody(body);
                }
                if(d.UseJoint)
                    insta.AddJoint(d.JointSettings.AddJointToGameObject(t.gameObject));
                instances.Add(insta);
            }
            return instances[0].Body;
        }

        public Rigidbody Initialize(Vector3 direction, float impulseForce) {
            var body = Initialize();
            body.AddForce(direction * impulseForce, ForceMode.Impulse);
            return body;
        }

        #endregion

        #region Restore

        public void Restore(bool enableAnimation = true) {
            if(!IsInitialized)
                return;

            if(animatorReference != null)
                animatorReference.enabled = enableAnimation;
            if(animationReference != null)
                animationReference.enabled = enableAnimation;

            foreach(var i in instances)
                i.Destroy();

            instances.Clear();
        }

        public void Restore(AnimationClip clip, float duration) {
            if(!IsInitialized)
                return;

            var current = boneReference.Snapshot;
            clip.SampleAnimation(this.gameObject, 0f);
            var target = boneReference.Snapshot;

            foreach(var p in instances)
                p.Destroy();

            instances.Clear();

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

        private class AnimationData
        {
            public BonesSnapshot current;
            public BonesSnapshot target;
        }

        public class Instance
        {
            #region Variables

            [SerializeField] private Transform bone;
            [SerializeField] private bool keepCollider = true;
            [SerializeField] private Collider collider = null;
            [SerializeField] private bool keepBody = true;
            [SerializeField] private Rigidbody body = null;
            [SerializeField] private bool keepJoint = true;
            [SerializeField] private Joint joint = null;

            #endregion

            #region Properties

            public Transform Bone => bone;
            public Rigidbody Body => body;
            public Joint Joint => joint;
            public Collider Collider => collider;

            #endregion

            #region Constructor

            public Instance() { }
            public Instance(Transform bone) {
                this.bone = bone;
            }

            #endregion

            #region Add

            public void AddCollider(Collider col) {
                keepCollider = false;
                this.collider = col;
            }

            public void AddBody(Rigidbody body) {
                keepBody = false;
                this.body = body;
            }

            public void AddJoint(Joint joint) {
                keepJoint = false;
                this.joint = joint;
            }

            #endregion

            #region Destroy

            public void Destroy() {
                if(!keepJoint)
                    Component.Destroy(joint);
                if(!keepBody)
                    Component.Destroy(body);
                if(!keepCollider)
                    Component.Destroy(collider);
            }

            #endregion
        }
    }
}
