using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Network.Google {
    [System.Serializable]
    public class ReadOnlySpreadsheet {

        [System.Serializable]
        public class Row {
            [SerializeField] private List<string> cells = new List<string>();

            public IReadOnlyList<string> Cells => cells;

            public Row() { }
            public Row(List<string> cells) { this.cells = cells; }
        }

        #region Variables

        [SerializeField] private SpreadsheetReference spreadsheetReference = new SpreadsheetReference();
        [SerializeField] private List<Row> rows = new List<Row>();

        #endregion

        #region Properties

        public int Height => rows.Count;
        public int Width => rows.Count > 0 ? rows[0].Cells.Count : 0;

        #endregion

        #region Constructor

        public ReadOnlySpreadsheet() { }
        public ReadOnlySpreadsheet(SpreadsheetReference spreadsheetReference) { this.spreadsheetReference = spreadsheetReference; }
        public ReadOnlySpreadsheet(List<List<string>> rows) {
            foreach(var r in rows)
                this.rows.Add(new Row(r));
        }

        #endregion

        #region Download Update

        public void Download() {
            spreadsheetReference
                .DownloadCSV()
                .SetOnComplete(OnDownloadComplete)
                .SetOnError(OnDownloadError);
        }

        private void OnDownloadComplete(List<List<string>> value) {
            rows.Clear();
            foreach(var v in value)
                rows.Add(new Row(v));
        }

        private void OnDownloadError(string error) {
            Debug.Log(error);
        }

        #endregion

        #region Row

        public Row GetRow(int index) {
            return rows[index];
        }

        public Row[] GetRows(int index, int count) {
            Row[] rows = new Row[count];
            for(int i = 0; i < count; i++)
                rows[i] = this.rows[i + index];
            return rows;
        }

        #endregion

        public string GetCell(int x, int y) {
            return rows[y].Cells[x];
        }
    }
}
