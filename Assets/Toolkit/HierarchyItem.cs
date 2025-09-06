using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit {
    public class HierarchyItem : MonoBehaviour {
        #region Variables

        [SerializeField] private int backgroundMode = 0;
        [SerializeField] private Color color = Color.clear;
        [SerializeField] private Texture2D icon = null;
        [SerializeField] private Color fontColor = Color.clear;
        [SerializeField] private string customText = null;
        private System.Action<Rect> onDraw = null;
        private float onDrawReservedWidth = 0f;

        #endregion

        #region Properties

        public int BackgroundMode {
            get => backgroundMode; set {
                backgroundMode = value;
                EditorUpdate();
            }
        }

        public Color Color {
            get => color;
            set {
                color = value;
                EditorUpdate();
            }
        }
        public Texture2D Icon {
            get => icon;
            set {
                icon = value;
                EditorUpdate();
            }
        }

        public Color CustomFontColor {
            get => fontColor;
            set {
                fontColor = value;
                EditorUpdate();
            }
        }

        public string CustomText {
            get => customText;
            set {
                customText = value;
                EditorUpdate();
            }
        }
        public bool IsEmpty => color == Color.clear && fontColor == Color.clear && icon == null && string.IsNullOrEmpty(customText) && onDraw == null;

        public void Clear() {
            color = Color.clear;
            icon = null;
            fontColor = Color.clear;
            customText = null;
            EditorUpdate();
        }

        #endregion

        #region Custom Information

        public void AddCustomDrawer(float width, System.Action<Rect> onDraw) {
            this.onDraw = onDraw;
            onDrawReservedWidth = width;
            EditorUpdate();
        }

        public void ClearCustomDrawer() {
            this.onDraw = null;
            onDrawReservedWidth = 0f;
            EditorUpdate();
        }

        internal bool TryGetCustomDrawer(out float width, out System.Action<Rect> onDraw) {
            width = onDrawReservedWidth;
            onDraw = this.onDraw;
            return onDraw != null;
        }

        #endregion

        #region Hide / Show

        [ContextMenu("Hide")]
        public void Hide() {
            this.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        }

        public void Show() {
            this.hideFlags = HideFlags.None;
        }

        #endregion

        #region Static

        public static void SetColor(GameObject go, Color color) {
            AddTo(go).Color = color;
        }

        public static void SetTexture(GameObject go, Texture2D texture) {
            AddTo(go).Icon = texture;
        }

        public static void SetName(GameObject go, string text) {
            AddTo(go).CustomText = text;
        }

        public static void AddCustomDrawer(GameObject go, float width, System.Action<Rect> onDraw) {
            if(!go)
                return;
            AddTo(go).AddCustomDrawer(width, onDraw);
        }

        public static void ClearCustomDrawer(GameObject go) {
            if(!go)
                return;
            var hitem = go.GetComponent<HierarchyItem>();
            if(hitem)
                hitem.ClearCustomDrawer();
        }

        public static HierarchyItem AddTo(GameObject go) {
            if(!go)
                return null;
            var comp = go.GetComponent<HierarchyItem>();
            if(comp == null) {
                comp = go.AddComponent<HierarchyItem>();
                comp.Hide();
            }
            return comp;
        }

        public static void RemoveIfEmpty(GameObject go) {
            if(!go)
                return;
            var comp = go.GetComponent<HierarchyItem>();
            if(!comp)
                return;
            if(!comp.IsEmpty)
                return;
            if(Application.isPlaying)
                Destroy(comp);
            else
                DestroyImmediate(comp);
        }

        #endregion

        #region Built-In Formats

        public enum ProgressState {
            None,
            InProgress,
            Paused,
            Failure,
            Success,
        }

        public static void SetProgress(GameObject go, ProgressState state) {
            switch(state) {
                case ProgressState.None:
                    SetTexture(go, null);
                    SetName(go, null);
                    break;
                case ProgressState.InProgress: {
                        SetTexture(go, Resources.Load<Texture2D>("hierarchy_progress"));
                        SetName(go, $"{go.name} <color=#00FFFF>(In Progress)</color>");
                    }
                    break;
                case ProgressState.Paused: {
                        SetTexture(go, Resources.Load<Texture2D>("hierarchy_paused"));
                        SetName(go, $"{go.name} <color=#FAFAD2>(Paused)</color>");
                    }
                    break;
                case ProgressState.Failure: {
                        SetTexture(go, Resources.Load<Texture2D>("hierarchy_failed"));
                        SetName(go, $"{go.name} <color=#CD5C5C>(Failed)</color>");
                    }
                    break;
                case ProgressState.Success: {
                        SetTexture(go, Resources.Load<Texture2D>("hierarchy_success"));
                        SetName(go, $"{go.name} <color=#7CFC00>(Success)</color>");
                    }
                    break;
            }
        }

        public static void SetProgress(GameObject go, float percentage) {
            if(percentage < 1f) {
                SetTexture(go, Resources.Load<Texture2D>("hierarchy_progress"));
                SetName(go, $"{go.name} <color=#00FFFF>({percentage:P0})</color>");
            }
            else {
                SetTexture(go, Resources.Load<Texture2D>("hierarchy_success"));
                SetName(go, $"{go.name} <color=#7CFC00>(Success)</color>");
            }
        }

        #endregion

        #region Editor

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void EditorUpdate() {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            EditorApplication.RepaintHierarchyWindow();
#endif
        }

        #endregion
    }
}
