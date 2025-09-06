using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [AddComponentMenu("Toolkit/Items/Inventory")]
    public class InventoryComponent : MonoBehaviour, IInventory {
        #region Variables

        [SerializeField] private List<Container> containers = new List<Container>() { new Container() };

        #endregion

        #region Properties

        public IReadOnlyList<IContainer> Containers => containers;
        public IContainer this[int index] => containers[index];
        public int ContainerCount => containers.Count;

        #endregion

        #region Add / Remove

        public bool TryAdd(IContainer container) {
            if(!(container is Container con))
                return false;
            for(int i = containers.Count - 1; i >= 0; i--) {
                if(containers[i] == con)
                    return false;
            }
            containers.Add(con);
            return true;
        }

        public bool TryRemove(IContainer container) {
            for(int i = containers.Count - 1; i >= 0; i--) {
                if(containers[i] == container)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
