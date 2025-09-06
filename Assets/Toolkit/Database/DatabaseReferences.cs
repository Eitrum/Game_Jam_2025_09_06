using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    internal class DatabaseReferences : ScriptableSingleton<DatabaseReferences> {

        #region Singleton

        protected override void OnSingletonCreated() {
            Database.InitalizeDatabase(this);
        }

        #endregion

        #region Variables

        [SerializeField] private SubDatabase[] subDatabases = { };

        #endregion

        #region Properties

        public int DatabaseCount => subDatabases.Length;
        public SubDatabase GetSubDatabase(int index) => subDatabases[index];

        #endregion
    }
}
