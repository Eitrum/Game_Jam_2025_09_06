using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public class LoadingOverlay : MonoBehaviour {
        #region Variables

        private const string TAG = "[Toolkit.UI.PanelSystem.LoadingOverlay] - ";
        [SerializeField] private RectTransform spinner;
        private CanvasGroup group;
        private float timeActive;
        private int loadingStack = 0;

        #endregion

        #region Properties

        public bool IsLoading => gameObject.activeSelf;

        #endregion

        private void Awake() {
            group = GetComponent<CanvasGroup>();
        }

        #region Update

        private void LateUpdate() {
            if(spinner)
                spinner.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * -270f);
            timeActive += Time.deltaTime;
            if(group)
                group.alpha = Mathf.Clamp01(group.alpha + (timeActive * Time.deltaTime) * 50f);
        }

        #endregion

        #region Methods

        public void SetOverlayActive(bool active) {
            if(active)
                Show();
            else
                Hide();
        }

        public void SetOverlayActive(bool active, int stacks) {
            if(active)
                Show(stacks);
            else
                Hide(stacks);
        }

        [Button]
        public void Show()
            => Show(1);

        public void Show(int stacks) {
            loadingStack += stacks;
            //Debug.Log(TAG + "Show: " + loadingStack);
            if(gameObject.activeSelf)
                return;
            gameObject.SetActive(true);
            timeActive = 0;
            if(group)
                group.alpha = 0.01f;
        }

        [Button]
        public void Hide()
            => Hide(1);

        public void Hide(int stacks) {
            loadingStack -= stacks;
            Debug.Log(TAG + "Hide: " + loadingStack);
            if(loadingStack > 0)
                return;
            if(loadingStack < 0)
                loadingStack = 0;
            if(!gameObject.activeSelf)
                return;
            gameObject.SetActive(false);
        }

        #endregion
    }
}
