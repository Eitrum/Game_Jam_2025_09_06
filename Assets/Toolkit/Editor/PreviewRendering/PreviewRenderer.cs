using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public class PreviewRenderer
    {
        #region Structs

        private struct MeshCollection
        {
            public Mesh mesh;
            public Material material;
            public Matrix4x4 offset;

            public MeshCollection(MeshFilter f) {
                mesh = f.sharedMesh;
                material = f.GetComponent<MeshRenderer>().sharedMaterial;
                offset = f.transform.localToWorldMatrix;
            }

            public MeshCollection(Mesh m, Material mat, Matrix4x4 offset) {
                this.mesh = m;
                this.material = mat;
                this.offset = offset;
            }
        }

        #endregion

        #region Variables

        private PreviewRenderUtility preview;
        private GameObject prefab;

        private MeshCollection[] meshCollection;
        private Bounds bounds = new Bounds(new Vector3(), new Vector3(4, 4, 4));
        private Vector2 rotation = new Vector2(0, 0);
        private float zoom = 1f;
        private float zoomSpeed = 1f;
        private bool allowRotation = true;

        #endregion

        #region Properties

        public GameObject Prefab {
            get {
                return prefab;
            }
            set {
                if(prefab != value) {
                    prefab = value;
                    RefreshBounds();
                }
            }
        }

        public Bounds Bounds {
            get => bounds;
            set => bounds = value;
        }

        public float ZoomSpeed {
            get => zoomSpeed;
            set => zoomSpeed = Mathf.Clamp(value, 0f, 10f);
        }

        public bool AllowRotation {
            get => allowRotation;
            set => allowRotation = value;
        }

        #endregion

        #region Constructor

        public PreviewRenderer() {
            preview = new PreviewRenderUtility();

            preview.camera.transform.position = new Vector3(0, 0, -4);
            preview.camera.transform.LookAt(Vector3.zero, Vector3.up);
            preview.camera.farClipPlane = 30;

            preview.lights[0].intensity = 1f;
            preview.lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
            preview.lights[0].color = Color.white;
            preview.lights[1].intensity = 1f;
            preview.lights[1].color = Color.white;
            preview.lights[1].transform.rotation = Quaternion.Euler(30f, 210f, 0f);
            preview.ambientColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);

            Rotate(new Rect(0, 0, 1, 1));
        }

        #endregion

        #region Disposing

        public void Dispose() {
            if(preview != null) {
                preview.Cleanup();
            }
        }

        ~PreviewRenderer() {
            Dispose();
        }

        #endregion

        #region Render

        public void BeginRender(Rect r) {
            if(allowRotation)
                Rotate(r);
            preview.BeginPreview(r, GUIStyle.none);

            if(prefab != null && meshCollection != null) {
                foreach(var col in meshCollection) {
                    preview.DrawMesh(col.mesh, col.offset, col.material, 0);
                }
            }
        }

        public void RenderCustom(Matrix4x4 matrix, Mesh mesh, Material material) {
            preview.DrawMesh(mesh, matrix, material, 0);
        }

        public void EndRender(Rect r) {
            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            preview.camera.Render();
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);

            var t = preview.EndPreview();
            GUI.DrawTexture(r, t);
        }

        #endregion

        #region Utility

        public void RefreshBounds() {
            bounds = default;

            if(prefab == null) {
                return;
            }

            meshCollection = prefab.GetComponentsInChildren<MeshFilter>()
                .Where(x => x.HasComponent<MeshRenderer>())
                .Select(x => new MeshCollection(x))
                .ToArray();

            bounds = GetTotalBounds(prefab.GetComponentsInChildren<MeshFilter>().Select(x => x.sharedMesh));
            Rotate(new Rect(0, 0, 1, 1));
        }

        private void Rotate(Rect r) {
            Drag2D(ref rotation, r, ref zoom, zoomSpeed);

            float halfSize = Mathf.Max(bounds.extents.magnitude, 0.0001f);
            float distance = halfSize * 3.8f * zoom;

            Quaternion rot = Quaternion.Euler(-rotation.y, -rotation.x, 0);
            Vector3 pos = bounds.center - rot * (Vector3.forward * distance);

            preview.camera.transform.position = pos;
            preview.camera.transform.rotation = rot;
            preview.camera.nearClipPlane = distance - halfSize * 1.1f;
            preview.camera.farClipPlane = distance + halfSize * 1.1f;
        }

        private static Bounds GetTotalBounds(IEnumerable<Mesh> instances) {
            float up = 0f, down = 0f, left = 0f, right = 0f, forward = 0f, backward = 0f;
            foreach(var inst in instances) {
                var tB = inst.bounds;
                var ex = tB.extents;
                var ce = tB.center;

                up = Math.Max(up, ex.y + ce.y);
                down = Math.Min(down, -ex.y + ce.y);

                right = Math.Max(right, ex.x + ce.x);
                left = Math.Min(left, -ex.x + ce.x);

                forward = Math.Max(forward, ex.z + ce.z);
                backward = Math.Min(backward, -ex.z + ce.z);
            }

            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        private static void Drag2D(ref Vector2 scrollPosition, Rect position, ref float scroll, float scrollSpeed) {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch(current.GetTypeForControl(controlID)) {
                case EventType.MouseDown:
                    if(position.Contains(current.mousePosition) && position.width > 50f) {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp:
                    if(GUIUtility.hotControl == controlID) {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if(GUIUtility.hotControl == controlID) {
                        scrollPosition -= current.delta * (float)((!current.shift) ? 1f : 3f) / Mathf.Min(position.width, position.height) * 140f;
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
                case EventType.ScrollWheel:
                    if(GUIUtility.hotControl == controlID) {
                        scroll = Mathf.Clamp(scroll + current.delta.y * 0.2f * scrollSpeed, 0.2f, 2f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
        }

        #endregion
    }
}
