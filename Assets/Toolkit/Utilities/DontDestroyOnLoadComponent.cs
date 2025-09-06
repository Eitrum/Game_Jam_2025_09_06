using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility {
    [AddComponentMenu("Toolkit/Utility/DontDestroyOnLoad")]
    public class DontDestroyOnLoadComponent : MonoBehaviour {
        public enum Mode {
            Awake,
            Start,
            OnEnable,
        }

        #region Variables

        [SerializeField] private Mode mode = Mode.Awake;

        #endregion

        #region Methods

        private void Awake() {
            if(mode == Mode.Awake)
                DontDestroyOnLoad(this.gameObject);
        }

        private void Start() {
            if(mode == Mode.Start)
                DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable() {
            if(mode == Mode.OnEnable)
                DontDestroyOnLoad(this.gameObject);
        }

        #endregion
    }
}
