using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Camera/Simple Portrait")]
    public class SimplePortrait : MonoBehaviour, IPortrait
    {
        #region Variables

        [Header("Portrait Settings")]
        [SerializeField] private int width = 128;
        [SerializeField] private int height = 128;
        [SerializeField] private bool transparent = true;
        [SerializeField] private Color backgroundColor = Color.white;
        [Header("Rendering Settings")]
        [SerializeField] private Transform cameraLocation = null;
        [SerializeField] private Mode renderMode = Mode.NextFrame;
        [SerializeField] private float delayModeTime = 1f;
        [SerializeField] private LayerMask mask = ~0;
        [SerializeField, Range(0.001f, 179.9999f)] private float fov = 60f;
        [SerializeField, Tooltip("Moves the object to random location in the world to ensure no overlap")] private bool randomLocation = false;
        [Header("Object Settings")]
        [SerializeField] private GameObject[] objectsToRender = null;
        [SerializeField] private bool includeChildObjects = true;
        [SerializeField] private bool recalculateSkinnedMeshRenderer = true;

        private Texture2D texture = null;
        private List<int> objectsMask = new List<int>();
        private List<SkinnedMeshRenderer> toDisable = new List<SkinnedMeshRenderer>();
        private static List<SkinnedMeshRenderer> toAddSkinnedMeshes = new List<SkinnedMeshRenderer>();

        public delegate void OnBeforeRenderCallback();
        public delegate void OnAfterRenderCallback();

        public event OnBeforeRenderCallback OnBeforeRender;
        public event OnAfterRenderCallback OnAfterRender;

        #endregion

        #region Properties

        public Texture2D Portrait => texture;
        public int Width => width;
        public int Height => height;

        public IReadOnlyList<GameObject> Objects => objectsToRender;

        #endregion

        #region Unity Methods

        private void Awake() {
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;

            if(!cameraLocation)
                cameraLocation = transform;

            switch(renderMode) {
                case Mode.Awake:
                    Render();
                    break;
            }
        }

        void Start() {
            switch(renderMode) {
                case Mode.NextFrame:
                    Timer.NextFrame(Render);
                    break;
                case Mode.Start:
                    Render();
                    break;
                case Mode.Delay:
                    Timer.Once(delayModeTime, Render);
                    break;
            }
        }

        private void OnDestroy() {
            Destroy(texture);
        }

        #endregion

        #region Render

        [ContextMenu("Render")]
        public void Render() {
            // Setup Masks
            if(objectsMask == null) {
                objectsMask = new List<int>();
            }
            objectsMask.Clear();
            for(int i = 0, length = objectsToRender.Length; i < length; i++) {
                if(includeChildObjects) {
                    GetLayerRecursive(objectsToRender[i], objectsMask);
                    ApplyLayerRecursive(objectsToRender[i], mask.value.GetFlagIndex());
                }
                else {
                    objectsMask.Add(objectsToRender[i].layer);
                    objectsToRender[i].layer = mask.value.GetFlagIndex();
                }
            }
            // Setup Skinned mesh
            if(recalculateSkinnedMeshRenderer) {
                for(int i = 0, length = objectsToRender.Length; i < length; i++) {
                    if(includeChildObjects) {
                        objectsToRender[i].GetComponentsInChildren<SkinnedMeshRenderer>(toAddSkinnedMeshes);
                        toDisable.AddRange(toAddSkinnedMeshes);
                    }
                    else {
                        var smr = objectsToRender[i].GetComponent<SkinnedMeshRenderer>();
                        toDisable.Add(smr);
                    }
                }
                for(int i = 0, length = toDisable.Count; i < length; i++) {
                    toDisable[i].forceMatrixRecalculationPerRender = true;
                }
            }
            // Setup Location
            var pos = transform.localPosition;
            if(randomLocation) {
                transform.localPosition = new Vector3(Random.Range(-5000, 5000), Random.Range(-5000, 5000), Random.Range(-5000, 5000));
            }

            // Before Render
            OnBeforeRender?.Invoke();

            // Render
            SimplePortraitCamera.Render(texture, cameraLocation, fov, mask, transparent, backgroundColor);

            // After Render
            OnAfterRender?.Invoke();

            // Revert location
            if(randomLocation) {
                transform.localPosition = pos;
            }

            // Revert Mask
            int index = 0;
            for(int i = 0, length = objectsToRender.Length; i < length; i++) {
                if(includeChildObjects)
                    ApplyLayerRecursive(objectsToRender[i], objectsMask, ref index);
                else
                    objectsToRender[i].layer = objectsMask[i];
            }

            // Revert Skinned Mesh
            if(recalculateSkinnedMeshRenderer) {
                for(int i = 0, length = toDisable.Count; i < length; i++) {
                    toDisable[i].forceMatrixRecalculationPerRender = false;
                }
                toDisable.Clear();
            }
        }

        private static void GetLayerRecursive(GameObject obj, List<int> layers) {
            layers.Add(obj.layer);
            for(int i = 0, length = obj.transform.childCount; i < length; i++) {
                var child = obj.transform.GetChild(i);
                GetLayerRecursive(child.gameObject, layers);
            }
        }

        private static void ApplyLayerRecursive(GameObject obj, int layer) {
            obj.layer = layer;
            for(int i = 0, length = obj.transform.childCount; i < length; i++) {
                var child = obj.transform.GetChild(i);
                ApplyLayerRecursive(child.gameObject, layer);
            }
        }

        private static void ApplyLayerRecursive(GameObject obj, List<int> layers, ref int index) {
            obj.layer = layers[index];
            index = index + 1;
            for(int i = 0, length = obj.transform.childCount; i < length; i++) {
                var child = obj.transform.GetChild(i);
                ApplyLayerRecursive(child.gameObject, layers, ref index);
            }
        }

        #endregion

        #region Editor

        private void OnDrawGizmosSelected() {
            var mtx = Gizmos.matrix;
            if(cameraLocation) {
                Gizmos.matrix = cameraLocation.localToWorldMatrix;
                Gizmos.DrawFrustum(Vector3.zero, fov, 8f, 0.3f, width / (float)height);
            }
            else {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawFrustum(Vector3.zero, fov, 8f, 0.3f, width / (float)height);

            }
            Gizmos.matrix = mtx;
        }

        #endregion

        #region Operator

        public static implicit operator Texture2D(SimplePortrait sp) => sp.Portrait;

        #endregion

        public enum Mode
        {
            Awake,
            Start,
            NextFrame,
            Manual,
            Delay,
        }
    }
}
