using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public enum ColliderType
    {
        None = 0,
        Box = 1,
        Sphere = 2,
        Capsule = 3,
        Mesh = 4,
        Wheel = 5,
        Terrain = 6,
    }

    public enum Collider2DType
    {
        None = 0,
        Square = 1,
        Circle = 2,
        Polygon = 3,
        Edge = 4,
    }

    public enum CapsuleDirection
    {
        X = 0,
        Y = 1,
        Z = 2,
    }

    public static class ColliderUtility {
        public static bool IsCollider<T>(this T col, ColliderType type) where T : Collider {
            switch(type) {
                case ColliderType.Box: return col is BoxCollider;
                case ColliderType.Sphere: return col is SphereCollider;
                case ColliderType.Capsule: return col is CapsuleCollider;
                case ColliderType.Mesh: return col is MeshCollider;
                case ColliderType.Wheel: return col is WheelCollider;
                case ColliderType.Terrain: return col is TerrainCollider;
            }
            return false;
        }
    }
}
