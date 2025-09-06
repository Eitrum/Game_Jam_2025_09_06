using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CreateAssetMenu(fileName = "Physics Settings", menuName = "Toolkit/Physics/Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        #region Enums

        public enum SimulationMode
        {
            FixedUpdate = 0,
            Update = 1,
            Manual = 2,
            Toolkit = 3,
        }

        public enum ContactsGeneration
        {
            LegacyContactsGeneration,
            PersistentContactManifold,
        }

        public enum ContactPairsMode
        {
            DefaultContactPairs,
            EnableKinematicKinematicsPairs,
            EnableKinematicStaticPairs,
            EnableAllContactPairs
        }

        public enum BroadphaseType
        {
            SweepAndPruneBroadphase,
            MultiboxPruningBroadphase,
            AutomaticBoxPruning,
        }

        public enum FrictionType
        {
            PatchFrictionType,
            OneDirectionalFrictionType,
            TwoDirectionalFrictionType
        }

        public enum SolverType
        {
            ProjectedGaussSeidel,
            TemporalGaussSeidel,
        }

        #endregion

        #region Variables

        // Simulation
        [SerializeField] private SimulationMode simulationMode;
        [SerializeField] private float updatesPerSecond = 50;
        [SerializeField] private bool autoSyncTransform = false;
        [SerializeField] private bool reuseCollisionCallbacks = true;
        [SerializeField] private int fixedTimeSmoothing = 0;

        // Base
        [SerializeField] private Vector3 gravity = new Vector3(0, -9.81f, 0f);

        // Thresholds
        [SerializeField, Min(0.00001f)] private float bounceThreshold = 2f;
        [SerializeField, Min(0f)] private float sleepThreshold = 0.005f;

        // Default Values
        [SerializeField] private float defaultMaxDepenetrationVelocity = 10f;
        [SerializeField] private float defaultContactOffset = 0.01f;
        [SerializeField, RangeEx(1, 255)] private int defaultSolverIterations = 6;
        [SerializeField, RangeEx(1, 255)] private int defaultSolverVelocityIterations = 1;
        [SerializeField, Min(0f)] private float defaultMaxAngularSpeed = 7f;

        // Queries
        [SerializeField] private bool queriesHitBackfaces = false;
        [SerializeField] private bool queriesHitTriggers = false;

        // Broadphase
        [SerializeField] private Bounds worldBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(500, 500, 500));
        [SerializeField, Range(1, 16)] private int worldSubdivisions = 8;

        // Cloth
        [SerializeField] private Vector3 clothGravity = new Vector3(0f, -9.81f, 0f);
        [SerializeField] private bool interCollisionEnabled = false;
        [SerializeField, Min(0f)] private float interCollisionDistance = 0f;
        [SerializeField, Min(0f)] private float interCollisionStiffness = 0f;

        // Editor Only
        [SerializeField] private PhysicsMaterial defaultMaterial = null;
        [SerializeField] private bool enableAdaptiveForce = false;
        [SerializeField] private ContactPairsMode contactPairsMode = ContactPairsMode.DefaultContactPairs;
        [SerializeField] private BroadphaseType broadphaseType = BroadphaseType.SweepAndPruneBroadphase;
        [SerializeField] private FrictionType frictionType = FrictionType.PatchFrictionType;
        [SerializeField] private bool improvedPatchFriction = false;
        [SerializeField] private bool enableEnhancedDeterminism = false;
        [SerializeField] private bool enableUnifiedHeightmaps = true;
        [SerializeField] private SolverType solverType = SolverType.ProjectedGaussSeidel;
        [SerializeField] private ContactsGeneration contactsGeneration = ContactsGeneration.PersistentContactManifold;

        #endregion

        #region Properties

        public SimulationMode Mode => simulationMode;
        public float UpdatesPerSecond => updatesPerSecond;
        public bool AutoSyncTransform => autoSyncTransform;
        public bool ReuseCollisionCallbacks => reuseCollisionCallbacks;
        public int FixedTimeSmoothing => fixedTimeSmoothing;

        public Vector3 Gravity => gravity;

        public float BounceThreshold => bounceThreshold;
        public float SleepThreshold => sleepThreshold;

        public float DefaultMaxDepenetrationVelocity => defaultMaxDepenetrationVelocity;
        public float DefaultContactOffset => defaultContactOffset;
        public float DefaultSolverIterations => defaultSolverIterations;
        public float DefaultSolverVelocityIterations => defaultSolverVelocityIterations;
        public float DefaultMaxAngularSpeed => defaultMaxAngularSpeed;

        public bool QueriesHitBackfaces => queriesHitBackfaces;
        public bool QueriesHitTriggers => queriesHitTriggers;

        public Bounds WorldBounds => worldBounds;
        public int WorldSubdivisions => worldSubdivisions;

        public Vector3 ClothGravity => clothGravity;
        public bool InterCollisionEnabled => interCollisionEnabled;
        public float InterCollisionDistance => interCollisionDistance;
        public float InterCollisionStiffness => interCollisionStiffness;

        public PhysicsMaterial DefaultMaterial => defaultMaterial;
        public bool EnableAdaptiveForce => enableAdaptiveForce;
        public ContactPairsMode ContactPairs => contactPairsMode;
        public BroadphaseType Broadphase => broadphaseType;
        public FrictionType Friction => frictionType;
        public bool ImprovedPatchFriction => improvedPatchFriction;
        public bool EnableEnahancedDeterminism => enableEnhancedDeterminism;
        public bool EnableUnifiedHeightmaps => enableUnifiedHeightmaps;
        public SolverType Solver => solverType;
        public ContactsGeneration Contacts => contactsGeneration;

        #endregion

        #region Apply

        [ContextMenu("Apply to Physics Settings")]
        private void ApplyToPhysicsSettings() {
#if UNITY_2023_1_OR_NEWER
            Physics.simulationMode = (UnityEngine.SimulationMode)(int)Mathf.Clamp((int)simulationMode, 0, 2);
#else
            Physics.autoSimulation = simulationMode == SimulationMode.FixedUpdate ? true : false;
#endif
            Time.fixedDeltaTime = updatesPerSecond <= Mathf.Epsilon ? 0.02f : (1f / updatesPerSecond);
            Physics.autoSyncTransforms = autoSyncTransform;
            Physics.reuseCollisionCallbacks = reuseCollisionCallbacks;

            Physics.gravity = gravity;
            Physics.bounceThreshold = bounceThreshold;
            Physics.sleepThreshold = sleepThreshold;

            // Default
            Physics.defaultMaxDepenetrationVelocity = defaultMaxDepenetrationVelocity;
            Physics.defaultContactOffset = defaultContactOffset;
            Physics.defaultSolverIterations = defaultSolverIterations;
            Physics.defaultSolverVelocityIterations = defaultSolverVelocityIterations;
            Physics.defaultMaxAngularSpeed = defaultMaxAngularSpeed;

            // Queries
            Physics.queriesHitBackfaces = queriesHitBackfaces;
            Physics.queriesHitTriggers = queriesHitTriggers;

            // Broadphase
            // if(broadphaseType != BroadphaseType.SweepAndPruneBroadphase)
            //     Physics.RebuildBroadphaseRegions(worldBounds, worldSubdivisions);

            // Cloth
            Physics.clothGravity = clothGravity;
            Physics.interCollisionSettingsToggle = interCollisionEnabled;
            Physics.interCollisionDistance = interCollisionDistance;
            Physics.interCollisionStiffness = interCollisionStiffness;

            Physics.improvedPatchFriction = improvedPatchFriction;

            Editor_ApplyToCurrent();

#if UNITY_EDITOR
            if(!Application.isPlaying) {
                using(UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(PhysicsUpdateSettings.Instance)) {
                    so.FindProperty("physicsSettings").objectReferenceValue = this;
                    if(so.hasModifiedProperties)
                        so.ApplyModifiedProperties();
                }
            }
#endif
            PhysicsUpdateSettings.PhysicsSettings = this;
        }

        #endregion

        #region Copy

        [ContextMenu("Copy from current Physics Settings")]
        private void CopyFromCurrent() {
#if UNITY_2023_1_OR_NEWER
            simulationMode = (SimulationMode)(int)Physics.simulationMode;
#else
            simulationMode = Physics.autoSimulation ? SimulationMode.FixedUpdate : SimulationMode.Manual;
#endif
            updatesPerSecond = Time.fixedDeltaTime <= Mathf.Epsilon ? 50 : (1f / Time.fixedDeltaTime);
            autoSyncTransform = Physics.autoSyncTransforms;
            reuseCollisionCallbacks = Physics.reuseCollisionCallbacks;

            gravity = Physics.gravity;
            bounceThreshold = Physics.bounceThreshold;
            sleepThreshold = Physics.sleepThreshold;

            // Default
            defaultMaxDepenetrationVelocity = Physics.defaultMaxDepenetrationVelocity;
            defaultContactOffset = Physics.defaultContactOffset;
            defaultSolverIterations = Physics.defaultSolverIterations;
            defaultSolverVelocityIterations = Physics.defaultSolverVelocityIterations;
            defaultMaxAngularSpeed = Physics.defaultMaxAngularSpeed;

            // Queries
            queriesHitBackfaces = Physics.queriesHitBackfaces;
            queriesHitTriggers = Physics.queriesHitTriggers;

            // Broadphase
            // worldBounds = new Bounds(new Vector3(), new Vector3(500, 500, 500)); // Get from default file
            // worldSubdivisions = 8; // Get from default file

            // Cloth
            clothGravity = Physics.clothGravity;
            interCollisionEnabled = Physics.interCollisionSettingsToggle;
            interCollisionDistance = Physics.interCollisionDistance;
            interCollisionStiffness = Physics.interCollisionStiffness;

            improvedPatchFriction = Physics.improvedPatchFriction;

            Editor_CopyFromCurrent();
        }

        internal static PhysicsSettings CreateCopyFromCurrent() {
            var ps = CreateInstance<PhysicsSettings>();
            ps.CopyFromCurrent();
            return ps;
        }

        #endregion

        #region Editor Only

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Editor_ApplyToCurrent() {
#if UNITY_EDITOR
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/DynamicsManager.asset");
            using(UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(obj)) {
                // Bounds
                so.FindProperty("m_WorldBounds").boundsValue = worldBounds;
                so.FindProperty("m_WorldSubdivisions").intValue = worldSubdivisions;

                // Editor Only
                so.FindProperty("m_DefaultMaterial").objectReferenceValue = defaultMaterial;
                so.FindProperty("m_ContactsGeneration").intValue = (int)contactsGeneration;
                so.FindProperty("m_ContactPairsMode").intValue = (int)contactPairsMode;
                so.FindProperty("m_FrictionType").intValue = (int)frictionType;
                so.FindProperty("m_EnableEnhancedDeterminism").boolValue = enableEnhancedDeterminism;
                so.FindProperty("m_EnableUnifiedHeightmaps").boolValue = enableUnifiedHeightmaps;
                so.FindProperty("m_EnableAdaptiveForce").boolValue = enableAdaptiveForce;
                so.FindProperty("m_SolverType").intValue = (int)solverType;

                if(so.hasModifiedProperties)
                    so.ApplyModifiedProperties();
            }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Editor_CopyFromCurrent() {
#if UNITY_EDITOR
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/DynamicsManager.asset");
            using(UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(obj)) {
                // Bounds
                worldBounds = so.FindProperty("m_WorldBounds").boundsValue;
                worldSubdivisions = so.FindProperty("m_WorldSubdivisions").intValue;

                // Editor Only
                defaultMaterial = so.FindProperty("m_DefaultMaterial").objectReferenceValue as PhysicsMaterial;
                contactsGeneration = (ContactsGeneration)(so.FindProperty("m_ContactsGeneration")?.intValue ?? 1);
                contactPairsMode = (ContactPairsMode)(so.FindProperty("m_ContactPairsMode")?.intValue ?? 0);
                frictionType = (FrictionType)(so.FindProperty("m_FrictionType")?.intValue ?? 0);
                enableEnhancedDeterminism = so.FindProperty("m_EnableEnhancedDeterminism")?.boolValue ?? false;
                enableUnifiedHeightmaps = so.FindProperty("m_EnableUnifiedHeightmaps")?.boolValue ?? true;
                enableAdaptiveForce = so.FindProperty("m_EnableAdaptiveForce")?.boolValue ?? false;
                solverType = (SolverType)(so.FindProperty("m_SolverType")?.intValue ?? 0);
            }
#endif
        }

        #endregion
    }
}
