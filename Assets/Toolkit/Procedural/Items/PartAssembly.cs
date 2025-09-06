using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.Inventory;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    [CreateAssetMenu(fileName = "new part assembly", menuName = "Toolkit/Procedural/Items/Part Assembly")]
    public class PartAssembly : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Variables

        [SerializeField] private Branch root = new Branch();
        [SerializeField, HideInInspector] private List<PartCollection> referencedCollections = new List<PartCollection>();
        [SerializeField, HideInInspector] private byte[] bytes = null;

        [Header("Meta")]
        [SerializeField] private bool attachBlueprintRefOnItem = false;

        #endregion

        #region Properties

        public Branch Root => root;

        #endregion

        #region Create Simple Prefab

        [ContextMenu("Create Prefab")]
        public GameObject CreatePrefab() {
            var blueprint = CreateBlueprint();
            var go = new GameObject(name + "_" + Random.Range(100000, 999999));
            GenerateBranch(go.transform, Part.None, Pose.identity, root, blueprint);
            if(attachBlueprintRefOnItem)
                go.AddComponent<PartBlueprintReference>().Assign(blueprint, this);
            Debug.Log(blueprint.ToString(this));
            return go;
        }

        public GameObject CreatePrefab(PartBlueprint blueprint) {
            var go = new GameObject(name + "_" + Random.Range(100000, 999999));
            GenerateBranch(go.transform, Part.None, Pose.identity, root, blueprint);
            if(attachBlueprintRefOnItem)
                go.AddComponent<PartBlueprintReference>().Assign(blueprint, this);
            Debug.Log(blueprint.ToString(this));
            return go;
        }

        public void CreateAssemblyInRoot(Transform root) {
            var blueprint = CreateBlueprint();
            GenerateBranch(root, Part.None, Pose.identity, this.root, blueprint);
            if(attachBlueprintRefOnItem) {
                root.GetOrAddComponent<PartBlueprintReference>().Assign(blueprint, this);
            }
        }

        public void CreateAssemblyInRoot(Transform root, PartBlueprint blueprint) {
            GenerateBranch(root, Part.None, Pose.identity, this.root, blueprint);
            if(attachBlueprintRefOnItem) {
                root.GetOrAddComponent<PartBlueprintReference>().Assign(blueprint, this);
            }
        }

        [ContextMenu("Create Prefab x16")]
        private void CreatePrefabX16() {
            var container = new GameObject($"{name}_4x4_container_{Random.Range(100000, 999999)}");
            for(int x = 0; x < 4; x++) {
                for(int z = 0; z < 4; z++) {
                    var blueprint = CreateBlueprint();
                    var go = new GameObject($"{name}_({x}, {z})_{Random.Range(100000, 999999)}");
                    go.transform.SetParent(container.transform);
                    go.transform.localPosition = new Vector3(x, 0, z);
                    GenerateBranch(go.transform, Part.None, Pose.identity, root, blueprint);
                    if(attachBlueprintRefOnItem)
                        go.AddComponent<PartBlueprintReference>().Assign(blueprint, this);
                }
            }
        }

        #endregion

        #region Create Blueprint

        public PartBlueprint CreateBlueprint() {
            if(root == null) {
                root = new Branch();
            }
            if(root.collection == null) {
                return null;
            }
            var rootBlueprint = new PartBlueprint(root.Collection.GetEntryId());
            GenerateBlueprint(rootBlueprint, root);
            return rootBlueprint;
        }

        public PartBlueprint CreateBlueprint(System.Func<PartCollection.Entry, bool> searchCriteria) {
            var rootBlueprint = new PartBlueprint(root.Collection.GetEntryId(searchCriteria));
            GenerateBlueprint(rootBlueprint, root, searchCriteria);
            return rootBlueprint;
        }

        private static void GenerateBlueprint(PartBlueprint parent, Branch branch) {
            foreach(var br in branch.Branches) {
                if(br.collection == null)
                    return;
                var child = new PartBlueprint(br.Collection.GetEntryId());
                parent.AddBranch(child);
                GenerateBlueprint(child, br);
            }
        }

        private static void GenerateBlueprint(PartBlueprint parent, Branch branch, System.Func<PartCollection.Entry, bool> searchCriteria) {
            foreach(var br in branch.Branches) {
                if(br.collection == null)
                    return;
                var child = new PartBlueprint(br.Collection.GetEntryId(searchCriteria));
                parent.AddBranch(child);
                GenerateBlueprint(child, br);
            }
        }

        #endregion

        #region Generate Branches

        private static void GenerateBranch(Transform root, Part parent, Pose offset, Branch branch, PartBlueprint blueprint) {
            // Setup correct entry
            var collection = branch.Collection;
            var entry = collection.GetEntry(blueprint.Id);

            // Handle offset
            if(parent == Part.None) {
                // Add root offset
                offset = new Pose(offset.position + entry.Offset.position, offset.rotation * entry.Offset.rotation);
            }
            else {
                // Remove anchor offset
                for(int i = 0, length = entry.Connections.Count; i < length; i++) {
                    if(entry.Connections[i].OtherPart == parent) {
                        var anchor = entry.Connections[i].Anchor;
                        offset = new Pose(offset.position - anchor.position, offset.rotation * Quaternion.Inverse(anchor.rotation));
                        break;
                    }
                }
            }
            // Add part to gameobject
            var go = Instantiate(entry.Prefab, root);
            go.transform.localPosition = offset.position;
            go.transform.localRotation = offset.rotation;
            // Generate sub branches
            // Find connections, for each connection, generate a branch
            for(int i = 0, length = entry.Connections.Count; i < length; i++) {
                var con = entry.Connections[i];
                // For each connection, find a branch of matching part type
                for(int brIndex = 0, brLength = branch.BranchCount; brIndex < brLength; brIndex++) {
                    var br = branch.GetBranch(brIndex);
                    if(con.OtherPart == br.Collection.Part) {
                        var anchor = con.Anchor;
                        GenerateBranch(root, collection.Part, new Pose(offset.position + anchor.position, offset.rotation * anchor.rotation), br, blueprint.GetBranch(brIndex));
                        break;
                    }
                }
            }
        }

        #endregion

        #region ISerialization Callback

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            referencedCollections.Clear();
            referencedCollections.AddRange(root.GetCollections());

            DynamicBuffer buffer = new DynamicBuffer(16);
            root.Serialize(referencedCollections, buffer);
            bytes = buffer.GetWrittenBuffer();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if(bytes == null || bytes.Length == 0)
                return;
            root.Deserialize(referencedCollections, new Buffer(bytes));
        }

        #endregion

        [System.Serializable]
        public class Branch
        {
            #region Variables

            [System.NonSerialized] internal PartCollection collection = null;
            [System.NonSerialized] internal List<Branch> branches = new List<Branch>();

            #endregion

            #region Properties

            public PartCollection Collection => collection;
            public int BranchCount => branches.Count;
            public IReadOnlyList<Branch> Branches => branches;
            public Branch GetBranch(int index) => branches[index];

            #endregion

            #region Constructor

            public Branch() { }
            public Branch(PartCollection collection) => this.collection = collection;

            #endregion

            #region Serialization

            internal IEnumerable<PartCollection> GetCollections() {
                yield return collection;
                foreach(var br in branches)
                    foreach(var col in br.GetCollections())
                        yield return col;
            }

            internal void Serialize(IReadOnlyList<PartCollection> collections, IBuffer buffer) {
                int id = -1;
                for(int i = 0, length = collections.Count; i < length; i++) {
                    if(collections[i] == collection) {
                        id = i;
                        break;
                    }
                }
                buffer.Write(id);
                var branchCount = branches.Count;
                buffer.Write(branchCount);
                for(int i = 0; i < branchCount; i++) {
                    branches[i].Serialize(collections, buffer);
                }
            }

            internal void Deserialize(IReadOnlyList<PartCollection> collections, IBuffer buffer) {
                var id = buffer.ReadInt();
                branches.Clear();
                if(id == -1) {
                    collection = null;
                }
                else {
                    collection = collections[id];
                }
                var length = buffer.ReadInt();
                for(int i = 0; i < length; i++) {
                    var b = new Branch();
                    branches.Add(b);
                    b.Deserialize(collections, buffer);
                }
            }

            #endregion
        }
    }
}
