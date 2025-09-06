using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(ContainerBehaviour))]
    public class ContainerBehaviourEditor : Editor
    {
        public override void OnInspectorGUI() {
            var behaviour = target as ContainerBehaviour;
            var container = target as IContainer;
            var items = container.Items;

            using(new EditorGUILayout.VerticalScope("box")) {
                if(container.Count == 0) {
                    EditorGUILayout.LabelField("Empty", EditorStyles.boldLabel);
                }
                for(int i = 0; i < container.Count; i++) {
                    var item = items[i];
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField($"{i} - ", GUILayout.Width(20f));
                        if(item.Icon != null) {
                            var textureArea = GUILayoutUtility.GetRect(16f, 16f);
                            GUI.DrawTexture(textureArea, item.Icon);
                        }
                        ItemEditorUtility.DrawItem(item);
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("Drop", GUILayout.Width(60f)) && container.CanRemoveItem(item) && item.DropItem(behaviour.transform.GetPose())) {
                            container.RemoveItem(item);
                            i--;
                        }
                        if(GUILayout.Button("Destroy", GUILayout.Width(70f)) && container.RemoveItem(item)) {
                            item.DestroyItem();
                            i--;
                        }
                    }
                }
                if(Application.isPlaying) {
                    var obj = EditorGUILayout.ObjectField("Add Item", null, typeof(UnityEngine.Object), true);
                    if(obj != null) {
                        if(obj is GameObject go) {
                            var item = go.GetComponent<IItem>();
                            var reference = go.GetComponent<IItemReference>();
                            if(item != null) {
                                // Check if in world
                                if(!go.scene.IsValid()) {
                                    var newGo = Instantiate(go, behaviour.transform.position, behaviour.transform.rotation);
                                    item = newGo.GetComponent<IItem>();
                                }
                                if(container.AddItem(item)) {
                                    item.OnAddedToContainer(container);
                                }
                            }
                            if(reference != null && reference.Item != null && go.scene.IsValid()) {
                                item = reference.Item;
                                if(container.AddItem(item)) {
                                    reference.Item = null;
                                }
                            }
                        }
                        else if(obj is ScriptableObject so && so is IItem) {
                            var newSo = Instantiate(so);
                            var item = newSo as IItem;
                            if(container.AddItem(item) && !item.IsNull) {
                                item.OnAddedToContainer(container);
                            }
                        }
                    }
                }
            }
        }
    }
}
