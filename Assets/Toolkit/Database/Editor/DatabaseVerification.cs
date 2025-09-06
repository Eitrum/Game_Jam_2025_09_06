using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    internal static class DatabaseVerification {

        public static Item[] VerifyDuplicatedAssets(SubDatabase database) {
            var allItems = database.All;
            Dictionary<int, Item> lookup = new Dictionary<int, Item>();
            List<Item> duplication = new List<Item>();
            for(int i = 0, length = allItems.Length; i < length; i++) {
                var item = allItems[i];
                if(lookup.ContainsKey(item.Id)) {
                    duplication.Add(lookup[item.Id]);
                    duplication.Add(item);
                }
                else {
                    lookup.Add(item.Id, item);
                }
            }

            return duplication.ToArray();
        }

        public static Item[] VerifyDuplicatedAssets() {
            Database.ReloadDatabase();
            var allItems = Database.All;
            Dictionary<int, Item> lookup = new Dictionary<int, Item>();
            List<Item> duplication = new List<Item>();
            for(int i = 0, length = allItems.Length; i < length; i++) {
                var item = allItems[i];
                if(lookup.ContainsKey(item.Id)) {
                    duplication.Add(lookup[item.Id]);
                    duplication.Add(item);
                }
                else {
                    lookup.Add(item.Id, item);
                }
            }

            return duplication.ToArray();
        }

        public static void LogDuplicatedAssets(Item[] items) {
            if(items.Length % 2 != 0) {
                Debug.LogError("Duplicated Assets should always come in pairs");
                return;
            }
            for(int i = 0, length = items.Length; i < length; i += 2) {
                var firstItem = items[i];
                var secondItem = items[i + 1];
                var firstItemPath = UnityEditor.AssetDatabase.GetAssetPath(firstItem);
                var secondItemPath = UnityEditor.AssetDatabase.GetAssetPath(secondItem);

                Debug.LogError($"{firstItem.ItemName} conflicts at {firstItemPath} and {secondItemPath}");
            }
        }
    }
}
