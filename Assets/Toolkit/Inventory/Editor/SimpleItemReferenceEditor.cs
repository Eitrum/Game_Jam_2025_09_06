using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(SimpleItemReference))]
    public class SimpleItemReferenceEditor : Editor
    {
        public override void OnInspectorGUI() {
            var reference = target as IItemReference;

            var item = reference.Item;
            if(item == null) {
                EditorGUILayout.LabelField("No Item", EditorStyles.boldLabel);
            }
            else {
                ItemEditorUtility.DrawItem(item);
            }
        }
    }
}
