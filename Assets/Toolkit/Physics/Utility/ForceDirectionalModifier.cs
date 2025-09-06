using System;
using UnityEngine;

namespace Toolkit.PhysicEx.Utility
{
    [CreateAssetMenu(fileName = "Force Directional Modifier", menuName = "Toolkit/Physics/Force Modifier/DIrectional")]
    public class ForceDirectionalModifier : ScriptableObject, IForceModifier
    {
        #region Variables

        [SerializeField, Direction] private Vector3 direction = new Vector3(0, 1, 0);
        [SerializeField] private ForceMode mode = ForceMode.Acceleration;
        [SerializeField] private Space relative = Space.Self;
        [SerializeField] private float strength = 10f;

        #endregion

        #region Modify

        public void Modify(Rigidbody body, IForceArea area, float multiplier) {
            body.AddForce((relative == Space.Self ? area.Location.rotation * direction : direction) * (strength * multiplier), mode);
        }

        #endregion
    }
}
