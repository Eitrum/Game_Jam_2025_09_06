using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.UI
{
    [CustomEditor(typeof(BarImage))]
    public class BarImageInspector : Editor
    {
        #region Variables

        private SerializedProperty value;
        private SerializedProperty range;
        private SerializedProperty image;

        #endregion

        #region Init

        private void OnEnable() {
            value = serializedObject.FindProperty("value");
            range = serializedObject.FindProperty("range");
            image = serializedObject.FindProperty("image");
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();

            var bar = (target as IBar);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(image);
                var obj = image.objectReferenceValue;
                if(obj == null)
                    EditorGUILayout.HelpBox("Missing graphics object!", MessageType.Warning);
                else if(obj is UnityEngine.UI.Image image) {
                    if(image.sprite == null)
                        EditorGUILayout.HelpBox("Image is missing a sprite!", MessageType.Warning);
                    else if(image.type != UnityEngine.UI.Image.Type.Filled)
                        EditorGUILayout.HelpBox("Image not in fill mode!", MessageType.Warning);
                }

                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUILayout.Slider("Value", value.floatValue, range.FindPropertyRelative("min").floatValue, range.FindPropertyRelative("max").floatValue);
                if(EditorGUI.EndChangeCheck()) {
                    value.floatValue = newValue;
                    bar.Value = newValue;
                }
                EditorGUI.BeginChangeCheck();
                var norm = bar.NormalizedValue;
                EditorGUILayout.PropertyField(range);
                if(EditorGUI.EndChangeCheck()) {
                    serializedObject.ApplyModifiedProperties();
                    bar.NormalizedValue = norm;
                    serializedObject.Update();
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Add Object

        [MenuItem("GameObject/UI/Bar (Image)")]
        public static void AddBarImage() {
            var active = Selection.activeGameObject;
            if(active != null) {
                if(active.GetComponentInParent<Canvas>() == null) {
                    Debug.LogError("Unable to add UI bar, need to be child of canvas.");
                    return;
                }
                var defaultSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Toolkit/UI/white.png");

                var go = new GameObject("new bar", typeof(RectTransform));
                go.transform.SetParent(active.transform, false);
                go.transform.ToRectTransform().sizeDelta = new Vector2(200, 20);
                var bImage = go.AddComponent<UnityEngine.UI.Image>();
                bImage.color = ColorTable.SlateGray;
                bImage.sprite = defaultSprite;
                var bar = go.AddComponent<BarImage>();

                var foreground = new GameObject("image", typeof(RectTransform), typeof(UnityEngine.UI.Image));
                foreground.transform.SetParent(go.transform, false);
                var fgRect = foreground.transform.ToRectTransform();
                fgRect.anchorMin = Vector2.zero;
                fgRect.anchorMax = Vector2.one;
                fgRect.sizeDelta = Vector2.zero;

                bar.Image = foreground.GetComponent<UnityEngine.UI.Image>();
                bar.Image.color = ColorTable.Azure;
                bar.Image.type = UnityEngine.UI.Image.Type.Filled;
                bar.Image.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
                
                Selection.activeGameObject = go;
            }
        }

        [MenuItem("GameObject/UI/Bar (Image)", validate = true)]
        public static bool CanAddBarImage() {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInParent<Canvas>() != null;
        }

        #endregion
    }
}
