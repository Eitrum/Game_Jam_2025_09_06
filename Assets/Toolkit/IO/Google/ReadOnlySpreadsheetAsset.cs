using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Network.Google {
    public class ReadOnlySpreadsheetAsset : ScriptableObject {
        #region Variables

        [SerializeField] private ReadOnlySpreadsheet spreadsheet;

        #endregion

        #region Properties

        public ReadOnlySpreadsheet Spreadsheet => spreadsheet;

        #endregion

        #region Editor

        [Button]
        private void Download() {
            spreadsheet.Download();
        }

        #endregion
    }
}
