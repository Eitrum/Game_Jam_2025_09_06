using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Interactables {
    public class Interactor : MonoBehaviour {

        #region Variables

        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float interactRange = 0.7f;
        [SerializeField, RangeEx(0f, 180f, 0.5f)] private float directional = 180f;
        [SerializeField] private LayerMask interactableLayer = ~0;

        [SerializeField] private Transform referencePoint;
        [SerializeField] private Transform personView;

        [Header("Render")]
        [SerializeField] private bool render;
        [SerializeField] private KeyCode holdToRender = KeyCode.LeftAlt;
        [SerializeField] private KeyCode toggleRendering = KeyCode.Keypad1;
        [SerializeField, Layer] private int layer = 0;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.3f);
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField] private GameObject panelPrefab;

        private IInteractableObject inFocus;
        private List<IInteractableObject> interactables = new List<IInteractableObject>();
        private List<float> distances = new List<float>();
        private List<Matrix4x4> matrices = new List<Matrix4x4>();
        private static Collider[] colliderCache = new Collider[256];
        private Entity entity;
        private UI.InteractableUIPanel followTarget;
        private Toolkit.UI.PanelSystem.HUDModule module;

        #endregion

        private void Awake() {
            entity = GetComponent<Entity>();
        }

        void OnEnable() {
            if(referencePoint == null)
                referencePoint = transform;
            module = Toolkit.UI.PanelSystem.PanelManager.Main.GetHUD().GetOrAdd(panelPrefab);
            followTarget = module.GetComponentInChildren<UI.InteractableUIPanel>(true);
        }

        private void OnDisable() {
            if(module)
                module.Hide();
        }

        private void LateUpdate() {
            var position = referencePoint.position;
            interactables.Clear();
            distances.Clear();
            var hits = Physics.OverlapSphereNonAlloc(position, detectionRadius, colliderCache, interactableLayer, QueryTriggerInteraction.Collide);

            for(int i = 0; i < hits; i++) {
                var interactable = colliderCache[i].GetComponentInParent<IInteractableObject>();
                if(interactable == null)
                    continue;
                if(interactables.Contains(interactable)) // maybe swap to hashset
                    continue;
                if(interactable.IsHidden)
                    continue;
                interactables.Add(interactable);
                distances.Add(Vector3.Distance(colliderCache[i].transform.position, position));
            }

            if(interactables.Count == 0) {
                inFocus = null;
                if(module)
                    module.Hide();
                followTarget.Assign(entity, null);
                return;
            }
            FindInFocus();
            if(module)
                module.Show();
            followTarget.Assign(entity, inFocus);

            if(Input.GetKeyDown(toggleRendering))
                render = !render;

            Render();
        }

        void FindInFocus() {
            IInteractableObject focused = null;

            float currentFocused = detectionRadius;

            for(int i = 0, len = interactables.Count; i < len; i++) {
                var inter = interactables[i];
                var dist = distances[i];

                if(dist > interactRange || dist > currentFocused)
                    continue;

                focused = inter;
                currentFocused = dist;
            }

            if(focused == null) {
                inFocus = null;
                return;
            }

            inFocus = focused;
        }

        void Render() {
            if(!render && !Input.GetKey(holdToRender))
                return;

            matrices.Clear();

            var cam = CameraInstance.MainCameraInstance;
            var forward = cam.transform.forward;
            var rotation = Quaternion.LookRotation(forward);

            for(int i = 0, length = interactables.Count; i < length; i++) {
                if(interactables[i] == inFocus)
                    continue;

                var comp = interactables[i] as Component;
                matrices.Add(Matrix4x4.TRS(comp.transform.position, rotation, scaleCurve.Evaluate(distances[i] / detectionRadius).To_Vector3()));
            }

            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, cam.Camera);
        }
    }
}
