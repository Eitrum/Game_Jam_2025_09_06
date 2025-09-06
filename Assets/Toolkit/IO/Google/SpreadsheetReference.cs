using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Network.Google {
    [System.Serializable]
    public class SpreadsheetReference {
        #region Variables

        [SerializeField] private string sheetID;
        [SerializeField] private uint gid = 0;

        private const string CSV_EXPORT_FORMAT = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

        #endregion

        #region Constructor

        public SpreadsheetReference() { }
        public SpreadsheetReference(string sheetId, uint gid) {
            this.sheetID = sheetId;
            this.gid = gid;
        }

        #endregion

        public static SpreadsheetReference ReadFromURL(string url) {
            try {
                // https://docs.google.com/spreadsheets/d/1vSCvUdGM0qxwhW39bFCdFKmgWNT2sVNUyC56XWrrDmc/edit?gid=1686509516#gid=1686509516
                System.Uri uri = new System.Uri(url);
                string[] segments = uri.Segments;
                string spreadsheetId = segments.Length > 3 ? segments[3].Trim('/') : string.Empty;
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string gid = queryParams["gid"];

                return new SpreadsheetReference(spreadsheetId, uint.Parse(gid));
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return new SpreadsheetReference();
            }
        }

        #region Download

        public static string GetCSVDownloadLink(SpreadsheetReference spreadsheet) {
            return GetCSVDownloadLink(spreadsheet.sheetID, spreadsheet.gid);
        }

        public static string GetCSVDownloadLink(string sheetid, uint gid) {
            return string.Format(CSV_EXPORT_FORMAT, sheetid, gid);
        }

        public Promise<List<List<string>>> DownloadCSV() {
            var promise = new Promise<List<List<string>>>(30);
            try {
                if(string.IsNullOrEmpty(sheetID))
                    return promise.ErrorAndReturn("SheetID is null");

                SimpleHttpClient.GetContent(GetCSVDownloadLink(this))
                    .OnError((err) => promise.Error(err.GetError()))
                    .OnCanceled((can) => promise.Cancel())
                    .OnComplete((byte[] data) => {
                        try {
                            var csv = System.Text.Encoding.UTF8.GetString(data);
                            if(string.IsNullOrEmpty(csv)) {
                                promise.Error("Empty sheet");
                                return;
                            }
                            if(!Toolkit.IO.CSVParser.TryParse(csv, out var rows)) {
                                promise.Error("Failed to parse csv");
                                return;
                            }
                            promise.Complete(rows);
                        }
                        catch(System.Exception e) {
                            Debug.LogException(e);
                            promise.Error(e.Message);
                        }
                    });
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                promise.Error(e.Message);
            }
            return promise;
        }

        #endregion
    }
}
