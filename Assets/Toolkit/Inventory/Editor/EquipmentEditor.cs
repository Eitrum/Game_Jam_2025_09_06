using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory {
    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : Editor {

        private EquipmentSlotMask[] maskObjects;
        private string[] maskNames;

        void OnEnable() {
            maskObjects = AssetDatabaseUtility.LoadAssets<EquipmentSlotMask>();
            maskNames = new string[maskObjects.Length + 1];
            maskNames[0] = "None";
            for(int i = 1; i < maskNames.Length; i++) {
                maskNames[i] = maskObjects[i - 1].name;
            }
        }

        public override void OnInspectorGUI() {
            #region Handle Custom Mask Dropdown
            var maskProperty = serializedObject.FindProperty("mask");
            var index = 0;
            var mask = maskProperty.objectReferenceValue as EquipmentSlotMask;
            if(mask != null) {
                for(int i = 0; i < maskObjects.Length; i++) {
                    if(maskObjects[i].name == mask.name) {
                        index = i + 1;
                        break;
                    }
                }
            }
            GUILayout.BeginHorizontal();
            index = EditorGUILayout.Popup("Mask", index, maskNames);
            if(index == 0)
                maskProperty.objectReferenceValue = mask = null;
            else
                maskProperty.objectReferenceValue = mask = maskObjects[index - 1];

            if(GUILayout.Button("New", GUILayout.Width(40f), GUILayout.Height(14f))) {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                var newMaskObject = ScriptableObject.CreateInstance<EquipmentSlotMask>();
                ProjectWindowUtil.CreateAsset(newMaskObject, AssetDatabase.GenerateUniqueAssetPath(path + "new mask.asset"));
                maskProperty.objectReferenceValue = newMaskObject;

            }
            GUILayout.EndHorizontal();

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region Custom Equipment View

            var equipment = target as Equipment;

            GUILayout.BeginVertical("box");

            for(int i = 0; i < equipment.EquipmentSlots; i++) {
                GUILayout.BeginHorizontal("box");
                var slot = equipment.GetEquipmentSlot(i);
                EditorGUILayout.Toggle(mask == null ? true : mask.IsSlotEnabled(slot), GUILayout.Width(20f));
                GUILayout.Label(slot.ToString(), GUILayout.Width(160f));
                if(Application.isPlaying) {
                    var item = equipment.GetEquipment(slot);
                    GUILayout.Label(item?.Name ?? "empty", GUILayout.Width(200f));
                    GUILayout.FlexibleSpace();
                    if(item != null) {
                        if(GUILayout.Button("Remove")) {
                            equipment.Unequip(slot);
                        }
                    }
                    else {
                        UnityEngine.Object obj = null;
                        obj = EditorGUILayout.ObjectField(obj, typeof(IItem), false);
                        if(obj != null && obj is IItem newItem) {
                            if(!equipment.Equip(newItem, slot)) {
                                Debug.LogError($"Could not equip item '{newItem.Name}' into slot '{slot.ToString()}'");
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            #endregion
        }

    }
}
