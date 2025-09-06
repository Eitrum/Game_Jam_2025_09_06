using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    public static class ItemEditorUtility
    {
        public static void DrawItem(Rect area, IItem item) {
            string text = $"{item.Name} ({item.Type}) ({item.Rarity})";
            if(item is IStackable stackable) {
                text += $" ({stackable.CurrentStackSize}/{stackable.MaxStackSize})";
            }
            if(item is IItemWeight weighted) {
                text += $" ({weighted.Weight}kg)";
            }
            if(item is ISize size) {
                text += $" ({size.GridSize.x}, {size.GridSize.y})";
            }
            EditorGUI.LabelField(area, text);
        }

        public static void DrawItem(IItem item) {
            string text = $"{item.Name} ({item.Type}) ({item.Rarity})";
            if(item is IStackable stackable) {
                text += $" ({stackable.CurrentStackSize}/{stackable.MaxStackSize})";
            }
            if(item is IItemWeight weighted) {
                text += $" ({weighted.Weight}kg)";
            }
            if(item is ISize size) {
                text += $" ({size.GridSize.x}, {size.GridSize.y})";
            }
            EditorGUILayout.LabelField(text);
        }
    }
}
