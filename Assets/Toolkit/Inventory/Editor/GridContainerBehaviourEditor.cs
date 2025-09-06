using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(GridContainerBehaviour))]
    public class GridContainerBehaviourEditor : Editor
    {
        private SerializedProperty widthProperty;
        private SerializedProperty heightProperty;

        private int selected = -1;
        private float gridScale = 40f;
        private Vector2 dragOffset = Vector2.zero;
        private Vector2 delta = Vector2.zero;

        private void OnEnable() {
            widthProperty = serializedObject.FindProperty("width");
            heightProperty = serializedObject.FindProperty("height");
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }

        public override void OnInspectorGUI() {
            var behaviour = target as GridContainerBehaviour;
            var container = target as IGridContainer;
            var items = container.Items;
            var areas = container.Areas;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                widthProperty.intValue = Mathf.Max(1, EditorGUILayout.IntField("Width", widthProperty.intValue));
                heightProperty.intValue = Mathf.Max(1, EditorGUILayout.IntField("Height", heightProperty.intValue));

                if(serializedObject.hasModifiedProperties) {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            var w = container.Width;
            var h = container.Height;

            var rect = GUILayoutUtility.GetRect(w * gridScale, h * gridScale);
            var pos = rect.position;
            Color backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    EditorGUI.DrawRect(new Rect(pos.x + x * gridScale, pos.y + y * gridScale, gridScale - 2, gridScale - 2), backgroundColor);
                }
            }

            Color iconColor = Color.white;

            var ev = Event.current;
            for(int i = 0, length = items.Count; i < length; i++) {

                var item = items[i];
                var area = areas[i];

                var guiArea = new Rect(pos.x + area.x * gridScale, pos.y + area.y * gridScale, area.w * gridScale - 1, area.h * gridScale - 1);

                if(selected == i) {
                    EditorGUI.DrawRect(guiArea, Color.cyan);
                    guiArea.ShrinkRef(2f);
                }

                EditorGUI.DrawRect(guiArea, iconColor);
                EditorGUI.LabelField(guiArea, item.Name, EditorStyles.centeredGreyMiniLabel);

                if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && guiArea.Contains(ev.mousePosition)) {
                    selected = i;
                }
                if(ev != null) {
                    if(selected == i) {
                        if(ev.type == EventType.MouseUp) {
                            if(dragOffset.sqrMagnitude > 1f) {
                                var offset = (dragOffset / gridScale).To_Int();
                                if(container.RemoveItem(item)) {
                                    var newArea = area;
                                    newArea.position += offset;
                                    if(!container.HasItemInArea(newArea, out IItem otherItem)) {
                                        if(!container.AddItemAtLocation(item, newArea.position)) {
                                            container.AddItemAtLocation(item, area.position);
                                        }
                                    }
                                    else {
                                        container.AddItemAtLocation(item, area.position);
                                    }
                                }
                                selected = -1;
                            }
                            dragOffset = Vector2.zero;
                        }
                        if(ev.type == EventType.MouseDrag) {
                            dragOffset += ev.delta;
                        }
                    }
                }
                if(selected == i && dragOffset.sqrMagnitude > 1f) {
                    guiArea.position += dragOffset;
                    EditorGUI.DrawRect(guiArea, Color.red);
                    EditorGUI.LabelField(guiArea, item.Name, EditorStyles.centeredGreyMiniLabel);
                }
            }

            if(selected >= 0 && selected < items.Count) {
                var item = items[selected];
                using(new EditorGUILayout.VerticalScope("box")) {
                    ItemEditorUtility.DrawItem(item);
                    using(new EditorGUILayout.HorizontalScope()) {
                        if(GUILayout.Button("Drop", GUILayout.Width(60f)) && container.CanRemoveItem(item) && item.DropItem(behaviour.transform.GetPose())) {
                            container.RemoveItem(item);
                            selected = -1;
                        }
                        if(GUILayout.Button("Destroy", GUILayout.Width(70f)) && container.RemoveItem(item)) {
                            item.DestroyItem();
                            selected = -1;
                        }
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
