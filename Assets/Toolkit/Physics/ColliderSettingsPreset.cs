using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CreateAssetMenu(fileName = "Collider Settings", menuName = "Toolkit/Physics/Collider Settings")]
    public class ColliderSettingsPreset : ScriptableObject, IColliderSettings, IBoxColliderSettings, ISphereColliderSettings, ICapsuleColliderSettings, IMeshColliderSettings, IWheelColliderSettings, ITerrainColliderSettings
    {
        #region Variables

        [SerializeField] private ColliderType type = ColliderType.None;
        [SerializeField] private bool isTrigger = false;
        [SerializeField, Min(0f)] private float contactOffset = 0f;
        [SerializeField] private PhysicsMaterial material = null;
        [SerializeField] private Vector3 center = new Vector3(0, 0, 0);

        [SerializeField] private Vector3 generic = new Vector3(0, 0, 0);
        [SerializeField] private Mesh mesh = null;
        [SerializeField] private TerrainData terrainData = null;
        [SerializeField] private float[] wheelData = { };

        #endregion

        #region Properties

        // Shared
        public ColliderType Type { get => type; set => type = value; }
        public bool IsTrigger { get => isTrigger; set => isTrigger = value; }
        public float ContactOffset { get => contactOffset; set => contactOffset = Mathf.Max(0f, value); }
        public PhysicsMaterial Material { get => material; set => material = value; }
        public Vector3 Center { get => center; set => center = value; }

        // Primitive
        public Vector3 Size { get => generic; set => generic = value; }
        public float Radius { get => generic.x; set => generic.x = Mathf.Max(0f, value); }
        public float Height { get => generic.y; set => generic.y = Mathf.Max(0f, value); }
        public CapsuleDirection Direction {
            get {
                var dir = Mathf.RoundToInt(generic.z);
                if(dir < 1)
                    return CapsuleDirection.X;
                if(dir > 1)
                    return CapsuleDirection.Z;
                return CapsuleDirection.Y;
            }
            set => generic.z = Mathf.Clamp(value.ToInt(), 0, 2);
        }

        // Mesh / Terrain / Wheel
        public Mesh Mesh { get => mesh; set => mesh = value; }
        public bool Convex { get => Mathf.RoundToInt(generic.x) == 1; set => generic.x = value ? 1 : 0; }
        public MeshColliderCookingOptions CookingOptions { get => Mathf.RoundToInt(generic.y).ToEnum<MeshColliderCookingOptions>(); set => generic.y = value.ToInt(); }
        public TerrainData Terrain { get => terrainData; set => terrainData = value; }

        #endregion

        #region Wheel Properties

        public float WheelMass {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return wheelData[0];
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];
                wheelData[0] = value;
            }
        }

        public float WheelDampingRate {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return wheelData[1];
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];
                wheelData[1] = value;
            }
        }

        public float SuspensionDistance {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return wheelData[2];
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];
                wheelData[2] = value;
            }
        }

        public float ForceAppPointDistance {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return wheelData[3];
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];
                wheelData[3] = value;
            }
        }

        public JointSpring WheelSpring {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return new JointSpring() {
                    spring = wheelData[4],
                    damper = wheelData[5],
                    targetPosition = wheelData[6],
                };
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];
                wheelData[4] = value.spring;
                wheelData[5] = value.damper;
                wheelData[6] = value.targetPosition;
            }
        }

        public WheelFrictionCurve WheelForwardFriction {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return new WheelFrictionCurve() {
                    extremumSlip = wheelData[7],
                    extremumValue = wheelData[8],
                    asymptoteSlip = wheelData[9],
                    asymptoteValue = wheelData[10],
                    stiffness = wheelData[11],
                };
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];

                wheelData[7] = value.extremumSlip;
                wheelData[8] = value.extremumValue;
                wheelData[9] = value.asymptoteSlip;
                wheelData[10] = value.asymptoteValue;
                wheelData[11] = value.stiffness;
            }
        }

        public WheelFrictionCurve WheelSideFriction {
            get {
                if(wheelData == null || wheelData.Length == 0)
                    return default;
                return new WheelFrictionCurve() {
                    extremumSlip = wheelData[12],
                    extremumValue = wheelData[13],
                    asymptoteSlip = wheelData[14],
                    asymptoteValue = wheelData[15],
                    stiffness = wheelData[16],
                };
            }
            set {
                if(wheelData == null || wheelData.Length == 0)
                    wheelData = new float[17];

                wheelData[12] = value.extremumSlip;
                wheelData[13] = value.extremumValue;
                wheelData[14] = value.asymptoteSlip;
                wheelData[15] = value.asymptoteValue;
                wheelData[16] = value.stiffness;
            }
        }

        #endregion

        #region Copy

        public void Copy<T>(T col) where T : Collider {
            if(col == null) {
                type = ColliderType.None;
                return;
            }

            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;

            if(col is BoxCollider box) {
                type = ColliderType.Box;
                center = box.center;
                generic = box.size;
            }
            else if(col is SphereCollider sphere) {
                type = ColliderType.Sphere;
                center = sphere.center;
                generic = new Vector3(sphere.radius, 0f, 0f);
            }
            else if(col is CapsuleCollider capsule) {
                type = ColliderType.Capsule;
                center = capsule.center;
                generic = new Vector3(capsule.radius, capsule.height, capsule.direction);
            }
            else if(col is MeshCollider meshCollider) {
                type = ColliderType.Mesh;
                generic = new Vector3(meshCollider.convex ? 1 : 0, meshCollider.cookingOptions.ToInt(), 0);
                mesh = meshCollider.sharedMesh;
            }
            else if(col is WheelCollider wheel) {
                type = ColliderType.Wheel;
                center = wheel.center;
                if(wheelData == null || wheelData.Length != 17)
                    wheelData = new float[17];
                wheelData[0] = wheel.mass;
                wheelData[1] = wheel.wheelDampingRate;
                wheelData[2] = wheel.suspensionDistance;
                wheelData[3] = wheel.forceAppPointDistance;

                var spring = wheel.suspensionSpring;
                wheelData[4] = spring.spring;
                wheelData[5] = spring.damper;
                wheelData[6] = spring.targetPosition;

                var fFric = wheel.forwardFriction;
                wheelData[7] = fFric.extremumSlip;
                wheelData[8] = fFric.extremumValue;
                wheelData[9] = fFric.asymptoteSlip;
                wheelData[10] = fFric.asymptoteValue;
                wheelData[11] = fFric.stiffness;

                var sFric = wheel.sidewaysFriction;
                wheelData[12] = sFric.extremumSlip;
                wheelData[13] = sFric.extremumValue;
                wheelData[14] = sFric.asymptoteSlip;
                wheelData[15] = sFric.asymptoteValue;
                wheelData[16] = sFric.stiffness;
            }
            else if(col is TerrainCollider terrain) {
                type = ColliderType.Terrain;
                terrainData = terrain.terrainData;
            }

            if(type != ColliderType.Wheel)
                wheelData = null;
        }

        public void Copy(BoxCollider col) {
            type = ColliderType.Box;
            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;
            center = col.center;
            generic = col.size;
        }

        public void Copy(SphereCollider col) {
            type = ColliderType.Sphere;
            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;
            center = col.center;
            generic = new Vector3(col.radius, 0, 0);
        }

        public void Copy(CapsuleCollider col) {
            type = ColliderType.Capsule;
            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;
            center = col.center;
            generic = new Vector3(col.radius, col.height, col.direction);
        }

        public void Copy(MeshCollider col) {
            type = ColliderType.Mesh;
            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;
            mesh = col.sharedMesh;
            generic = new Vector3(col.convex ? 1 : 0, col.cookingOptions.ToInt(), 0);
        }

        public void Copy(WheelCollider col) {
            type = ColliderType.Wheel;
            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;

            center = col.center;
            if(wheelData == null || wheelData.Length != 17)
                wheelData = new float[17];
            wheelData[0] = col.mass;
            wheelData[1] = col.wheelDampingRate;
            wheelData[2] = col.suspensionDistance;
            wheelData[3] = col.forceAppPointDistance;

            var spring = col.suspensionSpring;
            wheelData[4] = spring.spring;
            wheelData[5] = spring.damper;
            wheelData[6] = spring.targetPosition;

            var fFric = col.forwardFriction;
            wheelData[7] = fFric.extremumSlip;
            wheelData[8] = fFric.extremumValue;
            wheelData[9] = fFric.asymptoteSlip;
            wheelData[10] = fFric.asymptoteValue;
            wheelData[11] = fFric.stiffness;

            var sFric = col.sidewaysFriction;
            wheelData[12] = sFric.extremumSlip;
            wheelData[13] = sFric.extremumValue;
            wheelData[14] = sFric.asymptoteSlip;
            wheelData[15] = sFric.asymptoteValue;
            wheelData[16] = sFric.stiffness;
        }

        public void Copy(TerrainCollider col) {
            type = ColliderType.Terrain;
            isTrigger = col.isTrigger;
            contactOffset = col.contactOffset;
            material = col.sharedMaterial;
            terrainData = col.terrainData;
        }

        #endregion

        #region ApplyTo

        public void ApplyTo<T>(T col) where T : Collider {
            if(type == ColliderType.None)
                return;

            col.isTrigger = isTrigger;
            col.contactOffset = contactOffset;
            col.sharedMaterial = material;

            switch(type) {
                case ColliderType.Box:
                    if(col is BoxCollider box) {
                        box.center = center;
                        box.size = generic;
                    }
                    else
                        Debug.LogError("Collider is not a BoxCollider!");
                    break;
                case ColliderType.Sphere:
                    if(col is SphereCollider sphere) {
                        sphere.center = center;
                        sphere.radius = generic.x;
                    }
                    else
                        Debug.LogError("Collider is not a SphereCollider!");
                    break;
                case ColliderType.Capsule:
                    if(col is CapsuleCollider capsule) {
                        capsule.center = center;
                        capsule.radius = generic.x;
                        capsule.height = generic.y;
                        capsule.direction = Mathf.RoundToInt(generic.z);
                    }
                    else
                        Debug.LogError("Collider is not a CapsuleCollider!");
                    break;
                case ColliderType.Mesh:
                    if(col is MeshCollider meshCol) {
                        meshCol.sharedMesh = mesh;
                        meshCol.convex = Convex;
                        meshCol.cookingOptions = CookingOptions;
                    }
                    else
                        Debug.LogError("Collider is not a MeshCollider!");
                    break;
                case ColliderType.Wheel:
                    if(col is WheelCollider wheel) {
                        if(wheelData == null || wheelData.Length != 17) {

                            return;
                        }
                        wheel.mass = wheelData[0];
                        wheel.wheelDampingRate = wheelData[1];
                        wheel.suspensionDistance = wheelData[2];
                        wheel.forceAppPointDistance = wheelData[3];
                        wheel.suspensionSpring = WheelSpring;
                        wheel.forwardFriction = WheelForwardFriction;
                        wheel.sidewaysFriction = WheelSideFriction;
                    }
                    else
                        Debug.LogError("Collider is not a WheelCollider!");
                    break;
                case ColliderType.Terrain:
                    if(col is TerrainCollider terrain) {
                        terrainData = terrain.terrainData;
                    }
                    else
                        Debug.LogError("Collider is not a MeshCollider!");
                    break;
            }
        }

        public void ApplyTo(BoxCollider col) {
            if(type != ColliderType.Box) {
                Debug.LogError("Collider is not a BoxCollider!");
                return;
            }
            col.isTrigger = isTrigger;
            col.contactOffset = contactOffset;
            col.sharedMaterial = material;
            col.center = center;
            col.size = generic;
        }

        public void ApplyTo(SphereCollider col) {
            if(type != ColliderType.Sphere) {
                Debug.LogError("Collider is not a SphereCollider!");
                return;
            }
            col.isTrigger = isTrigger;
            col.contactOffset = contactOffset;
            col.sharedMaterial = material;
            col.radius = generic.x;
        }

        public void ApplyTo(CapsuleCollider col) {
            if(type != ColliderType.Capsule) {
                Debug.LogError("Collider is not a CapsuleCollider!");
                return;
            }
            col.isTrigger = isTrigger;
            col.contactOffset = contactOffset;
            col.sharedMaterial = material;
            col.radius = generic.x;
            col.height = generic.y;
            col.direction = Mathf.RoundToInt(generic.z);
        }

        public void ApplyTo(MeshCollider col) {
            if(type != ColliderType.Mesh) {
                Debug.LogError("Collider is not a MeshCollider!");
                return;
            }
            col.isTrigger = isTrigger;
            col.contactOffset = contactOffset;
            col.sharedMaterial = material;
            col.sharedMesh = mesh;
            col.convex = Convex;
            col.cookingOptions = CookingOptions;
        }

        public void ApplyTo(WheelCollider col) {
            if(type != ColliderType.Wheel) {
                Debug.LogError("Collider is not a WheelCollider!");
                return;
            }
            if(wheelData == null || wheelData.Length != 17) {

                return;
            }
            col.mass = wheelData[0];
            col.wheelDampingRate = wheelData[1];
            col.suspensionDistance = wheelData[2];
            col.forceAppPointDistance = wheelData[3];
            col.suspensionSpring = WheelSpring;
            col.forwardFriction = WheelForwardFriction;
            col.sidewaysFriction = WheelSideFriction;
        }

        public void ApplyTo(TerrainCollider col) {
            if(type != ColliderType.Terrain) {
                Debug.LogError("Collider is not a TerrainCollider!");
                return;
            }
            col.isTrigger = isTrigger;
            col.contactOffset = contactOffset;
            col.sharedMaterial = material;
            col.terrainData = terrainData;
        }

        #endregion

        #region AddCollider

        public Collider AddColliderToGameObject(GameObject go) {
            switch(type) {
                case ColliderType.Box: return (this as IBoxColliderSettings).AddColliderToGameObject(go);
                case ColliderType.Sphere: return (this as ISphereColliderSettings).AddColliderToGameObject(go);
                case ColliderType.Capsule: return (this as ICapsuleColliderSettings).AddColliderToGameObject(go);
                case ColliderType.Mesh: return (this as IMeshColliderSettings).AddColliderToGameObject(go);
                case ColliderType.Wheel: return (this as IWheelColliderSettings).AddColliderToGameObject(go);
                case ColliderType.Terrain: return (this as ITerrainColliderSettings).AddColliderToGameObject(go);
                default: Debug.LogError($"Could not add collider to game object as it is not of a supported type '{type}'"); break;
            }
            return null;
        }

        BoxCollider IBoxColliderSettings.AddColliderToGameObject(GameObject go) {
            if(type != ColliderType.Box)
                return null;
            var col = go.AddComponent<BoxCollider>();
            ApplyTo(col);
            return col;
        }

        SphereCollider ISphereColliderSettings.AddColliderToGameObject(GameObject go) {
            if(type != ColliderType.Sphere)
                return null;
            var col = go.AddComponent<SphereCollider>();
            ApplyTo(col);
            return col;
        }

        CapsuleCollider ICapsuleColliderSettings.AddColliderToGameObject(GameObject go) {
            if(type != ColliderType.Capsule)
                return null;
            var col = go.AddComponent<CapsuleCollider>();
            ApplyTo(col);
            return col;
        }

        MeshCollider IMeshColliderSettings.AddColliderToGameObject(GameObject go) {
            if(type != ColliderType.Mesh)
                return null;
            var col = go.AddComponent<MeshCollider>();
            ApplyTo(col);
            return col;
        }

        WheelCollider IWheelColliderSettings.AddColliderToGameObject(GameObject go) {
            if(type != ColliderType.Wheel)
                return null;
            var col = go.AddComponent<WheelCollider>();
            ApplyTo(col);
            return col;
        }

        TerrainCollider ITerrainColliderSettings.AddColliderToGameObject(GameObject go) {
            if(type != ColliderType.Terrain)
                return null;
            var col = go.AddComponent<TerrainCollider>();
            ApplyTo(col);
            return col;
        }

        #endregion
    }
}
