using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [CustomEditor(typeof(ItemBlueprint))]
    public class ItemBlueprintInspector : Editor {

        private SerializedProperty uid;
        private NSONestedArrayEditor nestedArrayEditor;
        private TMLNode generatedItem;
        private string generatedItemText;

        private void OnEnable() {
            nestedArrayEditor = new NSONestedArrayEditor(serializedObject.FindProperty("dataBlueprints"), header: null, alwaysExpanded: true, reorderable: true);
            nestedArrayEditor.OnUpdated += Repaint;
            nestedArrayEditor.DrawHeader = false;
            uid = serializedObject.FindProperty("uid");
        }

        private void OnDisable() {
            nestedArrayEditor?.Dispose();
        }

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(uid, EditorGUIUtility.TrTempContent("UID:"));
                EditorGUILayout.Space();
                nestedArrayEditor.DrawLayout();
            }
        }

        public override bool HasPreviewGUI() {
            return true;
        }

        public override void DrawPreview(Rect previewArea) {
            previewArea.ShrinkRef(4);
            var genButton = new Rect(previewArea.x, previewArea.y, 120, EditorGUIUtility.singleLineHeight);
            previewArea = new Rect(previewArea.x, previewArea.y + EditorGUIUtility.singleLineHeight, previewArea.width, previewArea.height - EditorGUIUtility.singleLineHeight);
            if(GUI.Button(genButton, "Generate new")) {
                var bp = target as ItemBlueprint;
                generatedItem = bp.GenerateTMLItem();
                generatedItemText = Toolkit.IO.TML.TMLParser.ToString(generatedItem, true);
            }
            GUI.TextArea(previewArea, generatedItemText);
        }
    }
}
