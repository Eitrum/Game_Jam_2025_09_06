using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.UI
{
    [CustomEditor(typeof(BarImageDelayed))]
    public class BarImageDelayedInspector : Editor
    {
        #region Variables

        private SerializedProperty value;
        private SerializedProperty range;
        private SerializedProperty delay;
        private SerializedProperty duration;
        private SerializedProperty foreground;
        private SerializedProperty background;

        private SerializedProperty useTint;
        private SerializedProperty negativeTint;
        private SerializedProperty positiveTint;

        #endregion

        #region Init

        private void OnEnable() {
            value = serializedObject.FindProperty("value");
            range = serializedObject.FindProperty("range");
            delay = serializedObject.FindProperty("delay");
            duration = serializedObject.FindProperty("duration");
            foreground = serializedObject.FindProperty("foreground");
            background = serializedObject.FindProperty("background");

            useTint = serializedObject.FindProperty("useTint");
            negativeTint = serializedObject.FindProperty("negativeTint");
            positiveTint = serializedObject.FindProperty("positiveTint");
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();

            var bar = (target as IBar);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Graphics", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(foreground);
                EditorGUILayout.PropertyField(background);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStylesUtility.BoldLabel);
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
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(delay);
                EditorGUILayout.PropertyField(duration);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Tint (Background)", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(useTint);
                using(new EditorGUI.DisabledScope(!useTint.boolValue)) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(positiveTint);
                    EditorGUILayout.PropertyField(negativeTint);
                    EditorGUI.indentLevel--;
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Add Object

        [MenuItem("GameObject/UI/Bar (Image Delayed)")]
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
                var bar = go.AddComponent<BarImageDelayed>();

                var background = new GameObject("background", typeof(RectTransform), typeof(UnityEngine.UI.Image));
                background.transform.SetParent(go.transform, false);
                var bgRect = background.transform.ToRectTransform();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.sizeDelta = Vector2.zero;


                var foreground = new GameObject("foreground", typeof(RectTransform), typeof(UnityEngine.UI.Image));
                foreground.transform.SetParent(background.transform, false);
                var fgRect = foreground.transform.ToRectTransform();
                fgRect.anchorMin = Vector2.zero;
                fgRect.anchorMax = Vector2.one;
                fgRect.sizeDelta = Vector2.zero;

                bar.Foreground = foreground.GetComponent<UnityEngine.UI.Image>();
                bar.Background = background.GetComponent<UnityEngine.UI.Image>();
                bar.Foreground.sprite = defaultSprite;
                bar.Background.sprite = defaultSprite;
                bar.Foreground.color = ColorTable.Azure;
                bar.Background.color = ColorTable.IndianRed;
                bar.Foreground.type = UnityEngine.UI.Image.Type.Filled;
                bar.Background.type = UnityEngine.UI.Image.Type.Filled;
                bar.Foreground.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
                bar.Background.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

                Selection.activeGameObject = go;
            }
        }

        [MenuItem("GameObject/UI/Bar (Image Delayed)", validate = true)]
        public static bool CanAddBarImage() {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInParent<Canvas>() != null;
        }

        #endregion
    }
}
