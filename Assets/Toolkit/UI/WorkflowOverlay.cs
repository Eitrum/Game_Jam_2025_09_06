using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI
{
    [ExecuteAlways]
    public class WorkflowOverlay : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool overrideTransparency = false;
        [SerializeField] private float overrideTransparencyValue = 0.5f;

        private ImageComponent component;
        private static List<WorkflowOverlay> components = new List<WorkflowOverlay>();
        private static float transparency = 0.5f;

        #endregion

        #region Properties

        public static float Transparency {
            get => transparency;
            set {
                transparency = Mathf.Clamp01(value);
                for(int i = components.Count - 1; i >= 0; i--) {
                    if(components[i] == null) {
                        components.RemoveAt(i);
                    }
                    else {
                        components[i].UpdateTransparency();
                    }
                }
            }
        }
        public static IReadOnlyList<WorkflowOverlay> Components => components;

        #endregion

        #region Initialize

        private void Awake() {
#if UNITY_EDITOR
            if(Application.isPlaying)
                Destroy(this.gameObject);
#else
            Destroy(this.gameObject);
#endif
        }

        private void OnEnable() {
            components.Add(this);
            UpdateTransparency();
        }

        private void OnDisable() {
            components.Remove(this);
        }

        #endregion

        #region Update

        public void UpdateTransparency() {
            if(!component.HasReference) {
                component = ImageComponent.Find(this);
            }
            if(component.HasReference) {
                component.Transparency = overrideTransparency ? overrideTransparencyValue : transparency;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this.gameObject);
#endif
            }
        }

        public void UpdateTransparency(float value) {
            if(!component.HasReference) {
                component = ImageComponent.Find(this);
            }
            if(component.HasReference) {
                component.Transparency = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this.gameObject);
#endif
            }
        }

        #endregion
    }
}
