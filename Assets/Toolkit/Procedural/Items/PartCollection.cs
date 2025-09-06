using System.Collections;
using System.Collections.Generic;
using Toolkit.Inventory;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    [CreateAssetMenu(fileName = "new part collection", menuName = "Toolkit/Procedural/Items/Part Collection")]
    public class PartCollection : ScriptableObject
    {
        [SerializeField] private Part part = Part.None;
        [SerializeField] private Entry[] entries = null;


        public Part Part => part;
        public IReadOnlyList<Entry> Entries => entries;

        public Entry GetEntry() => entries.RandomElement();
        public Entry GetEntry(int index) {
            if(index < 0 || index >= entries.Length)
                return null;
            return entries[index];
        }
        public int GetEntryId() => entries.RandomIndex();
        public Entry GetEntry(System.Func<Entry, bool> searchCriteria) => entries.RandomElement(searchCriteria);
        public int GetEntryId(System.Func<Entry, bool> searchCriteria) => entries.RandomIndex(searchCriteria);

        [ContextMenu("Verify Rotations")]
        private void VerifyRotations() {
            Quaternion quaternion = new Quaternion(0, 0, 0, 0);
            foreach(var entry in entries) {
                if(entry.Offset.rotation == quaternion) {
                    Debug.LogError("found error at entry offset: " + entry.Prefab.name);
                }
                foreach(var con in entry.Connections) {
                    if(con.Anchor.rotation == quaternion) {
                        Debug.LogError("found error at connection: " + entry.Prefab.name + " - " + con.OtherPart);
                    }
                }
            }
        }

        [System.Serializable]
        public class Entry
        {
            [SerializeField] private GameObject prefab = null;
            [SerializeField] private PartConnection[] connections = null;
            [SerializeField] private Pose offset = Pose.identity;
            [SerializeField] private Rarity rarity = Rarity.None;
            [SerializeField] private Quality quality = Quality.None;

            public GameObject Prefab => prefab;
            public IReadOnlyList<PartConnection> Connections => connections;
            public Pose Offset => offset;
            public Rarity Rarity => rarity;
            public Quality Quality => quality;
        }
    }
}
