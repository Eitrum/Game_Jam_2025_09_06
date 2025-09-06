using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    [CreateAssetMenu(fileName = "Nav Mesh Build Settings", menuName = "Toolkit/AI/Navigation/Build Settings")]
    public class NavMeshBuildSettingsObject : ScriptableObject, INavMeshBuildSettings
    {
        [SerializeField, Min(0.05f)] private float radius = 0.5f;
        [SerializeField, Min(0.001f)] private float height = 2f;
        [SerializeField, Min(0.001f)] private float stepHeight = 0.2f;
        [SerializeField, Range(0f, 60f)] private float maxSlope = 45f;

        private NavMeshBuildSettings settings;

        public NavMeshBuildSettings Settings {
            get {
                if(settings.agentTypeID == 0) {
                    settings = NavMesh.CreateSettings();
                    settings.agentRadius = radius;
                    settings.agentHeight = height;
                    settings.agentSlope = maxSlope;
                    settings.agentClimb = stepHeight;
                    settings.overrideTileSize = true;
                    settings.overrideVoxelSize = true;
                    settings.minRegionArea = 1f;
                    settings.tileSize = NavMeshBuildRules.TILE_SIZE;
                    settings.voxelSize = radius / NavMeshBuildRules.VOXEL_SIZE_PER_AGENT_RADIUS;
                }
                return settings;
            }
        }
    }
}
