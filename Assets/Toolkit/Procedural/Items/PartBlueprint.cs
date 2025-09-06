using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using Toolkit.Serializable;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    [System.Serializable]
    public class PartBlueprint : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private byte[] bytes = null;
        [System.NonSerialized] private int id = 0;
        [System.NonSerialized] private List<PartBlueprint> branches = new List<PartBlueprint>();
        public int Id => id;
        public IReadOnlyList<PartBlueprint> Branches => branches;

        private PartBlueprint() { }
        internal PartBlueprint(int id) => this.id = id;

        internal void AddBranch(PartBlueprint branch) {
            branches.Add(branch);
        }

        public PartBlueprint GetBranch(int index) => branches[index];

        #region To String

        public override string ToString() {
            return $"Blueprint\n{id}{branches.Select(x => x.ToString(1)).CombineToString()}";
        }

        private string ToString(int indent) {
            return $"\n{Indent(indent)}{id}{branches.Select(x => x.ToString(indent + 1)).CombineToString()}";
        }

        public string ToString(PartAssembly assembly) {
            var entry = assembly.Root.Collection.GetEntry(id);
            if(entry == null)
                return "";
            var tBranches = assembly.Root.Branches;
            return $"Blueprint\n({id}){entry.Prefab.name}{branches.SelectWithIndex((x, i) => x.ToString(tBranches[i], 1)).CombineToString()}";
        }

        private string ToString(PartAssembly.Branch branch, int indent) {
            var entry = branch.Collection.GetEntry(id);
            if(entry == null)
                return "";
            var tBranches = branch.Branches;
            return $"\n{Indent(indent)}({id}){entry.Prefab.name}{branches.SelectWithIndex((x, i) => x.ToString(tBranches[i], indent + 1)).CombineToString()}";
        }

        private string Indent(int indent) {
            const string LINES = "----------------";
            return LINES.Substring(0, indent);
        }

        #endregion

        #region Part Blueprint Serialization

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            Toolkit.IO.DynamicBuffer dynbuffer = new IO.DynamicBuffer(16);
            Serialize(dynbuffer);
            bytes = dynbuffer.GetWrittenBuffer();
        }

        private void Serialize(IBuffer buffer) {
            buffer.Write(id);
            var length = branches.Count;
            buffer.Write(length);
            for(int i = 0; i < length; i++) {
                branches[i].Serialize(buffer);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if(bytes == null)
                return;
            Deserialize(new Buffer(bytes));
        }

        private void Deserialize(IBuffer buffer) {
            branches.Clear();
            id = buffer.ReadInt();
            var length = buffer.ReadInt();
            for(int i = 0; i < length; i++) {
                var b = new PartBlueprint();
                branches.Add(b);
                b.Deserialize(buffer);
            }
        }

        #endregion
    }
}
