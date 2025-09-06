using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolkit.PhysicEx
{
    [CreateAssetMenu(fileName = "Ragdoll Profile", menuName = "Toolkit/Physics/Ragdoll Profile")]
    public class RagdollProfile : ScriptableObject
    {
        #region Variables

        [SerializeField] private Data[] information = { };

        #endregion

        #region Properties

        public IReadOnlyList<Data> Information => information;

        #endregion

        #region Create

        public void CreateFrom(Toolkit.Utility.BoneReferences boneReference) {
            information = boneReference
                .GetAllBones()
                .Where(x => x.bone.GetComponent<Collider>() != null)
                .Select(x => new Data(x.bone.name, x.bone.GetComponent<Collider>(), x.bone.GetComponent<Rigidbody>(), x.bone.GetComponent<Joint>()))
                .ToArray();
            Debug.Log("Created a new information array for ragdoll profile: " + information.Length);
        }

        #endregion

        [Serializable]
        public class Data
        {
            #region Variables

            // Shared
            [SerializeField, Readonly] private string boneName = "";
            private int boneId;
            [SerializeField, Readonly] private bool useCollider = false;
            [SerializeField, Readonly] private ColliderSettings colliderSettings = new ColliderSettings();
            [SerializeField, Readonly] private bool useRigidbody = false;
            [SerializeField, Readonly] private RigidbodySettings rigidbodySettings = new RigidbodySettings();
            [SerializeField, Readonly] private bool useJoint = false;
            [SerializeField, Readonly] private JointSettings jointSettings = new JointSettings();

            #endregion

            #region Properties

            public int BoneId {
                get {
                    if(boneId == 0)
                        boneId = boneName.GetHash32();
                    return boneId;
                }
            }
            public bool UseCollider => useCollider;
            public IColliderSettings ColliderSettings => colliderSettings;
            public bool UseRigidbody => useRigidbody;
            public IRigidbodySettings RigidbodySettings => rigidbodySettings;
            public bool UseJoint => useJoint;
            public IJointSettings JointSettings => jointSettings;

            #endregion

            #region Constructor

            public Data() { }

            public Data(string boneName, Collider collider, Rigidbody body, Joint joint) {
                this.boneName = boneName;
                if(useCollider = (collider != null))
                    this.colliderSettings.Copy(collider);
                if(useRigidbody = (body != null))
                    this.rigidbodySettings.Copy(body);
                if(useJoint = (joint != null))
                    this.jointSettings.Copy(joint);
            }

            #endregion

            #region Assign to object (internal)

            internal void AssignTo(Transform bone) {
                if(useJoint) {
                    var joint = bone.GetComponent<Joint>();
                    if(!joint)
                        jointSettings.AddJointToGameObject(bone.gameObject);
                    else {
                        if(!joint.IsJoint(jointSettings.Type)) {
                            if(Application.isPlaying)
                                Destroy(joint);
                            else
                                DestroyImmediate(joint);
                            jointSettings.AddJointToGameObject(bone.gameObject);
                        }
                        else
                            jointSettings.ApplyTo(joint);
                    }
                }
                if(useRigidbody) {
                    var body = bone.GetComponent<Rigidbody>();
                    if(!body)
                        body = bone.AddComponent<Rigidbody>();
                    rigidbodySettings.ApplyTo(body);
                }
                if(useCollider) {
                    var col = bone.GetComponent<Collider>();
                    if(!col)
                        colliderSettings.AddColliderToGameObject(bone.gameObject);
                    else {
                        if(!ColliderUtility.IsCollider(col, colliderSettings.Type)) {
                            if(Application.isPlaying)
                                Destroy(col);
                            else
                                DestroyImmediate(col);
                            colliderSettings.AddColliderToGameObject(bone.gameObject);
                        }
                        else
                            colliderSettings.ApplyTo(col);
                    }
                }
            }

            #endregion
        }
    }
}
