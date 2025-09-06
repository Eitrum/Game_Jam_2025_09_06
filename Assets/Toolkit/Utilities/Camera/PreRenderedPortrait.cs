using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Camera/Pre-Rendered Portrait")]
    public class PreRenderedPortrait : MonoBehaviour, IPortrait
    {
        [SerializeField] private Texture2D texture = null;

        public Texture2D Portrait => texture;
        public int Width => texture != null ? texture.width : 0;
        public int Height => texture != null ? texture.height : 0;

        void Awake() {
            if(texture == null)
                Debug.LogError($"{name} is missing a portrait!");
        }
    }
}
