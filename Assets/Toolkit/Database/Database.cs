using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Toolkit
{
    public static class Database
    {

        #region Consts

        internal const string MENU_PATH = "Toolkit/Database/";
        private const string FILE_NAME = "New Database";
        private const string TAG = "Database";

        public static class Types
        {
            public static readonly System.Type DATABASE_REFERENCES = typeof(DatabaseReferences);
            public static readonly System.Type SUB_DATABASE = typeof(SubDatabase);
        }

        #endregion

        #region Utility

        internal static int GetHashCode(string str) {
            unchecked {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        #endregion

        #region Initalization
#if UNITY_EDITOR
        /// Used to create and store a reference to all databases after recompile
        /// If changing or updating any assets, call the "ReloadDatabase" method to ensure everything is setup.
        //[UnityEditor.InitializeOnLoadMethod]
#endif
        //[RuntimeInitializeOnLoadMethod]
        internal static void InitalizeDatabase(DatabaseReferences instance) {
            databaseLookup.Clear();
            itemLookup.Clear();
            if(instance == null) {
                Debug.Log("<color=yellow>[Database]</color> - Database reference object does not exist. If first time loading, the codebase this will always happen");
                return;
            }
            List<Item> uniqueItems = new List<Item>();
            databases = new SubDatabase[instance.DatabaseCount];
            for(int i = 0, length = instance.DatabaseCount; i < length; i++) {
                var db = instance.GetSubDatabase(i);
                if(db == null) // if the database reference is null, do not try to initialize.
                    continue;
                // Add Database to internal storage
                databases[i] = db;
                databaseLookup.Add(db.Id, db);
                // Add items to storage
                for(int index = 0, itemCount = db.Length; index < itemCount; index++) {
                    var item = db[index];
                    if(item == null) // if the item is null, skip to ensure everything else is loaded. This should only fail if files have been deleted and database not updated accordingly.
                        continue;
                    if(itemLookup.ContainsKey(item.Id)) {
                        if(itemLookup[item.Id] != item) {
                            Debug.LogError("Multiple Objects sharing same id (" + item.ItemName + ")"); // FIX LOG
                        }
                        continue;
                    }
                    else {
                        uniqueItems.Add(item);
                        itemLookup.Add(item.Id, item);
                    }
                }
            }
            items = uniqueItems.ToArray();
        }

        public static void ReloadDatabase() {
            itemLookup.Clear();
            databaseLookup.Clear();
            InitalizeDatabase(DatabaseReferences.Instance);
        }

        #endregion

        #region Variables

        private static Item[] items = { };
        private static Dictionary<int, Item> itemLookup = new Dictionary<int, Item>();

        private static SubDatabase[] databases = { };
        private static Dictionary<int, SubDatabase> databaseLookup = new Dictionary<int, SubDatabase>();

        #endregion

        #region Properties

        public static Item[] All => items;
        public static int ItemCount => items.Length;
        public static int DatabaseCount => databases.Length;

        #endregion

        #region Filters

        public static PrefabItem[] AllPrefabs => AllOf<PrefabItem>();

        public static T[] AllOf<T>() => items.WhereSelect(x => x is T t ? (true, t) : (false, default)).ToArray();
        public static T[] AllOf<T>(System.Func<T, bool> predicate) where T : Item => items.Where(x => x is T).Select(x => x as T).TakeWhile(predicate).ToArray();

        #endregion

        #region Finding

        public static Item GetItemById(int id) => itemLookup[id];
        public static Item GetItemByIndex(int index) => items[index];
        public static Item GetItemByName(string name) {
#if UNITY_EDITOR
            try {
                return GetItemById(GetHashCode(name));
            }
            catch {
                Debug.LogError(TAG + $"Could not find item with specific name in database '{name}'");
                return null;
            }
#else
            return GetItemById(GetHashCode(name));
#endif
        }

        public static Item GetItemByIdSafe(int id) {
            if(itemLookup.TryGetValue(id, out Item value)) {
                return value;
            }
#if UNITY_EDITOR
            Debug.LogWarning(TAG + $"'SAFE' - Could not find item with specific id in database '{id}'");
#endif
            return null;
        }

        public static Item GetItemByNameSafe(string name) => GetItemByIdSafe(GetHashCode(name));

        public static T GetItemById<T>(int id) where T : Item => GetItemById(id) as T;
        public static T GetItemByIndex<T>(int index) where T : Item => GetItemByIndex(index) as T;
        public static T GetItemByName<T>(string name) where T : Item => GetItemByName(name) as T;

        #endregion

        #region Sub Database

        public static SubDatabase GetSubDatabaseById(int id) => databaseLookup[id];
        public static T GetSubDatabaseById<T>(int id) where T : SubDatabase => databaseLookup[id] as T;
        public static SubDatabase GetSubDatabaseByIndex(int index) => databases[index];
        public static T GetSubDatabaseByIndex<T>(int index) where T : SubDatabase => databases[index] as T;

        #endregion
    }
}
