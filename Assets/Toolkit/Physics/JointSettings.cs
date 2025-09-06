using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [System.Serializable]
    public class JointSettings : IJointSettings
    {
        #region Variables

        [SerializeField] private JointType type = JointType.None;
        [SerializeField] private JointConnectedBodyMode connectedBodyMode = JointConnectedBodyMode.None;
        [SerializeField] private float breakForce = float.PositiveInfinity;
        [SerializeField] private float breakTorque = float.PositiveInfinity;
        [SerializeField] private float massScale = 1f;
        [SerializeField] private float connectedMassScale = 1f;
        [SerializeField] private int flag = 0;
        [SerializeField] private float[] values = new float[0];

        #endregion

        #region Default Properties

        public JointType Type {
            get => type;
            set {
                if(type != value) {
                    type = value;
                    switch(type) {
                        case JointType.FixedJoint: values = new float[0]; break;
                        case JointType.SpringJoint: values = new float[11]; break;
                        case JointType.HingeJoint: values = new float[16]; break;
                        case JointType.CharacterJoint: values = new float[30]; break;
                        case JointType.ConfigurableJoint: values = new float[77]; break;
                        default: values = new float[0]; break;
                    }
                }
            }
        }

        public JointConnectedBodyMode ConnectedBody { get => connectedBodyMode; set => connectedBodyMode = value; }
        public float BreakForce { get => breakForce; set => breakForce = value; }
        public float BreakTorque { get => breakTorque; set => breakTorque = value; }
        public bool EnableCollision { get => flag.HasFlag(1); set => flag.SetFlag(1, value); }
        public bool EnablePreprocessing { get => flag.HasFlag(2); set => flag.SetFlag(2, value); }

        public float MassScale { get => massScale; set => massScale = value; }
        public float ConnectedMassScale { get => connectedMassScale; set => connectedMassScale = value; }

        public IFixedJointSettings FixedJointSettings {
            get {
                if(type != JointType.FixedJoint)
                    Type = JointType.FixedJoint;
                return this as IFixedJointSettings;
            }
        }
        public ISpringJointSettings SpringJointSettings {
            get {
                if(type != JointType.SpringJoint)
                    Type = JointType.SpringJoint;
                return this as ISpringJointSettings;
            }
        }
        public IHingeJointSettings HingeJointSettings {
            get {
                if(type != JointType.HingeJoint)
                    Type = JointType.HingeJoint;
                return this as IHingeJointSettings;
            }
        }
        public ICharacterJointSettings CharacterJointSettings {
            get {
                if(type != JointType.CharacterJoint)
                    Type = JointType.CharacterJoint;
                return this as ICharacterJointSettings;
            }
        }
        public IConfigurableJointSettings ConfigurableJointSettings {
            get {
                if(type != JointType.ConfigurableJoint)
                    Type = JointType.ConfigurableJoint;
                return this as IConfigurableJointSettings;
            }
        }

        #endregion

        #region Spring Joint

        Vector3 ISpringJointSettings.Anchor {
            get => new Vector3(values[0], values[1], values[2]);
            set {
                values[0] = value.x;
                values[1] = value.y;
                values[2] = value.z;
            }
        }
        bool ISpringJointSettings.AutoConfigureConnectedAnchor { get => flag.HasFlag(4); set => flag.SetFlag(4, value); } // Flag 4
        Vector3 ISpringJointSettings.ConnectedAnchor {
            get => new Vector3(values[3], values[4], values[5]);
            set {
                values[3] = value.x;
                values[4] = value.y;
                values[5] = value.z;
            }
        }
        float ISpringJointSettings.Spring { get => values[6]; set => values[6] = value; }
        float ISpringJointSettings.Damper { get => values[7]; set => values[7] = value; }
        MinMax ISpringJointSettings.Distance {
            get => new MinMax(values[8], values[9]);
            set {
                values[8] = value.min;
                values[9] = value.max;
            }
        }
        float ISpringJointSettings.Tolerance { get => values[10]; set => values[10] = value; }

        #endregion

        #region Hinge Joint // Flag 8, 16, 32, 64

        Vector3 IHingeJointSettings.Anchor {
            get => new Vector3(values[0], values[1], values[2]);
            set {
                values[0] = value.x;
                values[1] = value.y;
                values[2] = value.z;
            }
        }
        bool IHingeJointSettings.AutoConfigureConnectedAnchor { get => flag.HasFlag(4); set => flag.SetFlag(4, value); } // Flag 4
        Vector3 IHingeJointSettings.ConnectedAnchor {
            get => new Vector3(values[3], values[4], values[5]);
            set {
                values[3] = value.x;
                values[4] = value.y;
                values[5] = value.z;
            }
        }
        Vector3 IHingeJointSettings.Axis {
            get => new Vector3(values[6], values[7], values[8]);
            set {
                values[6] = value.x;
                values[7] = value.y;
                values[8] = value.z;
            }
        }
        bool IHingeJointSettings.UseSpring { get => flag.HasFlag(8); set => flag.SetFlag(8, value); }
        JointSpring IHingeJointSettings.Spring {
            get {
                return new JointSpring() {
                    spring = values[6],
                    damper = values[7],
                    targetPosition = values[8],
                };
            }
            set {
                values[6] = value.spring;
                values[7] = value.damper;
                values[8] = value.targetPosition;
            }
        }
        bool IHingeJointSettings.UseMotor { get => flag.HasFlag(16); set => flag.SetFlag(16, value); }
        JointMotor IHingeJointSettings.Motor {
            get {
                return new JointMotor() {
                    force = values[9],
                    targetVelocity = values[10],
                    freeSpin = flag.HasFlag(64)
                };
            }
            set {
                values[9] = value.force;
                values[10] = value.targetVelocity;
                flag.SetFlag(64, value.freeSpin);
            }
        }
        bool IHingeJointSettings.UseLimits { get => flag.HasFlag(32); set => flag.SetFlag(32, value); }
        JointLimits IHingeJointSettings.Limits {
            get {
                return new JointLimits() {
                    min = values[11],
                    max = values[12],
                    bounciness = values[13],
                    bounceMinVelocity = values[14],
                    contactDistance = values[15]
                };
            }
            set {
                values[11] = value.min;
                values[12] = value.max;
                values[13] = value.bounciness;
                values[14] = value.bounceMinVelocity;
                values[15] = value.contactDistance;
            }
        }

        #endregion

        #region Character Joint // Flag 128

        Vector3 ICharacterJointSettings.Anchor {
            get => new Vector3(values[0], values[1], values[2]);
            set {
                values[0] = value.x;
                values[1] = value.y;
                values[2] = value.z;
            }
        }
        bool ICharacterJointSettings.AutoConfigureConnectedAnchor { get => flag.HasFlag(4); set => flag.SetFlag(4, value); } // Flag 4
        Vector3 ICharacterJointSettings.ConnectedAnchor {
            get => new Vector3(values[3], values[4], values[5]);
            set {
                values[3] = value.x;
                values[4] = value.y;
                values[5] = value.z;
            }
        }
        Vector3 ICharacterJointSettings.Axis {
            get => new Vector3(values[6], values[7], values[8]);
            set {
                values[6] = value.x;
                values[7] = value.y;
                values[8] = value.z;
            }
        }
        Vector3 ICharacterJointSettings.SwingAxis {
            get => new Vector3(values[9], values[10], values[11]);
            set {
                values[9] = value.x;
                values[10] = value.y;
                values[11] = value.z;
            }
        }
        SoftJointLimitSpring ICharacterJointSettings.TwistLimitSpring {
            get => new SoftJointLimitSpring() {
                spring = values[12],
                damper = values[13],
            };
            set {
                values[12] = value.spring;
                values[13] = value.damper;
            }
        }
        SoftJointLimit ICharacterJointSettings.LowTwistLimit {
            get => new SoftJointLimit() {
                limit = values[14],
                bounciness = values[15],
                contactDistance = values[16],
            };
            set {
                values[14] = value.limit;
                values[15] = value.bounciness;
                values[16] = value.contactDistance;
            }
        }
        SoftJointLimit ICharacterJointSettings.HighTwistLimit {
            get => new SoftJointLimit() {
                limit = values[17],
                bounciness = values[18],
                contactDistance = values[19],
            };
            set {
                values[17] = value.limit;
                values[18] = value.bounciness;
                values[19] = value.contactDistance;
            }
        }
        SoftJointLimitSpring ICharacterJointSettings.SwingLimitSpring {
            get => new SoftJointLimitSpring() {
                spring = values[20],
                damper = values[21],
            };
            set {
                values[20] = value.spring;
                values[21] = value.damper;
            }
        }
        SoftJointLimit ICharacterJointSettings.Swing1Limit {
            get => new SoftJointLimit() {
                limit = values[22],
                bounciness = values[23],
                contactDistance = values[24],
            };
            set {
                values[22] = value.limit;
                values[23] = value.bounciness;
                values[24] = value.contactDistance;
            }
        }
        SoftJointLimit ICharacterJointSettings.Swing2Limit {
            get => new SoftJointLimit() {
                limit = values[25],
                bounciness = values[26],
                contactDistance = values[27],
            };
            set {
                values[25] = value.limit;
                values[26] = value.bounciness;
                values[27] = value.contactDistance;
            }
        }
        bool ICharacterJointSettings.EnableProjection { get => flag.HasFlag(128); set => flag.SetFlag(128, value); } // Flag 128
        float ICharacterJointSettings.ProjectionDistance { get => values[28]; set => values[28] = value; }
        float ICharacterJointSettings.ProjectionAngle { get => values[29]; set => values[29] = value; }

        #endregion

        #region Configurable Joint // Flag 256, 512

        Vector3 IConfigurableJointSettings.Anchor {
            get => new Vector3(values[0], values[1], values[2]);
            set {
                values[0] = value.x;
                values[1] = value.y;
                values[2] = value.z;
            }
        }
        Vector3 IConfigurableJointSettings.Axis {
            get => new Vector3(values[3], values[4], values[5]);
            set {
                values[3] = value.x;
                values[4] = value.y;
                values[5] = value.z;
            }
        }
        bool IConfigurableJointSettings.AutoConfigureConnectedAnchor { get => flag.HasFlag(4); set => flag.SetFlag(4, value); } // Flag 4
        Vector3 IConfigurableJointSettings.ConnectedAnchor {
            get => new Vector3(values[6], values[7], values[8]);
            set {
                values[6] = value.x;
                values[7] = value.y;
                values[8] = value.z;
            }
        }
        Vector3 IConfigurableJointSettings.SecondaryAxis {
            get => new Vector3(values[9], values[10], values[11]);
            set {
                values[9] = value.x;
                values[10] = value.y;
                values[11] = value.z;
            }
        }

        ConfigurableJointMotion IConfigurableJointSettings.XMotion { get => (ConfigurableJointMotion)Mathf.RoundToInt(values[12]); set => values[12] = (int)value; }
        ConfigurableJointMotion IConfigurableJointSettings.YMotion { get => (ConfigurableJointMotion)Mathf.RoundToInt(values[13]); set => values[13] = (int)value; }
        ConfigurableJointMotion IConfigurableJointSettings.ZMotion { get => (ConfigurableJointMotion)Mathf.RoundToInt(values[14]); set => values[14] = (int)value; }
        ConfigurableJointMotion IConfigurableJointSettings.AngularXMotion { get => (ConfigurableJointMotion)Mathf.RoundToInt(values[15]); set => values[15] = (int)value; }
        ConfigurableJointMotion IConfigurableJointSettings.AngularYMotion { get => (ConfigurableJointMotion)Mathf.RoundToInt(values[16]); set => values[16] = (int)value; }
        ConfigurableJointMotion IConfigurableJointSettings.AngularZMotion { get => (ConfigurableJointMotion)Mathf.RoundToInt(values[17]); set => values[17] = (int)value; }


        SoftJointLimitSpring IConfigurableJointSettings.LinearLimitSpring {
            get => new SoftJointLimitSpring() {
                spring = values[18],
                damper = values[19],
            };
            set {
                values[18] = value.spring;
                values[19] = value.damper;
            }
        }
        SoftJointLimit IConfigurableJointSettings.LinearLimit {
            get => new SoftJointLimit() {
                limit = values[20],
                bounciness = values[21],
                contactDistance = values[22],
            };
            set {
                values[20] = value.limit;
                values[21] = value.bounciness;
                values[22] = value.contactDistance;
            }
        }
        SoftJointLimitSpring IConfigurableJointSettings.AngularXLimitSpring {
            get => new SoftJointLimitSpring() {
                spring = values[23],
                damper = values[24],
            };
            set {
                values[23] = value.spring;
                values[24] = value.damper;
            }
        }
        SoftJointLimit IConfigurableJointSettings.LowAngularXLimit {
            get => new SoftJointLimit() {
                limit = values[25],
                bounciness = values[26],
                contactDistance = values[27],
            };
            set {
                values[25] = value.limit;
                values[26] = value.bounciness;
                values[27] = value.contactDistance;
            }
        }
        SoftJointLimit IConfigurableJointSettings.HighAngularXLimit {
            get => new SoftJointLimit() {
                limit = values[28],
                bounciness = values[29],
                contactDistance = values[30],
            };
            set {
                values[28] = value.limit;
                values[29] = value.bounciness;
                values[30] = value.contactDistance;
            }
        }
        SoftJointLimitSpring IConfigurableJointSettings.AngularYZLimitSpring {
            get => new SoftJointLimitSpring() {
                spring = values[31],
                damper = values[32],
            };
            set {
                values[31] = value.spring;
                values[32] = value.damper;
            }
        }
        SoftJointLimit IConfigurableJointSettings.AngularYLimit {
            get => new SoftJointLimit() {
                limit = values[33],
                bounciness = values[34],
                contactDistance = values[35],
            };
            set {
                values[36] = value.limit;
                values[37] = value.bounciness;
                values[38] = value.contactDistance;
            }
        }
        SoftJointLimit IConfigurableJointSettings.AngularZLimit {
            get => new SoftJointLimit() {
                limit = values[39],
                bounciness = values[40],
                contactDistance = values[41],
            };
            set {
                values[39] = value.limit;
                values[40] = value.bounciness;
                values[41] = value.contactDistance;
            }
        }


        Vector3 IConfigurableJointSettings.TargetPosition {
            get => new Vector3(values[42], values[43], values[44]);
            set {
                values[42] = value.x;
                values[43] = value.y;
                values[44] = value.z;
            }
        }
        Vector3 IConfigurableJointSettings.TargetVelocity {
            get => new Vector3(values[45], values[46], values[47]);
            set {
                values[45] = value.x;
                values[46] = value.y;
                values[47] = value.z;
            }
        }
        JointDrive IConfigurableJointSettings.XDrive {
            get => new JointDrive() {
                positionSpring = values[48],
                positionDamper = values[49],
                maximumForce = values[50],
            };
            set {
                values[48] = value.positionSpring;
                values[49] = value.positionDamper;
                values[50] = value.maximumForce;
            }
        }
        JointDrive IConfigurableJointSettings.YDrive {
            get => new JointDrive() {
                positionSpring = values[51],
                positionDamper = values[52],
                maximumForce = values[53],
            };
            set {
                values[51] = value.positionSpring;
                values[52] = value.positionDamper;
                values[53] = value.maximumForce;
            }
        }
        JointDrive IConfigurableJointSettings.ZDrive {
            get => new JointDrive() {
                positionSpring = values[54],
                positionDamper = values[55],
                maximumForce = values[56],
            };
            set {
                values[54] = value.positionSpring;
                values[55] = value.positionDamper;
                values[56] = value.maximumForce;
            }
        }
        Quaternion IConfigurableJointSettings.TargetRotation {
            get => new Quaternion(values[57], values[58], values[59], values[60]);
            set {
                values[57] = value.x;
                values[58] = value.y;
                values[59] = value.z;
                values[60] = value.w;
            }
        }
        Vector3 IConfigurableJointSettings.TargetAngularVelocity {
            get => new Vector3(values[61], values[62], values[63]);
            set {
                values[61] = value.x;
                values[62] = value.y;
                values[63] = value.z;
            }
        }
        RotationDriveMode IConfigurableJointSettings.RotationDriveMode { get => (RotationDriveMode)Mathf.RoundToInt(values[64]); set => values[64] = (int)value; }
        JointDrive IConfigurableJointSettings.AngularXDrive {
            get => new JointDrive() {
                positionSpring = values[65],
                positionDamper = values[66],
                maximumForce = values[67],
            };
            set {
                values[65] = value.positionSpring;
                values[66] = value.positionDamper;
                values[67] = value.maximumForce;
            }
        }
        JointDrive IConfigurableJointSettings.AngularYZDrive {
            get => new JointDrive() {
                positionSpring = values[68],
                positionDamper = values[69],
                maximumForce = values[70],
            };
            set {
                values[68] = value.positionSpring;
                values[69] = value.positionDamper;
                values[70] = value.maximumForce;
            }
        }
        JointDrive IConfigurableJointSettings.SlerpDrive {
            get => new JointDrive() {
                positionSpring = values[71],
                positionDamper = values[72],
                maximumForce = values[73],
            };
            set {
                values[71] = value.positionSpring;
                values[72] = value.positionDamper;
                values[73] = value.maximumForce;
            }
        }
        JointProjectionMode IConfigurableJointSettings.ProjectionMode { get => (JointProjectionMode)Mathf.RoundToInt(values[74]); set => values[74] = (int)value; }
        float IConfigurableJointSettings.ProjectionDistance { get => values[75]; set => values[75] = value; }
        float IConfigurableJointSettings.ProjectionAngle { get => values[76]; set => values[76] = value; }
        bool IConfigurableJointSettings.ConfiguredInWorldSpace { get => flag.HasFlag(256); set => flag.SetFlag(256, value); } // Flag 256
        bool IConfigurableJointSettings.SwapBodies { get => flag.HasFlag(512); set => flag.SetFlag(512, value); } // Flag 512

        #endregion

        #region Apply To

        public void ApplyTo<T>(T joint) where T : Joint {
            switch(type) {
                case JointType.FixedJoint: ApplyTo(joint as FixedJoint); break;
                case JointType.SpringJoint: ApplyTo(joint as SpringJoint); break;
                case JointType.HingeJoint: ApplyTo(joint as HingeJoint); break;
                case JointType.CharacterJoint: ApplyTo(joint as CharacterJoint); break;
                case JointType.ConfigurableJoint: ApplyTo(joint as ConfigurableJoint); break;
            }
        }

        public void ApplyTo(FixedJoint joint) {
            if(type != JointType.FixedJoint)
                return;
            if(joint == null)
                return;
            joint.breakForce = BreakForce;
            joint.breakTorque = BreakTorque;
            joint.enableCollision = EnableCollision;
            joint.enablePreprocessing = EnablePreprocessing;
            joint.connectedMassScale = connectedMassScale;
            joint.massScale = massScale;

            if(connectedBodyMode == JointConnectedBodyMode.Parent)
                ApplyParentRigidbody(joint);
        }

        public void ApplyTo(SpringJoint joint) {
            if(type != JointType.SpringJoint)
                return;
            if(joint == null)
                return;
            var sj = SpringJointSettings;
            joint.breakForce = BreakForce;
            joint.breakTorque = BreakTorque;
            joint.enableCollision = EnableCollision;
            joint.enablePreprocessing = EnablePreprocessing;
            joint.connectedMassScale = connectedMassScale;
            joint.massScale = massScale;

            joint.anchor = sj.Anchor;
            joint.autoConfigureConnectedAnchor = sj.AutoConfigureConnectedAnchor;
            joint.connectedAnchor = sj.ConnectedAnchor;
            joint.spring = sj.Spring;
            joint.damper = sj.Damper;
            var dist = sj.Distance;
            joint.minDistance = dist.min;
            joint.maxDistance = dist.max;
            joint.tolerance = sj.Tolerance;

            if(connectedBodyMode == JointConnectedBodyMode.Parent)
                ApplyParentRigidbody(joint);
        }

        public void ApplyTo(HingeJoint joint) {
            if(type != JointType.HingeJoint)
                return;
            if(joint == null)
                return;
            var hj = HingeJointSettings;
            joint.breakForce = BreakForce;
            joint.breakTorque = BreakTorque;
            joint.enableCollision = EnableCollision;
            joint.enablePreprocessing = EnablePreprocessing;
            joint.connectedMassScale = connectedMassScale;
            joint.massScale = massScale;

            joint.anchor = hj.Anchor;
            joint.axis = hj.Axis;
            joint.autoConfigureConnectedAnchor = hj.AutoConfigureConnectedAnchor;
            joint.connectedAnchor = hj.ConnectedAnchor;
            joint.useSpring = hj.UseSpring;
            joint.spring = hj.Spring;
            joint.useMotor = hj.UseMotor;
            joint.motor = hj.Motor;
            joint.useLimits = hj.UseLimits;
            joint.limits = hj.Limits;

            if(connectedBodyMode == JointConnectedBodyMode.Parent)
                ApplyParentRigidbody(joint);
        }

        public void ApplyTo(CharacterJoint joint) {
            if(type != JointType.CharacterJoint)
                return;
            if(joint == null)
                return;
            var cj = CharacterJointSettings;
            joint.breakForce = BreakForce;
            joint.breakTorque = BreakTorque;
            joint.enableCollision = EnableCollision;
            joint.enablePreprocessing = EnablePreprocessing;
            joint.connectedMassScale = connectedMassScale;
            joint.massScale = massScale;

            joint.anchor = cj.Anchor;
            joint.axis = cj.Axis;
            joint.autoConfigureConnectedAnchor = cj.AutoConfigureConnectedAnchor;
            joint.connectedAnchor = cj.ConnectedAnchor;
            joint.swingAxis = cj.SwingAxis;

            joint.twistLimitSpring = cj.TwistLimitSpring;
            joint.lowTwistLimit = cj.LowTwistLimit;
            joint.highTwistLimit = cj.HighTwistLimit;
            joint.swingLimitSpring = cj.SwingLimitSpring;
            joint.swing1Limit = cj.Swing1Limit;
            joint.swing2Limit = cj.Swing2Limit;
            joint.enableProjection = cj.EnableProjection;
            joint.projectionDistance = cj.ProjectionDistance;
            joint.projectionAngle = cj.ProjectionAngle;

            if(connectedBodyMode == JointConnectedBodyMode.Parent)
                ApplyParentRigidbody(joint);
        }

        public void ApplyTo(ConfigurableJoint joint) {
            if(type != JointType.ConfigurableJoint)
                return;
            if(joint == null)
                return;
            var cj = ConfigurableJointSettings;
            joint.breakForce = BreakForce;
            joint.breakTorque = BreakTorque;
            joint.enableCollision = EnableCollision;
            joint.enablePreprocessing = EnablePreprocessing;
            joint.connectedMassScale = connectedMassScale;
            joint.massScale = massScale;

            joint.anchor = cj.Anchor;
            joint.axis = cj.Axis;
            joint.autoConfigureConnectedAnchor = cj.AutoConfigureConnectedAnchor;
            joint.connectedAnchor = cj.ConnectedAnchor;
            joint.secondaryAxis = cj.SecondaryAxis;

            joint.xMotion = cj.XMotion;
            joint.yMotion = cj.YMotion;
            joint.zMotion = cj.ZMotion;
            joint.angularXMotion = cj.AngularXMotion;
            joint.angularYMotion = cj.AngularYMotion;
            joint.angularZMotion = cj.AngularZMotion;

            joint.linearLimitSpring = cj.LinearLimitSpring;
            joint.linearLimit = cj.LinearLimit;
            joint.angularXLimitSpring = cj.AngularXLimitSpring;
            joint.lowAngularXLimit = cj.LowAngularXLimit;
            joint.highAngularXLimit = cj.HighAngularXLimit;
            joint.angularYZLimitSpring = cj.AngularYZLimitSpring;
            joint.angularYLimit = cj.AngularYLimit;
            joint.angularZLimit = cj.AngularZLimit;

            joint.targetPosition = cj.TargetPosition;
            joint.targetVelocity = cj.TargetVelocity;

            joint.xDrive = cj.XDrive;
            joint.yDrive = cj.YDrive;
            joint.zDrive = cj.ZDrive;

            joint.targetRotation = cj.TargetRotation;
            joint.targetAngularVelocity = cj.TargetAngularVelocity;

            joint.angularXDrive = cj.AngularXDrive;
            joint.angularYZDrive = cj.AngularYZDrive;
            joint.slerpDrive = cj.SlerpDrive;

            joint.projectionMode = cj.ProjectionMode;
            joint.projectionDistance = cj.ProjectionDistance;
            joint.projectionAngle = cj.ProjectionAngle;

            joint.configuredInWorldSpace = cj.ConfiguredInWorldSpace;
            joint.swapBodies = cj.SwapBodies;

            if(connectedBodyMode == JointConnectedBodyMode.Parent)
                ApplyParentRigidbody(joint);
        }

        #endregion

        #region Copy

        public void Copy<T>(T joint) where T : Joint {
            if(joint == null)
                return;
            if(joint is FixedJoint fj) {
                Copy(fj);
            }
            else if(joint is SpringJoint sj) {
                Copy(sj);
            }
            else if(joint is HingeJoint hj) {
                Copy(hj);
            }
            else if(joint is CharacterJoint cj) {
                Copy(cj);
            }
            else if(joint is ConfigurableJoint cj2) {
                Copy(cj2);
            }
        }

        public void Copy(FixedJoint joint) {
            if(joint == null)
                return;
            Type = JointType.FixedJoint;
            connectedBodyMode = GetConnectedBodyMode(joint);
            var fj = FixedJointSettings;
            fj.BreakForce = joint.breakForce;
            fj.BreakTorque = joint.breakTorque;
            fj.EnableCollision = joint.enableCollision;
            fj.EnablePreprocessing = joint.enablePreprocessing;
            fj.MassScale = joint.massScale;
            fj.ConnectedMassScale = joint.connectedMassScale;
        }

        public void Copy(SpringJoint joint) {
            if(joint == null)
                return;
            Type = JointType.SpringJoint;
            connectedBodyMode = GetConnectedBodyMode(joint);
            var sj = SpringJointSettings;
            sj.BreakForce = joint.breakForce;
            sj.BreakTorque = joint.breakTorque;
            sj.EnableCollision = joint.enableCollision;
            sj.EnablePreprocessing = joint.enablePreprocessing;
            sj.MassScale = joint.massScale;
            sj.ConnectedMassScale = joint.connectedMassScale;

            sj.Anchor = joint.anchor;
            sj.AutoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            sj.ConnectedAnchor = joint.connectedAnchor;
            sj.Spring = joint.spring;
            sj.Damper = joint.damper;
            sj.Distance = new MinMax(joint.minDistance, joint.maxDistance);
            sj.Tolerance = joint.tolerance;
        }

        public void Copy(HingeJoint joint) {
            if(joint == null)
                return;
            Type = JointType.HingeJoint;
            connectedBodyMode = GetConnectedBodyMode(joint);
            var hj = HingeJointSettings;
            hj.BreakForce = joint.breakForce;
            hj.BreakTorque = joint.breakTorque;
            hj.EnableCollision = joint.enableCollision;
            hj.EnablePreprocessing = joint.enablePreprocessing;
            hj.ConnectedMassScale = joint.connectedMassScale;
            hj.MassScale = joint.massScale;

            hj.Anchor = joint.anchor;
            hj.Axis = joint.axis;
            hj.AutoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            hj.ConnectedAnchor = joint.connectedAnchor;
            hj.UseSpring = joint.useSpring;
            hj.Spring = joint.spring;
            hj.UseMotor = joint.useMotor;
            hj.Motor = joint.motor;
            hj.UseLimits = joint.useLimits;
            hj.Limits = joint.limits;
        }

        public void Copy(CharacterJoint joint) {
            if(joint == null)
                return;
            Type = JointType.CharacterJoint;
            connectedBodyMode = GetConnectedBodyMode(joint);
            var cj = CharacterJointSettings;
            cj.BreakForce = joint.breakForce;
            cj.BreakTorque = joint.breakTorque;
            cj.EnableCollision = joint.enableCollision;
            cj.EnablePreprocessing = joint.enablePreprocessing;
            cj.ConnectedMassScale = joint.connectedMassScale;
            cj.MassScale = joint.massScale;

            cj.Anchor = joint.anchor;
            cj.Axis = joint.axis;
            cj.AutoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            cj.ConnectedAnchor = joint.connectedAnchor;
            cj.SwingAxis = joint.swingAxis;

            cj.TwistLimitSpring = joint.twistLimitSpring;
            cj.LowTwistLimit = joint.lowTwistLimit;
            cj.HighTwistLimit = joint.highTwistLimit;
            cj.SwingLimitSpring = joint.swingLimitSpring;
            cj.Swing1Limit = joint.swing1Limit;
            cj.Swing2Limit = joint.swing2Limit;
            cj.EnableProjection = joint.enableProjection;
            cj.ProjectionDistance = joint.projectionDistance;
            cj.ProjectionAngle = joint.projectionAngle;
        }

        public void Copy(ConfigurableJoint joint) {
            if(joint == null)
                return;
            Type = JointType.ConfigurableJoint;
            connectedBodyMode = GetConnectedBodyMode(joint);
            var cj = ConfigurableJointSettings;
            cj.BreakForce = joint.breakForce;
            cj.BreakTorque = joint.breakTorque;
            cj.EnableCollision = joint.enableCollision;
            cj.EnablePreprocessing = joint.enablePreprocessing;
            cj.ConnectedMassScale = joint.connectedMassScale;
            cj.MassScale = joint.massScale;

            cj.Anchor = joint.anchor;
            cj.Axis = joint.axis;
            cj.AutoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            cj.ConnectedAnchor = joint.connectedAnchor;
            cj.SecondaryAxis = joint.secondaryAxis;

            cj.XMotion = joint.xMotion;
            cj.YMotion = joint.yMotion;
            cj.ZMotion = joint.zMotion;
            cj.AngularXMotion = joint.angularXMotion;
            cj.AngularYMotion = joint.angularYMotion;
            cj.AngularZMotion = joint.angularZMotion;

            cj.LinearLimitSpring = joint.linearLimitSpring;
            cj.LinearLimit = joint.linearLimit;
            cj.AngularXLimitSpring = joint.angularXLimitSpring;
            cj.LowAngularXLimit = joint.lowAngularXLimit;
            cj.HighAngularXLimit = joint.highAngularXLimit;
            cj.AngularYZLimitSpring = joint.angularYZLimitSpring;
            cj.AngularYLimit = joint.angularYLimit;
            cj.AngularZLimit = joint.angularZLimit;

            cj.TargetPosition = joint.targetPosition;
            cj.TargetVelocity = joint.targetVelocity;

            cj.XDrive = joint.xDrive;
            cj.YDrive = joint.yDrive;
            cj.ZDrive = joint.zDrive;

            cj.TargetRotation = joint.targetRotation;
            cj.TargetAngularVelocity = joint.targetAngularVelocity;

            cj.AngularXDrive = joint.angularXDrive;
            cj.AngularYZDrive = joint.angularYZDrive;
            cj.SlerpDrive = joint.slerpDrive;

            cj.ProjectionMode = joint.projectionMode;
            cj.ProjectionDistance = joint.projectionDistance;
            cj.ProjectionAngle = joint.projectionAngle;

            cj.ConfiguredInWorldSpace = joint.configuredInWorldSpace;
            cj.SwapBodies = joint.swapBodies;
        }

        #endregion

        #region Add Joint

        public Joint AddJointToGameObject(GameObject go) {
            switch(type) {
                case JointType.FixedJoint: return FixedJointSettings.AddJointToGameObject(go);
                case JointType.SpringJoint: return SpringJointSettings.AddJointToGameObject(go);
                case JointType.HingeJoint: return HingeJointSettings.AddJointToGameObject(go);
                case JointType.CharacterJoint: return CharacterJointSettings.AddJointToGameObject(go);
                case JointType.ConfigurableJoint: return ConfigurableJointSettings.AddJointToGameObject(go);
            }
            return null;
        }

        FixedJoint IFixedJointSettings.AddJointToGameObject(GameObject go) {
            if(type != JointType.FixedJoint)
                return null;
            var fj = go.AddComponent<FixedJoint>();
            FixedJointSettings.ApplyTo(fj);
            return fj;
        }

        SpringJoint ISpringJointSettings.AddJointToGameObject(GameObject go) {
            if(type != JointType.SpringJoint)
                return null;
            var sj = go.AddComponent<SpringJoint>();
            SpringJointSettings.ApplyTo(sj);
            return sj;
        }

        HingeJoint IHingeJointSettings.AddJointToGameObject(GameObject go) {
            if(type != JointType.HingeJoint)
                return null;
            var hj = go.AddComponent<HingeJoint>();
            HingeJointSettings.ApplyTo(hj);
            return hj;
        }

        CharacterJoint ICharacterJointSettings.AddJointToGameObject(GameObject go) {
            if(type != JointType.CharacterJoint)
                return null;
            var cj = go.AddComponent<CharacterJoint>();
            CharacterJointSettings.ApplyTo(cj);
            return cj;
        }

        ConfigurableJoint IConfigurableJointSettings.AddJointToGameObject(GameObject go) {
            if(type != JointType.ConfigurableJoint)
                return null;
            var cj = go.AddComponent<ConfigurableJoint>();
            ConfigurableJointSettings.ApplyTo(cj);
            return cj;
        }

        #endregion

        #region Utility

        private static void ApplyParentRigidbody(Joint joint) {
            var p = joint.transform.parent;
            if(p == null)
                return;
            joint.connectedBody = p.GetComponentInParent<Rigidbody>();
        }

        private static JointConnectedBodyMode GetConnectedBodyMode(Joint joint) {
            if(joint == null)
                return JointConnectedBodyMode.None;
            var body = joint.connectedBody;
            if(body == null)
                return JointConnectedBodyMode.World;
            var p = joint.transform.parent;
            if(p == null)
                return JointConnectedBodyMode.World;
            var pBody = p.GetComponentInParent<Rigidbody>();
            if(pBody == body)
                return JointConnectedBodyMode.Parent;
            else
                return JointConnectedBodyMode.None;
        }

        #endregion
    }
}
