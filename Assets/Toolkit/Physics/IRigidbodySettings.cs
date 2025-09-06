using UnityEngine;

namespace Toolkit.PhysicEx
{
    public interface IRigidbodySettings
    {
        float Mass { get; set; }
        float Drag { get; set; }
        float AngularDrag { get; set; }

        bool UseGravity { get; set; }
        bool IsKinematic { get; set; }
        bool DetectCollisions { get; set; }

        RigidbodyInterpolation Interpolation { get; set; }
        CollisionDetectionMode CollisionDetection { get; set; }
        RigidbodyConstraints Constraints { get; set; }

        void Copy(Rigidbody body);
        void ApplyTo(Rigidbody body);
    }
}
