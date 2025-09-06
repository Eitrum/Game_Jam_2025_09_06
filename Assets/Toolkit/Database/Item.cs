using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Toolkit.Editor")]

namespace Toolkit
{
    [CreateAssetMenu(fileName = FILE_NAME, menuName = Database.MENU_PATH + FILE_NAME)]
    public partial class Item : ScriptableObject
    {

        #region Consts

        private const string FILE_NAME = "New Base Item";
        private const string PATH_TOOLTIP = "An editor only path for code generator to write access through containers";

        #endregion

        #region Variables

        [Header("Basic Settings")]
        [SerializeField] private string databaseItemName = "";
#if UNITY_EDITOR
        [SerializeField, Tooltip(PATH_TOOLTIP)] internal string editorPath = "";
        [SerializeField] internal int order = 0;
#endif
        private int id = 0;

        #endregion

        #region Properties

        public string ItemName {
            get {
                if(string.IsNullOrEmpty(databaseItemName)) {
                    databaseItemName = name.EndsWith("(Clone)") ? name.Remove(name.Length - 7, 7) : name;
                }
                return databaseItemName;
            }
            internal set {
                if(value == name) {
                    databaseItemName = "";
                }
                else {
                    databaseItemName = value;
                }
            }
        }
        public int Id {
            get {
#if !UNITY_EDITOR
                if(id == 0)
#endif
                id = Database.GetHashCode(ItemName);
                return id;
            }
        }

        #endregion
    }
}
