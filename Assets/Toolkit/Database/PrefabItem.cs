using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    // Create Asset Menu replaced with PrefabItemEditor.cs
    //[CreateAssetMenu(fileName = FILE_NAME, menuName = Database.MENU_PATH + FILE_NAME)]
    public partial class PrefabItem : Item {

        #region Consts

        internal const string FILE_NAME = "New Prefab Item";

        #endregion

        #region Variables

        [Header("Prefab Settings")]
        [SerializeField] internal GameObject reference = null;

        #endregion

        #region Properties

        public GameObject Reference {
            get => reference;
            internal set => reference = value;
        }

        #endregion
    }
}
