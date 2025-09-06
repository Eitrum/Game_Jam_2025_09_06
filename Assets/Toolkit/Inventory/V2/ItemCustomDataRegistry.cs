using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public static class ItemCustomDataRegistry {
        #region Variables

        private static Dictionary<ushort, Type> idToType = new Dictionary<ushort, Type>();
        private static Dictionary<Type, ushort> typeToId = new Dictionary<Type, ushort>();

        #endregion

        #region Get Methods

        public static ushort GetId<T>(T item) where T : IItemData {
            return GetId(item.GetType());
        }

        public static ushort GetId<T>()
            => GetId(typeof(T));

        public static ushort GetId(Type type)
            => typeToId.TryGetValue(type, out var id) ? id : default;

        #endregion

        #region Create ItemData

        public static bool TryCreate(ushort id, out IItemData data) {
            data = null;
            if(!idToType.TryGetValue(id, out var dataType))
                return false;

            try {
                data = Activator.CreateInstance(dataType) as IItemData;
                return data != null;
            }
            catch { return false; }
        }

        #endregion

        #region Register

        public static void Register<T>(ushort id) {
            try {
                var t = typeof(T);
                idToType.Add(id, t);
                typeToId.Add(t, id);
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}
