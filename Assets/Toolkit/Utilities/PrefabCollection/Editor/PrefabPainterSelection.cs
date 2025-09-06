using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Utility
{
    public class PrefabPainterSelection
    {
        #region Variables

        private PrefabCollection selectedCollection;
        private PrefabCollection.Entry selectedEntry;
        private static Vector2 scroll;
        private IReadOnlyList<PrefabCollection> Collections;
        private bool isDone = false;
        private Toolkit.PreviewRenderer renderer;

        #endregion

        #region Properties

        public bool IsDone => isDone;
        public PrefabCollection Collection => selectedCollection;
        public PrefabCollection.Entry Entry => selectedEntry;

        #endregion

        #region Constructor

        public PrefabPainterSelection(PrefabCollection collection, PrefabCollection.Entry entry, Toolkit.PreviewRenderer renderer) {
            this.selectedCollection = collection;
            if(collection != null)
                this.selectedEntry = entry;
            Collections = PrefabCollectionUtility.Collections;
            this.renderer = renderer;
        }

        #endregion

        #region Drawing

        public void OnGUI(Rect rect) {
            if(selectedCollection == null)
                scroll = DrawGrid(rect, scroll, new Vector2(120, 120), Collections.Count, DrawCollectionItem);
            else
                scroll = DrawGrid(rect, scroll, new Vector2(120, 120), selectedCollection.EntriesCount + 1, DrawEntryItem);
        }

        private void DrawCollectionItem(Rect rect, int index) {
            var ev = Event.current;
            rect.ShrinkRef(4f);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 0.4f));
            if(PrefabPainter.FolderIcon)
                GUI.DrawTexture(rect, PrefabPainter.FolderIcon);
            var textArea = rect.Pad(0, 0, rect.height - 16, 0);
            EditorGUI.DropShadowLabel(textArea, Collections[index].CollectionName);
            if(ev.type == EventType.MouseDown && ev.button == 0 && rect.Contains(ev.mousePosition)) {
                selectedCollection = Collections[index];
                ev.Use();
            }
        }

        private void DrawEntryItem(Rect rect, int index) {
            var ev = Event.current;
            rect.ShrinkRef(4f);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 0.4f));
            if(index == 0) { // Draw back thingy  
                if(PrefabPainter.FolderBackIcon)
                    GUI.DrawTexture(rect, PrefabPainter.FolderBackIcon);
                if(ev.type == EventType.MouseDown && ev.button == 0 && rect.Contains(ev.mousePosition)) {
                    EditorApplication.delayCall += () => selectedCollection = null;
                    ev.Use();
                }
            }
            else {
                // Setup
                var textArea = rect.Pad(0, 0, rect.height - 16, 0);
                var entry = selectedCollection.GetEntry(index - 1);
                bool isSelected = selectedEntry == entry;
                var previewArea = rect.Shrink(20f);

                // Handle Input
                if(ev.type == EventType.MouseDown && ev.button == 0 && rect.Contains(ev.mousePosition)) {
                    selectedEntry = entry;
                    isDone = true;
                    ev.Use();
                }

                // Render
                if(entry.DefaultRenderer != null) {
                    var b = entry.DefaultRenderer.Bounds;
                    var s = (1f / b.extents.magnitude);
                    var centerTranslate = -b.center;
                    var trs = Matrix4x4.TRS(centerTranslate * s, Quaternion.identity, s.To_Vector3());

                    renderer.BeginRender(previewArea);
                    foreach(var inst in entry.DefaultRenderer.Instances) {
                        renderer.RenderCustom(trs * inst.Offset, inst.Mesh, inst.Material);
                    }
                    renderer.EndRender(previewArea);
                }
                EditorGUI.DropShadowLabel(textArea, entry.Name);
            }
        }

        public static Vector2 DrawGrid(Rect area, Vector2 scroll, Vector2 contentSize, int items, Action<Rect, int> callback) {
            var width = (int)(area.width / contentSize.x);
            if(width <= 0) {
                Debug.LogError("Can't fit content width into area!");
                return scroll;
            }
            var height = Mathf.Max(Mathf.CeilToInt((items) / (float)(width)), 1);
            var isScroll = (height * contentSize.y) > area.height;

            if(isScroll && width * contentSize.x - 16 > area.width) {
                Debug.LogError("Can't fit content width into area with a scrollbar!");
                return scroll;
            }

            using(var scrollObject = new GUI.ScrollViewScope(area, scroll, new Rect(0, 0, width * contentSize.x, height * contentSize.y))) {
                scroll = scrollObject.scrollPosition;
                int index = 0;
                for(int y = 0; y < height; y++) {
                    for(int x = 0; x < width; x++) {
                        if(index >= items)
                            return scroll;
                        var rect = new Rect(x * contentSize.x, y * contentSize.y, contentSize.x, contentSize.y);
                        callback?.Invoke(rect, index);
                        index++;
                    }
                }
            }

            return scroll;
        }

        #endregion
    }
}
