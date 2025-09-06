using UnityEngine;

namespace Toolkit.PhysicEx
{
    public interface IColliderSettings
    {
        ColliderType Type { get; set; }

        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }
        Vector3 Center { get; set; }

        Vector3 Size { get; set; }
        float Radius { get; set; }
        float Height { get; set; }
        CapsuleDirection Direction { get; set; }

        Mesh Mesh { get; set; }
        bool Convex { get; set; }
        MeshColliderCookingOptions CookingOptions { get; set; }
        TerrainData Terrain { get; set; }

        float WheelMass { get; set; }
        float WheelDampingRate { get; set; }
        float SuspensionDistance { get; set; }
        float ForceAppPointDistance { get; set; }
        JointSpring WheelSpring { get; set; }
        WheelFrictionCurve WheelForwardFriction { get; set; }
        WheelFrictionCurve WheelSideFriction { get; set; }

        void Copy<T>(T col) where T : Collider;
        void ApplyTo<T>(T col) where T : Collider;
        Collider AddColliderToGameObject(GameObject go);
    }

    public interface IBoxColliderSettings
    {
        ColliderType Type { get; }
        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }
        Vector3 Center { get; set; }
        Vector3 Size { get; set; }

        void Copy(BoxCollider col);
        void ApplyTo(BoxCollider col);
        BoxCollider AddColliderToGameObject(GameObject go);
    }

    public interface ISphereColliderSettings
    {
        ColliderType Type { get; }
        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }
        Vector3 Center { get; set; }
        float Radius { get; set; }

        void Copy(SphereCollider col);
        void ApplyTo(SphereCollider col);
        SphereCollider AddColliderToGameObject(GameObject go);
    }

    public interface ICapsuleColliderSettings
    {
        ColliderType Type { get; }
        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }
        Vector3 Center { get; set; }
        float Radius { get; set; }
        float Height { get; set; }
        CapsuleDirection Direction { get; set; }

        void Copy(CapsuleCollider col);
        void ApplyTo(CapsuleCollider col);
        CapsuleCollider AddColliderToGameObject(GameObject go);
    }

    public interface IMeshColliderSettings
    {
        ColliderType Type { get; }
        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }
        Mesh Mesh { get; set; }
        bool Convex { get; set; }
        MeshColliderCookingOptions CookingOptions { get; set; }

        void Copy(MeshCollider col);
        void ApplyTo(MeshCollider col);
        MeshCollider AddColliderToGameObject(GameObject go);
    }

    public interface IWheelColliderSettings
    {
        ColliderType Type { get; }
        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }

        float WheelMass { get; set; }
        float WheelDampingRate { get; set; }
        float SuspensionDistance { get; set; }
        float ForceAppPointDistance { get; set; }
        JointSpring WheelSpring { get; set; }
        WheelFrictionCurve WheelForwardFriction { get; set; }
        WheelFrictionCurve WheelSideFriction { get; set; }

        void Copy(WheelCollider col);
        void ApplyTo(WheelCollider col);
        WheelCollider AddColliderToGameObject(GameObject go);
    }

    public interface ITerrainColliderSettings
    {
        ColliderType Type { get; }
        bool IsTrigger { get; set; }
        float ContactOffset { get; set; }
        PhysicsMaterial Material { get; set; }
        TerrainData Terrain { get; set; }

        void Copy(TerrainCollider col);
        void ApplyTo(TerrainCollider col);
        TerrainCollider AddColliderToGameObject(GameObject go);
    }
}
