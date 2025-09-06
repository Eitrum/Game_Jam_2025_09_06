using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Toolkit;

namespace Toolkit.AI.Navigation
{
    [AddComponentMenu("Toolkit/AI/Navigation/Generator")]
    public class NavMeshGenerator : MonoBehaviour, INavMeshGenerator
    {
        #region Variables

        [SerializeField] NavMeshBuildRuleMask mask = null;
        [SerializeField] private Bounds bounds = new Bounds(Vector3.zero, new Vector3(256f, 256f, 256f));
        [SerializeField] private Pose offset = new Pose(Vector3.zero, Quaternion.identity);

        [SerializeField] private bool autoUpdateOnChange = true;

        private bool dirty = false;
        private List<INavMeshSource> sources = new List<INavMeshSource>();
        private List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
        private NavMeshDataInstance navMeshInstance;
        private NavMeshData data;

        #endregion

        #region Properties

        public NavMeshBuildRuleMask Mask => mask;
        public Bounds Bounds {
            get => bounds;
            set {
                bounds = value;
                if(autoUpdateOnChange)
                    UpdateAllNavMeshes();
            }
        }

        #endregion

        #region Start / Destroy

        void Start() {
            if(data == null) {
                data = NavMeshBuilder.BuildNavMeshData(UnityEngine.AI.NavMesh.GetSettingsByID(0), buildSources, bounds, transform.position, transform.rotation);
                navMeshInstance = UnityEngine.AI.NavMesh.AddNavMeshData(data);
            }
            UpdateAllNavMeshes();
        }

        void OnDestroy() {
            if(navMeshInstance.valid)
                UnityEngine.AI.NavMesh.RemoveNavMeshData(navMeshInstance);
        }

        #endregion

        #region Generate / Update

        public void UpdateAllNavMeshes() {
            if(!dirty) {
                dirty = true;
                Timer.NextFrame(UpdateAll);
            }
        }

        [ContextMenu("Update Nav Mesh")]
        private void UpdateAll() {
            Start();
            if(enabled) {
                buildSources.Clear();
                buildSources.AddRange(sources.Select(x => x.Source));
                NavMeshBuilder.UpdateNavMeshData(data, UnityEngine.AI.NavMesh.GetSettingsByID(0), buildSources, bounds);
            }
            dirty = false;
        }

        #endregion

        #region Add / Remove

        public void Add(INavMeshSource source) {
            sources.Add(source);
            if(autoUpdateOnChange)
                UpdateAllNavMeshes();
        }

        public void Remove(INavMeshSource source) {
            sources.Remove(source);
            if(autoUpdateOnChange)
                UpdateAllNavMeshes();
        }

        #endregion

        #region Editor

        void OnDrawGizmosSelected() {
            GizmosUtility.DrawWireCube(transform.position + offset.position + (transform.rotation * offset.rotation * bounds.center), transform.rotation * offset.rotation, bounds.extents);
        }

        #endregion
    }
}
