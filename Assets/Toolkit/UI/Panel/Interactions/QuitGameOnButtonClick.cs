using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Components {
    public class QuitGameOnButtonClick : MonoBehaviour {
        private UnityEngine.UI.Button button;

        private void Awake() {
            button = GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit(0);
#endif
        }
    }
}
