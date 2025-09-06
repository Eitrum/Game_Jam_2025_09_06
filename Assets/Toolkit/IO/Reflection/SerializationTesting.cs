using System;
using System.Collections.Generic;
using Toolkit.IO.TML;
using UnityEngine;

namespace Toolkit.IO.TReflection.Internal {
    internal class SerializationTesting : MonoBehaviour {

        #region Variables

        public bool PrintLogs = true;
        public bool ClearSerializationCache = true;
        public bool Deserialize = true;

        public ItemLocations itemLocations = new ItemLocations();
        public Catalogue catalogue = new Catalogue();

        [Header("Output")]
        public bool showResults;
        private TMLNode node = null;
        [ShowIf("showResults")]  public byte[] bytes = { };
        [ShowIf("showResults")] public ItemLocations outputItemLocations = new ItemLocations();
        [ShowIf("showResults")] public Catalogue outputCatalogue = new Catalogue();

        #endregion

        #region Util

        [Button]
        private void PrintTree() {
            ClassSerializer.Get<Catalogue>().Tree().Print();
        }

        [Button]
        private void StartGC() {
            GC.Collect();
        }

        [Button]
        private void PerfTesting() {
            using(new PerformanceUtility.StopwatchScope(this)) {
                outputCatalogue = new Catalogue();
                outputCatalogue.CatalogueId = 42;
                outputCatalogue.users = new User[3] {
                    new User() { FirstName = "hello, world", LastName = "something different", Location = "stökhölm" },
                    new User() { FirstName = "hello, world2", LastName = "something different2", Location = "stökhölm2" },
                    new User() { FirstName = "hello, world3", LastName = "something different3", Location = "stökhölm3" } };
                outputCatalogue.LastWritten = DateTime.Now;
                outputCatalogue.bannedUsers.Clear();
                outputCatalogue.bannedUsers.Add(new User() { FirstName = "hello, world15", LastName = "something different15", Location = "stökhölm15" });
            }
        }

        #endregion

        #region Item Locations

        [Button]
        private void SerializeItemLocation_TML() {
            using(new PerformanceUtility.StopwatchScope(this)) {
                node = TMLClassSerializer.Get<ItemLocations>().Serialize(itemLocations);
                if(Deserialize)
                    outputItemLocations = TMLClassSerializer.Get<ItemLocations>().Deserialize<ItemLocations>(node);
            }
            TML.TMLUtility.Debug("serializationtesting", node);
            if(PrintLogs)
                Debug.Log(TML.TMLParser.ToString(node, true));
        }

        [Button]
        private void SerializeItemLocation_Binary() {
            using(new PerformanceUtility.StopwatchScope(this)) {
                bytes = BinaryClassSerializer.Get<ItemLocations>().Serialize(itemLocations).GetWrittenBuffer();
                if(Deserialize)
                    outputItemLocations = BinaryClassSerializer.Get<ItemLocations>().Deserialize<ItemLocations>(new Buffer(bytes));
            }
            if(PrintLogs)
                Debug.Log(this.FormatLog("serialized location"));
        }

        #endregion

        #region Catalogue

        [Button]
        private void SerializeCatalogue_TML() {
            using(new PerformanceUtility.StopwatchScope(this)) {
                node = TMLClassSerializer.Get<Catalogue>().Serialize(catalogue);
                if(Deserialize)
                    outputCatalogue = TMLClassSerializer.Get<Catalogue>().Deserialize<Catalogue>(node);
            }
            TML.TMLUtility.Debug("serializationtesting", node);
            if(PrintLogs)
                Debug.Log(TML.TMLParser.ToString(node, true));
        }

        [Button]
        private void SerializeCatalogue_Binary() {
            using(new PerformanceUtility.StopwatchScope(this)) {
                bytes = BinaryClassSerializer.Get<Catalogue>().Serialize(catalogue).GetWrittenBuffer();
                if(Deserialize)
                    outputCatalogue = BinaryClassSerializer.Get<Catalogue>().Deserialize<Catalogue>(new Buffer(bytes));
            }
            if(PrintLogs)
                Debug.Log(this.FormatLog("serialized catalogue"));
        }

        #endregion


        [System.Serializable]
        public class ItemLocations {
            public string ItemName = "Rock";
            public List<Coord> coords = new List<Coord>();
            public Coord[] coords2 = new Coord[3];
        }

        [System.Serializable]
        public struct Coord {
            public float x;
            public float y;

            [field: SerializeField] public float Z { get; set; }
        }

        [System.Serializable]
        public class Catalogue {
            public long CatalogueId;
            public DateTime LastWritten = DateTime.UtcNow;

            public User[] users = { new User(), new User(), new User() };
            public List<User> bannedUsers = new List<User>() { new User() };
        }

        [System.Serializable]
        public class User {
            public string FirstName = "Sherlock";
            public string LastName = "Holmes";

            [SerializeField] private string location = "Bakers Street 221";

            [SerializeField] private int[] ids = { 1, 2, 3 };

            public string Location { get => location; set => location = value; }
            public IReadOnlyList<int> IDs => ids;
        }
    }
}
