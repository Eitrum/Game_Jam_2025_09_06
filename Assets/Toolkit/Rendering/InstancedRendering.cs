using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering {
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Rendering/Instanced Rendering")]
    public class InstancedRendering : BaseComponent {
        #region Variables

        private const string TAG = "<color=yellow>[Instanced Rendering]</color> - ";

        [SerializeField, Readonly(true)] private UpdateModeMask updateMode = UpdateModeMask.PostUpdate;

        [SerializeField, Layer] private int layer = 0;
        [SerializeField] private bool receiveShadows = true;
        [SerializeField] private UnityEngine.Rendering.ShadowCastingMode shadowCasting = UnityEngine.Rendering.ShadowCastingMode.On;

        [SerializeField] private InstancedRenderingData[] data = { };
        [SerializeField, Range(0f, 1f)] private float defaultDrawAmount = 1f;

        private float drawAmount = 0f;

        private static System.Threading.Thread optimizationThread;
        private static List<(InstancedRendering, Vector4)> optimizationQueue = new List<(InstancedRendering, Vector4)>();
        public static int OptimizationQueueCount => optimizationQueue.Count;

        #endregion

        #region Properties

        public int Layer => layer;
        public bool ReceiveShadows { get => receiveShadows; set => receiveShadows = value; }
        public UnityEngine.Rendering.ShadowCastingMode ShadowCasting { get => shadowCasting; set => shadowCasting = value; }
        public IReadOnlyList<InstancedRenderingData> Data => data;
        public float DrawAmount {
            get => drawAmount;
            set {
                drawAmount = Mathf.Clamp(value, 0f, 1f);
                for(int i = 0, length = data.Length; i < length; i++) {
                    data[i].DrawPercentage = drawAmount;
                }
            }
        }

        #endregion

        #region Awake

        private void Awake() {
            drawAmount = defaultDrawAmount;
#if UNITY_EDITOR
            if(!Application.isPlaying) {
                return;
            }
#endif
            for(int i = 0, length = data.Length; i < length; i++) {
                if(defaultDrawAmount < 1f - Mathf.Epsilon)
                    data[i].DrawPercentage = defaultDrawAmount;
            }
        }

        #endregion

        #region Enable Disable

        private void OnEnable() {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
#endif
            Subscribe(updateMode);
        }

        private void OnDisable() {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
#endif
            Unsubscribe(updateMode);
        }

        #endregion

        #region Update

        protected override void EarlyUpdateComponent(float dt) => Render();
        protected override void LateUpdateComponent(float dt) => Render();
        protected override void PostUpdateComponent(float dt) => Render();
        protected override void UpdateComponent(float dt) => Render();
        protected override void OnBeforeRenderComponent(float dt) => Render();

        protected override void OnGUIComponent(float dt) => Debug.LogError("Unable to render the object in the OnGUI step");
        protected override void FixedUpdateComponent(float dt) => Debug.LogError("Unable to render the object in the Fixed Update step");

#if UNITY_EDITOR
        private void LateUpdate() {
            if(!Application.isPlaying) {
                Render();
            }
        }
#endif

        #endregion

        #region Optimization

        public void OptimizeForPosition(Vector3 position, float renderDistance = 50f) {
            lock(optimizationQueue) {
                bool found = false;

                if(!found)
                    optimizationQueue.Add((this, position.To_Vector4(renderDistance)));
            }
            if(optimizationThread == null || !optimizationThread.IsAlive) {
                optimizationThread = new System.Threading.Thread(OptimizeStart);
                optimizationThread.Start();
            }
        }

        private static void OptimizeStart() {
            //Debug.Log(TAG + $"Starting new optimization thread!");
            System.Threading.Thread.Sleep(1);

            while(optimizationQueue.Count > 0) {
                InstancedRendering rend;
                Vector4 posDist;
                lock(optimizationQueue) {
                    var t = optimizationQueue[0];
                    optimizationQueue.RemoveAt(0);
                    rend = t.Item1;
                    posDist = t.Item2;
                }

                if(rend == null)
                    continue;


                var position = (Vector3)posDist;
                var distance = posDist.w;
                //Debug.Log(TAG + $"Starting optimization @ ({position}) [{distance}]!");

                var data = rend.data;
                for(int i = 0, length = data.Length; i < length; i++) {
                    //Debug.Log(TAG + "Optimizing: " + i);
                    data[i].Optimize(position, distance);
                }
            }
            //Debug.Log(TAG + $"Optimization thread complete!");
        }

        public void AddToCommandBuffer(CommandBuffer buffer) {
            foreach(var d in data)
                buffer.DrawMeshInstanced(d.Mesh, 0, d.Material, -1, d.Raw);
        }

        #endregion

        #region Rendering

        public void Render() {
            foreach(var con in data)
                con.Render(shadowCasting, receiveShadows, layer);
        }

        #endregion

        #region Clearing

        public void RemoveAt(Vector3 point, float radius) {
            foreach(var c in data)
                c.RemoveAt(point, radius);
        }

        #endregion

        #region Bake Fast

        [ContextMenu("Bake Fast")]
        public void BakeChildren() { // IMplement into RenderingBakeUtility
            var mfs = GetComponentsInChildren<MeshFilter>();
            List<(Mesh, Material[], List<Matrix4x4>)> objects = new List<(Mesh, Material[], List<Matrix4x4>)>();
            List<Material> materialCache = new List<Material>();
            for(int i = 0, length = mfs.Length; i < length; i++) {
                var t = mfs[i];
                var lod = t.GetComponentInParent<LODGroup>();
                if(lod != null) {
                    if(t.transform.GetSiblingIndex() != 0)
                        continue;
                }
                var r = t.GetComponent<MeshRenderer>();
                if(t && r) {
                    r.GetSharedMaterials(materialCache);
                    var obj = objects.FirstOrDefault(x => x.Item1 == t.sharedMesh && x.Item2.EqualElements(materialCache));
                    if(obj.Item1 != null)
                        obj.Item3.Add(t.transform.localToWorldMatrix);
                    else
                        objects.Add((t.sharedMesh, r.sharedMaterials, new List<Matrix4x4>() { t.transform.localToWorldMatrix }));
                }
            }
            for(int i = objects.Count - 1; i >= 0; i--) {
                var o = objects[i];
                if(o.Item3.Count > 1023) {
                    var splits = o.Item3.SplitAt(1023);
                    objects.RemoveAt(i);
                    for(int x = 0, sLength = splits.Count; x < sLength; x++) {
                        var s = splits[x];
                        objects.Insert(i + 1, (o.Item1, o.Item2, s));
                    }
                }
            }

            data = objects.Select(x => new InstancedRenderingData(x.Item1, x.Item2, x.Item3)).ToArray();
        }

        [ContextMenu("Disable Renderers")]
        private void DisableChildRenderers() {
            var mfs = GetComponentsInChildren<MeshFilter>();
            foreach(var m in mfs) {
                var r = m.GetComponent<MeshRenderer>();
                if(r) {
                    r.enabled = false;
                }
            }
        }

        [ContextMenu("Delete Children")]
        public void DeleteAllChildren() {
            transform.DestroyAllChildrenImmediate();
        }

        #endregion
    }
}
