using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.Utility
{
    public static class PrefabCollectionUtility
    {
        #region Variables

        private static PrefabCollection[] collections;
        private static string[] collectionNames;

        #endregion

        #region Properties

        public static IReadOnlyList<PrefabCollection> Collections => collections;
        public static string[] CollectionNames => collectionNames;

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        public static void Refresh() {
            collections = AssetDatabaseUtility.LoadAssets<PrefabCollection>();
            collectionNames = collections.Select(x => x.CollectionName).ToArray();

        }

        #endregion

        #region Methods

        public static IEnumerable<string> GetCategories() => collections.Select(x => x.Category);
        public static IReadOnlyList<PrefabCollection> GetCollections() => collections;
        public static IEnumerable<PrefabCollection> GetCollections(string category) => collections.Where(x => x.Category == category);

        #endregion
    }
}
