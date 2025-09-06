using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [CustomEditor(typeof(ItemReferenceComponent))]
    public class ItemReferenceComponentInspector : Editor {

        private Item cached;
        private TMLNode node;

        private void OnDisable() {
            cached = null;
            node = null;
        }

        public override void OnInspectorGUI() {
            var irc = (ItemReferenceComponent)target;
            var item = irc.Item;
            if(item == null) {
                if(EditorStylesUtility.IsHoldingAlt && GUILayout.Button("Test")) {
                    item = irc.Item = new Item("consumable/health_potion");
                    item.Add(new ItemMetadata("Health Potion", "Red strange brew", ItemType.Potion));
                }
                EditorGUILayout.HelpBox("No Item Reference", MessageType.Warning);
                return;
            }

            if(cached != item) {
                cached = item;
                node = item?.GetTMLNode();
            }

            Toolkit.IO.TML.TMLNodeDrawer.DrawNodeLayout(node, true);
        }
    }
}
