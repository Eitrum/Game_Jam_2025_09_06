using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [CreateAssetMenu(fileName = FILE_NAME, menuName = Database.MENU_PATH + FILE_NAME)]
    public class ObjectItem : Item {

        #region Consts

        internal const string FILE_NAME = "New Object Item";

        #endregion

        #region Variables

        [Header("Object Settings")]
        [SerializeField] private UnityEngine.Object reference;

        #endregion

        #region Properties

        public UnityEngine.Object Reference {
            get => reference;
            internal set => reference = value;
        }

        public GameObject AsGameObject => reference as GameObject;
        public Texture AsTexture => reference as Texture;
        public Material AsMaterial => reference as Material;
        public AudioClip AsAudioClip => reference as AudioClip;
        public Mesh AsMesh => reference as Mesh;
        public PhysicsMaterial AsPhysicMaterial => reference as PhysicsMaterial;

        #endregion

        #region Methods

        public T GetAs<T>() where T : UnityEngine.Object => reference as T;
        
        #endregion
    }
}
