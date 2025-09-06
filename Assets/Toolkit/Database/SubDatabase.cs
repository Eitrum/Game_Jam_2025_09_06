using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolkit {
    [CreateAssetMenu(fileName = FILE_NAME, menuName = Database.MENU_PATH + FILE_NAME)]
    public partial class SubDatabase : ScriptableObject
    {

        #region Consts

        private const string FILE_NAME = "New Database";

        #endregion

        #region Variables

        [Tooltip("Use '.' (dots) to indicate namespace during code generation")]
        [SerializeField] private string databaseName = "";
        [SerializeField] private Item[] items = { };

        #endregion

        #region Properties

        public string DatabaseName => string.IsNullOrEmpty(databaseName) ? name : databaseName;
        internal string Name => (string.IsNullOrEmpty(databaseName) ? name : databaseName).Split('.').Last();
        internal string Namespace {
            get {
                var split = DatabaseName.Split('.');
                return string.Join(".", split.Take(split.Length - 1));
            }
        }
        public int Id => Database.GetHashCode(DatabaseName);

        public int Length => items.Length;
        public Item this[int index] => items[index];
        public Item[] All => items;

        #endregion

        #region Filters

        public PrefabItem[] AllPrefabs => AllOf<PrefabItem>();

        public T[] AllOf<T>() where T : Item => items.Where(x => x is T).Select(x => x as T).ToArray();

        #endregion

        #region Verification

        public bool Verify(out string error) {
            foreach(var i in items) {
                if(i == null) {
                    error = "contains null items";
                    return false;
                }
            }
            error = null;
            return true;
        }

        #endregion
    }
}
