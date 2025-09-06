using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Interactables {
    [CreateAssetMenu(menuName = "Toolkit/Interactable/Examine Preset")]
    public class ExaminePreset : ScriptableObject {

        #region Variables

        [SerializeField] private string examineCustomName;
        [SerializeField] private string objectName;
        [SerializeField, TextArea(2, 8)] private string description;
        [SerializeField] private string author;

        [SerializeField] private Texture2D texture;
        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject prefabToRender;

        private Toolkit.Rendering.GameObjectRendererInstance rendererInstance;

        #endregion

        #region Properties

        public string ExamineCustomName => string.IsNullOrEmpty(examineCustomName) ? "Examine" : examineCustomName;
        public string ObjectName => objectName;
        public string Description => description;
        public string Author => author;

        public Texture2D Texture => texture;
        public Sprite Sprite => sprite;
        public Toolkit.Rendering.GameObjectRendererInstance RendererInstance {
            get {
                if(prefabToRender == null)
                    return null;
                if(rendererInstance == null || rendererInstance.Instances.Count == 0) {
                    rendererInstance = new Toolkit.Rendering.GameObjectRendererInstance(prefabToRender, true);
                }
                return rendererInstance;
            }
        }

        #endregion
    }
}
