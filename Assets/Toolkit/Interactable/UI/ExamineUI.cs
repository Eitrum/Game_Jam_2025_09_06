using System.Collections;
using System.Collections.Generic;
using Toolkit.UI;
using Toolkit.UI.PanelSystem;
using UnityEngine;

namespace Toolkit.Interactables.UI {
    public class ExamineUI : MonoBehaviour, IAssignable<ExaminePreset> {

        #region Variables

        [Header("Text")]
        [SerializeField] private TextField header;
        [SerializeField] private TextField description;
        [SerializeField] private TextField author;

        [Header("Image")]
        [SerializeField] private UnityEngine.UI.Image image;
        [SerializeField] private UnityEngine.UI.RawImage rawImage;

        [Header("Camera")]
        [SerializeField] private bool autoRotate = true;
        [SerializeField, Layer] private int layer = 29;

        [Header("Debug")]
        [SerializeField] private ExaminePreset current;

        private Toolkit.Rendering.GameObjectRendererInstance rendererInstance;
        private Matrix4x4 offset;
        private Panel panel;

        private static GameObject cameraContainer;
        private static Camera cameraRenderer;
        private Vector3 velocity;

        #endregion

        #region Init

        private void Awake() {
            panel = GetComponent<Panel>();
            Assign(current);
        }

        #endregion

        #region Assign

        public void Assign(ExaminePreset preset) {
            if(preset == null)
                return;
            this.current = preset;
            header.Text = preset.ObjectName;
            description.Text = preset.Description;
            author.Text = preset.Author;

            if(cameraRenderer != null)
                cameraRenderer.enabled = false;

            var tex = preset.Texture;
            if(tex != null) {
                rawImage.texture = tex;
                rawImage.SetGameObjectActiveSafe(true);
                image.SetGameObjectActiveSafe(false);
                return;
            }
            var spr = preset.Sprite;
            if(spr != null) {
                image.sprite = spr;
                image.SetGameObjectActiveSafe(true);
                rawImage.SetGameObjectActiveSafe(false);
            }
            rendererInstance = preset.RendererInstance;
            if(rendererInstance == null) {
                image.SetGameObjectActiveSafe(false);
                rawImage.SetGameObjectActiveSafe(false);
                return;
            }
            image.SetGameObjectActiveSafe(false);

            if(cameraContainer == null) {
                var go = Resources.Load<GameObject>("ExamineCamera");
                if(go != null) {
                    cameraContainer = Instantiate(go);
                    DontDestroyOnLoad(cameraContainer);
                    cameraRenderer = cameraContainer.GetComponentInChildren<Camera>();
                    if(cameraRenderer.targetTexture == null) {
                        cameraRenderer.targetTexture = RenderTextureUtility.GetTemporary(800, 800);
                    }
                }
                if(cameraContainer == null) {
                    rawImage.SetGameObjectActiveSafe(false);
                    return;
                }
            }

            cameraRenderer.enabled = true;
            cameraRenderer.cullingMask = 1 << layer;
            rawImage.texture = cameraRenderer.targetTexture;
            var bounds = rendererInstance.Bounds;
            var toOffset = Vector3.zero;// -bounds.center * 0.5f;
            bounds.center = Vector3.zero;
            offset = Matrix4x4.TRS(toOffset, Quaternion.identity, Vector3.one);
            var size = rendererInstance.Bounds.extents.Highest();

            cameraContainer.transform.localRotation = Quaternion.identity;
            cameraRenderer.transform.localPosition = new Vector3(0, 0, -size);
            cameraRenderer.GetPerspectiveFocusTransforms(out var pos, out var rot, bounds, 0.1f);
            cameraRenderer.transform.localPosition = new Vector3(0, 0, -pos.magnitude);
        }

        #endregion

        private void Update() {
            if(rendererInstance == null)
                return;

            if(cameraContainer == null)
                return;

            if(cameraRenderer == null)
                return;

            rendererInstance.Draw(offset, layer, cameraRenderer);

            if(velocity.sqrMagnitude < Mathf.Epsilon && autoRotate) {
                var rot = Quaternion.Euler(0, Time.deltaTime * 15f, 0);
                cameraContainer.transform.rotation *= rot;
            }
            else {
                cameraContainer.transform.rotation *= Quaternion.Euler(new Vector3(-velocity.y, velocity.x, velocity.z) * Time.deltaTime);
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime);
                if(velocity.sqrMagnitude < 0.01f)
                    velocity = Vector2.zero;
            }
        }

        public void Rotate(Vector2 delta) {
            cameraContainer.transform.rotation *= Quaternion.Euler(new Vector3(-delta.y, delta.x));
        }

        public void RotateZ(float delta) {
            cameraContainer.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, delta));
        }

        #region Velocity

        [Button]
        private void AddHorizontalVelocity() {
            AddVelocity(new Vector3(Random.Range(-70, 70), 0));
        }

        [Button]
        private void AddVerticalVelocity() {
            AddVelocity(new Vector3(0, Random.Range(-70, 70)));
        }

        [Button]
        private void AddZRotateVelocity() {
            AddVelocity(new Vector3(0, 0, Random.Range(-70, 70)));
        }

        public void AddVelocity(Vector3 velocity)
            => this.velocity += velocity;

        #endregion
    }
}
