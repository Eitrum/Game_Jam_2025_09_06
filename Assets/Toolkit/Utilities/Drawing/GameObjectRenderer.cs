using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering
{
    [AddComponentMenu("Toolkit/Rendering/Game Object Renderer")]
    public class GameObjectRenderer : MonoBehaviour, IEarlyUpdate, IUpdate, ILateUpdate, IPostUpdate, IOnBeforeRender
    {
        #region Variables

        [SerializeField] private GameObject targetGameObject = null;
        [SerializeField] private bool includeChildren = true;
        [SerializeField] private Material overrideMaterial = null;
        [SerializeField] private UpdateModeMask updateMode = UpdateModeMask.PostUpdate;

        private IRendererInstance rendererInstance;

        #endregion

        #region Properties

        public bool IsNull => this == null;
        public GameObject GameObject {
            get => targetGameObject;
            set {
                if(targetGameObject != value) {
                    targetGameObject = value;
                    rendererInstance = RenderUtility.CreateRendererInstance(value, includeChildren);
                }
            }
        }

        #endregion

        #region Unity Methods

        void Awake() {
            rendererInstance = RenderUtility.CreateRendererInstance(targetGameObject, includeChildren);
        }

        private void OnEnable() {
            UpdateSystem.Subscribe(this, updateMode);
        }

        private void OnDisable() {
            UpdateSystem.Unsubscribe(this, updateMode);
        }

        #endregion

        #region Draw

        public void Draw() {
            if(overrideMaterial)
                rendererInstance.Draw(overrideMaterial, transform.localToWorldMatrix);
            else
                rendererInstance.Draw(transform.localToWorldMatrix);
        }

        #endregion

        #region Update Impl

        void IPostUpdate.PostUpdate(float dt) => Draw();
        void IUpdate.Update(float dt) => Draw();
        void ILateUpdate.LateUpdate(float dt) => Draw();
        void IEarlyUpdate.EarlyUpdate(float dt) => Draw();
        void IOnBeforeRender.OnBeforeRender(float dt) => Draw();

        #endregion
    }
}
