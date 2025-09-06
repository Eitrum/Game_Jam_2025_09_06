using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public interface IJointSettings : IFixedJointSettings, ISpringJointSettings, IHingeJointSettings, ICharacterJointSettings, IConfigurableJointSettings
    {
        new JointType Type { get; set; }

        IFixedJointSettings FixedJointSettings { get; }
        ISpringJointSettings SpringJointSettings { get; }
        IHingeJointSettings HingeJointSettings { get; }
        ICharacterJointSettings CharacterJointSettings { get; }
        IConfigurableJointSettings ConfigurableJointSettings { get; }

        void Copy<T>(T joint) where T : Joint;
        void ApplyTo<T>(T joint) where T : Joint;
        new Joint AddJointToGameObject(GameObject go);
    }

    public interface IFixedJointSettings
    {
        JointType Type { get; }
        JointConnectedBodyMode ConnectedBody { get; set; }
        float BreakForce { get; set; }
        float BreakTorque { get; set; }
        bool EnableCollision { get; set; } // Flag 1
        bool EnablePreprocessing { get; set; } // Flag 2

        float MassScale { get; set; }
        float ConnectedMassScale { get; set; }

        void Copy(FixedJoint joint);
        void ApplyTo(FixedJoint joint);
        FixedJoint AddJointToGameObject(GameObject go);
    }

    public interface ISpringJointSettings
    {
        JointType Type { get; }
        JointConnectedBodyMode ConnectedBody { get; set; }
        float BreakForce { get; set; }
        float BreakTorque { get; set; }
        bool EnableCollision { get; set; } // Flag 1
        bool EnablePreprocessing { get; set; } // Flag 2

        Vector3 Anchor { get; set; }
        bool AutoConfigureConnectedAnchor { get; set; } // Flag 3
        Vector3 ConnectedAnchor { get; set; }
        float Spring { get; set; }
        float Damper { get; set; }
        MinMax Distance { get; set; }
        float Tolerance { get; set; }

        float MassScale { get; set; }
        float ConnectedMassScale { get; set; }

        void Copy(SpringJoint joint);
        void ApplyTo(SpringJoint joint);
        SpringJoint AddJointToGameObject(GameObject go);
    }

    public interface IHingeJointSettings
    {
        JointType Type { get; }
        JointConnectedBodyMode ConnectedBody { get; set; }
        float BreakForce { get; set; }
        float BreakTorque { get; set; }
        bool EnableCollision { get; set; } // Flag 1
        bool EnablePreprocessing { get; set; } // Flag 2

        Vector3 Anchor { get; set; }
        Vector3 Axis { get; set; }
        bool AutoConfigureConnectedAnchor { get; set; } // Flag 3
        Vector3 ConnectedAnchor { get; set; }
        bool UseSpring { get; set; } // Flag 4
        JointSpring Spring { get; set; }
        bool UseMotor { get; set; } // Flag 5
        JointMotor Motor { get; set; }
        bool UseLimits { get; set; } // Flag 6
        JointLimits Limits { get; set; }

        float MassScale { get; set; }
        float ConnectedMassScale { get; set; }

        void Copy(HingeJoint joint);
        void ApplyTo(HingeJoint joint);
        HingeJoint AddJointToGameObject(GameObject go);
    }

    public interface ICharacterJointSettings
    {
        JointType Type { get; }
        JointConnectedBodyMode ConnectedBody { get; set; }
        float BreakForce { get; set; }
        float BreakTorque { get; set; }
        bool EnableCollision { get; set; } // Flag 1
        bool EnablePreprocessing { get; set; } // Flag 2

        Vector3 Anchor { get; set; }
        Vector3 Axis { get; set; }
        bool AutoConfigureConnectedAnchor { get; set; } // Flag 3
        Vector3 ConnectedAnchor { get; set; }
        Vector3 SwingAxis { get; set; }
        SoftJointLimitSpring TwistLimitSpring { get; set; }
        SoftJointLimit LowTwistLimit { get; set; }
        SoftJointLimit HighTwistLimit { get; set; }
        SoftJointLimitSpring SwingLimitSpring { get; set; }
        SoftJointLimit Swing1Limit { get; set; }
        SoftJointLimit Swing2Limit { get; set; }
        bool EnableProjection { get; set; } // Flag 7
        float ProjectionDistance { get; set; }
        float ProjectionAngle { get; set; }

        float MassScale { get; set; }
        float ConnectedMassScale { get; set; }

        void Copy(CharacterJoint joint);
        void ApplyTo(CharacterJoint joint);
        CharacterJoint AddJointToGameObject(GameObject go);
    }

    public interface IConfigurableJointSettings
    {
        JointType Type { get; }
        JointConnectedBodyMode ConnectedBody { get; set; }
        float BreakForce { get; set; }
        float BreakTorque { get; set; }
        bool EnableCollision { get; set; } // Flag 1
        bool EnablePreprocessing { get; set; } // Flag 2

        Vector3 Anchor { get; set; }
        Vector3 Axis { get; set; }
        bool AutoConfigureConnectedAnchor { get; set; } // Flag 3
        Vector3 ConnectedAnchor { get; set; }
        Vector3 SecondaryAxis { get; set; }

        ConfigurableJointMotion XMotion { get; set; }
        ConfigurableJointMotion YMotion { get; set; }
        ConfigurableJointMotion ZMotion { get; set; }
        ConfigurableJointMotion AngularXMotion { get; set; }
        ConfigurableJointMotion AngularYMotion { get; set; }
        ConfigurableJointMotion AngularZMotion { get; set; }

        SoftJointLimitSpring LinearLimitSpring { get; set; }
        SoftJointLimit LinearLimit { get; set; }

        SoftJointLimitSpring AngularXLimitSpring { get; set; }
        SoftJointLimit LowAngularXLimit { get; set; }
        SoftJointLimit HighAngularXLimit { get; set; }
        SoftJointLimitSpring AngularYZLimitSpring { get; set; }
        SoftJointLimit AngularYLimit { get; set; }
        SoftJointLimit AngularZLimit { get; set; }

        Vector3 TargetPosition { get; set; }
        Vector3 TargetVelocity { get; set; }

        JointDrive XDrive { get; set; }
        JointDrive YDrive { get; set; }
        JointDrive ZDrive { get; set; }

        Quaternion TargetRotation { get; set; }
        Vector3 TargetAngularVelocity { get; set; }
        RotationDriveMode RotationDriveMode { get; set; }

        JointDrive AngularXDrive { get; set; }
        JointDrive AngularYZDrive { get; set; }
        JointDrive SlerpDrive { get; set; }

        JointProjectionMode ProjectionMode { get; set; }
        float ProjectionDistance { get; set; }
        float ProjectionAngle { get; set; }

        bool ConfiguredInWorldSpace { get; set; } // Flag 8
        bool SwapBodies { get; set; } // Flag 9

        float MassScale { get; set; }
        float ConnectedMassScale { get; set; }

        void Copy(ConfigurableJoint joint);
        void ApplyTo(ConfigurableJoint joint);
        ConfigurableJoint AddJointToGameObject(GameObject go);
    }
}
