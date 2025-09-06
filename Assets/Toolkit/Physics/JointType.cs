using UnityEngine;

namespace Toolkit.PhysicEx
{
    public enum JointType
    {
        None = 0,
        FixedJoint = 1,
        SpringJoint = 2,
        HingeJoint = 3,
        CharacterJoint = 4,
        ConfigurableJoint = 5,
    }

    public enum JointConnectedBodyMode
    {
        None,
        Parent,
        World,
    }

    public static class JointUtility
    {
        public static bool IsJoint<T>(this T joint, JointType type) where T : Joint {
            switch(type) {
                case JointType.FixedJoint: return joint is FixedJoint;
                case JointType.SpringJoint: return joint is SpringJoint;
                case JointType.HingeJoint: return joint is HingeJoint;
                case JointType.CharacterJoint: return joint is CharacterJoint;
                case JointType.ConfigurableJoint: return joint is ConfigurableJoint;
            }
            return false;
        }
    }
}
