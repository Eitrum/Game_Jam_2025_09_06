using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.LoadingScreen {
    public class LoreEntry : ScriptableObject {
        #region Variables

        [SerializeField] private string header;
        [SerializeField] private string content;

        [SerializeField] private Texture2D image;
        [SerializeField] private AudioClip clip;

        #endregion
    }
}
