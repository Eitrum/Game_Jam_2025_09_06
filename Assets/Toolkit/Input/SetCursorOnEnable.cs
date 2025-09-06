using UnityEngine;

namespace Toolkit.InputSystem {
    public class SetCursorOnEnable : NullableBehaviour {

        #region Variables

        [SerializeField] private bool setVisibility = false;
        [SerializeField] private bool visibility = true;

        [SerializeField] private bool setLockMode = false;
        [SerializeField] private CursorLockModeSetting lockMode = CursorLockModeSetting.Confined;

        [SerializeField] private bool setTexture = false;
        [SerializeField] private CursorObject texture = null;

        #endregion

        #region Init

        private void OnEnable() {
            if(setVisibility)
                CursorSystem.Add(this, visibility);
            if(setLockMode)
                CursorSystem.Add(this, lockMode);
            if(setTexture)
                CursorSystem.Add(this, texture);
        }

        private void OnDisable() {
            CursorSystem.Remove(this, visibility);
            CursorSystem.Remove(this, lockMode);
            CursorSystem.Remove(this, texture);
        }

        #endregion
    }
}
