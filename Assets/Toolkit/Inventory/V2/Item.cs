using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public class Item {
        #region Variables

        public Guid InstanceID { get; private set; }
        public int UID { get; private set; }

        private List<IItemData> itemDatas = new List<IItemData>();

        #endregion

        #region Properties

        public IReadOnlyList<IItemData> ItemData => itemDatas;
        public ItemBlueprint Blueprint => ItemUIDDatabase.GetBlueprint(UID);

        public string UIDAsName => ItemUIDDatabase.TryGetBlueprint(UID, out var bp) ? bp.UID : $"unknown ({UID})";

        #endregion

        #region Constructor

        public Item(int uid) {
            this.UID = uid;
            this.InstanceID = Guid.NewGuid();
        }

        public Item(string uid) {
            this.UID = ItemUIDDatabase.NameToId(uid);
            this.InstanceID = Guid.NewGuid();
        }

        public Item(int uid, Guid instanceId) {
            this.UID = uid;
            this.InstanceID = instanceId;
        }

        public Item(string uid, Guid instanceId) {
            this.UID = ItemUIDDatabase.NameToId(uid);
            this.InstanceID = instanceId;
        }

        /// <summary>
        /// Create from a TMLNode, handles deserialization
        /// </summary>
        public Item(Toolkit.IO.TMLNode node) {
            Deserialize(node);
        }

        #endregion

        #region Add / Remove

        public void Add<T>(T customData) where T : IItemData {
            customData.Parent = this;
            itemDatas.Add(customData);
        }

        public bool Remove<T>(T customData) where T : IItemData {
            return itemDatas.Remove(customData);
        }

        #endregion

        #region IndexOf

        public int IndexOf<T>(T data) where T : IItemData {
            for(int i = 0, len = itemDatas.Count; i < len; i++) {
                if(itemDatas[i].Equals(data)) {
                    return i;
                }
            }
            return -1;
        }

        public bool TryGetIndexOf<T>(T data, out int index) where T : IItemData {
            for(int i = 0, len = itemDatas.Count; i < len; i++) {
                if(itemDatas[i].Equals(data)) {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        #endregion

        #region Get Data

        public bool TryGetData<T>(out T data) {
            for(int i = 0, len = itemDatas.Count; i < len; i++) {
                if(itemDatas[i] is T tdata) {
                    data = tdata;
                    return true;
                }
            }

            data = default;
            return false;
        }

        public IEnumerable<T> GetAll<T>() where T : class {
            foreach(var i in itemDatas)
                if(i is T item)
                    yield return item;
        }

        public IEnumerable<IItemData> GetFiltered(Func<IItemData, bool> filter) {
            foreach(var i in itemDatas)
                if(filter(i))
                    yield return i;
        }

        #endregion

        #region Serialize

        public TMLNode GetTMLNode() {
            var node = new TMLNode(InstanceID.ToString());
            node.AddProperty("uid", UID);
            foreach(var t in itemDatas) {
                var tnode = node.AddNode($"{ItemCustomDataRegistry.GetId(t)}");
                t.Serialize(tnode);
            }
            return node;
        }

        public void Serialize(Toolkit.IO.TMLNode node) {
            node.AddNode(GetTMLNode());
        }

        public void Deserialize(Toolkit.IO.TMLNode node, bool keepCurrentGuid = false) {
            itemDatas.Clear();
            if(!keepCurrentGuid)
                InstanceID = Guid.Parse(node.Name);
            if(UID == 0)
                UID = node.GetInt("uid", UID);
            foreach(var t in node.Children) {
                if(!ushort.TryParse(t.Name, out var id)) {
                    Debug.LogError($"failed to parse itemdata on '{InstanceID}': {Toolkit.IO.TML.TMLParser.ToString(t, true)}");
                    continue;
                }
                if(!ItemCustomDataRegistry.TryCreate(id, out var itemData)) {
                    Debug.LogWarning($"failed to create custom data from id: '{id}'");
                    continue;
                }
                itemData.Parent = this;
                itemData.Deserialize(t);
                itemDatas.Add(itemData);
            }
        }

        #endregion

        #region String Override

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(InstanceID.ToString());
            sb.AppendLine(UIDAsName);
            sb.AppendLine($"data:{itemDatas.Count}");
            var itemType = typeof(Item);
            foreach(var item in itemDatas) {
                var t = item.GetType();
                sb.AppendLine($"--{t.FullName}--");
                var prop = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                foreach(var f in prop) {
                    if(f.PropertyType == itemType)
                        continue;
                    sb.AppendLine($"{f.Name} = {f.GetValue(item)}");
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
