using UnityEditor;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    /// <summary>
    /// Utility class to draw navigation area cost as Unity does.
    /// </summary>
    public static class NavigationAreaCostEditor
    {
        private static UnityEditorInternal.ReorderableList areaCostList;
        private static UnityEditorInternal.ReorderableList areaCostCustomList;

        private static SerializedObject m_NavMeshProjectSettingsObject;
        private static SerializedProperty m_Areas;

        private static float[] temporaryReference;

        private static bool Verify() {

            if(m_NavMeshProjectSettingsObject == null) {
                UnityEngine.Object obj = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshProjectSettings");
                m_NavMeshProjectSettingsObject = new SerializedObject(obj);
            }
            if(m_Areas == null) {
                m_Areas = m_NavMeshProjectSettingsObject.FindProperty("areas");
            }
            if(areaCostList == null) {
                areaCostList = new UnityEditorInternal.ReorderableList(m_NavMeshProjectSettingsObject, m_Areas, false, true, false, false);
                areaCostList.drawElementCallback = DrawAreaCostListElement;
                areaCostList.drawHeaderCallback = DrawAreaCostListHeader;
                areaCostList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            if(areaCostCustomList == null) {
                areaCostCustomList = new UnityEditorInternal.ReorderableList(m_NavMeshProjectSettingsObject, m_Areas, false, true, false, false);
                areaCostCustomList.drawElementCallback = DrawAreaCostCustomListElement;
                areaCostCustomList.drawHeaderCallback = DrawAreaCostCustomListHeader;
                areaCostCustomList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if(temporaryReference == null || temporaryReference.Length != 32) {
                temporaryReference = new float[32];
            }

            return m_NavMeshProjectSettingsObject != null && m_Areas != null && areaCostList != null && areaCostCustomList != null;
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/AI/Navigation/Area Cost", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string obj) {
            DrawLayout();
        }

        public static void DrawLayout() {
            if(!Verify()) {
                return;
            }
            m_NavMeshProjectSettingsObject.Update();
            areaCostList.DoLayoutList();
            if(m_NavMeshProjectSettingsObject.hasModifiedProperties) {
                m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
            }
        }

        public static void Draw(Rect area) {
            if(!Verify()) {
                return;
            }
            m_NavMeshProjectSettingsObject.Update();
            areaCostList.DoList(area);
            if(m_NavMeshProjectSettingsObject.hasModifiedProperties) {
                m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
            }
        }

        public static void DrawCustomLayout(SerializedProperty floatArrayProperty) {
            if(!Verify()) {
                return;
            }

            if(!(floatArrayProperty.isArray && floatArrayProperty.arraySize == 32)) {
                return;
            }

            for(int i = 0; i < 32; i++) {
                temporaryReference[i] = floatArrayProperty.GetArrayElementAtIndex(i).floatValue;
            }

            m_NavMeshProjectSettingsObject.Update();
            areaCostCustomList.DoLayoutList();

            for(int i = 0; i < 32; i++) {
                floatArrayProperty.GetArrayElementAtIndex(i).floatValue = temporaryReference[i];
            }
        }

        public static void DrawCustomLayout(float[] multiplier) {
            if(!Verify()) {
                return;
            }
            temporaryReference = multiplier;
            m_NavMeshProjectSettingsObject.Update();
            areaCostCustomList.DoLayoutList();
        }

        public static void DrawCustomLayout(Rect area, float[] multiplier) {
            if(!Verify()) {
                return;
            }
            temporaryReference = multiplier;
            m_NavMeshProjectSettingsObject.Update();
            areaCostCustomList.DoList(area);
        }

        #region Default Drawing

        private static void DrawAreaCostListHeader(Rect rect) {
            GetAreaListRects(rect, out Rect colorRect, out Rect labelRect, out Rect nameRect, out Rect costRect);
            EditorGUI.LabelField(rect, "Area Cost", EditorStyles.boldLabel);
            EditorGUI.LabelField(nameRect, "Name", EditorStyles.boldLabel);
            EditorGUI.LabelField(costRect, "Cost", EditorStyles.boldLabel);
        }

        private static void DrawAreaCostListElement(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty areaProp = m_Areas.GetArrayElementAtIndex(index);
            if(areaProp == null)
                return;
            SerializedProperty nameProp = areaProp.FindPropertyRelative("name");
            SerializedProperty costProp = areaProp.FindPropertyRelative("cost");
            if(nameProp == null || costProp == null)
                return;

            rect.height -= 2f;
            GetAreaListRects(rect, out Rect colorRect, out Rect labelRect, out Rect nameRect, out Rect costRect);

            EditorGUI.DrawRect(colorRect, Color.black);
            EditorGUI.DrawRect(colorRect.Shrink(1f), Color.grey);
            EditorGUI.DrawRect(colorRect.Shrink(1f), GetAreaColor(index, 0.7f));

            bool builtInLayer = false;
            bool allowChangeName = true;
            bool allowChangeCost = true;
            switch(index) {
                case 0: // Default
                    builtInLayer = true;
                    allowChangeName = false;
                    allowChangeCost = true;
                    break;
                case 1: // NonWalkable
                    builtInLayer = true;
                    allowChangeName = false;
                    allowChangeCost = false;
                    break;
                case 2: // Jump
                    builtInLayer = true;
                    allowChangeName = false;
                    allowChangeCost = true;
                    break;
                default:
                    builtInLayer = false;
                    allowChangeName = true;
                    allowChangeCost = true;
                    break;
            }
            GUI.Label(labelRect, builtInLayer ? ("Built-in " + index) : ("User " + index));
            using(new EditorGUI.DisabledScope(!allowChangeName)) {
                EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
            }

            using(new EditorGUI.DisabledScope(!allowChangeCost)) {
                EditorGUI.PropertyField(costRect, costProp, GUIContent.none);
            }
        }

        #endregion

        #region Custom Drawing

        private static void DrawAreaCostCustomListHeader(Rect rect) {
            GetAreaListRects(rect, out Rect colorRect, out Rect labelRect, out Rect nameRect, out Rect costRect, out Rect modifierRect, out Rect defaultRect);
            EditorGUI.LabelField(rect, "Area Cost", EditorStyles.boldLabel);
            EditorGUI.LabelField(nameRect, "Name", EditorStyles.boldLabel);
            EditorGUI.LabelField(costRect, "Cost", EditorStyles.boldLabel);
            EditorGUI.LabelField(modifierRect, "Modifier", EditorStyles.boldLabel);
            EditorGUI.LabelField(defaultRect, "Default", EditorStyles.boldLabel);
        }

        private static void DrawAreaCostCustomListElement(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty areaProp = m_Areas.GetArrayElementAtIndex(index);
            if(areaProp == null)
                return;
            SerializedProperty nameProp = areaProp.FindPropertyRelative("name");
            SerializedProperty costProp = areaProp.FindPropertyRelative("cost");
            if(nameProp == null || costProp == null)
                return;

            rect.height -= 2f;
            GetAreaListRects(rect, out Rect colorRect, out Rect labelRect, out Rect nameRect, out Rect costRect, out Rect modifierRect, out Rect defaultRect);

            EditorGUI.DrawRect(colorRect, Color.black);
            EditorGUI.DrawRect(colorRect.Shrink(1f), Color.grey);
            EditorGUI.DrawRect(colorRect.Shrink(1f), GetAreaColor(index, 0.7f));

            bool builtInLayer = false;
            bool allowChangeCost = true;
            switch(index) {
                case 0: // Default
                    builtInLayer = true;
                    allowChangeCost = true;
                    break;
                case 1: // NonWalkable
                    builtInLayer = true;
                    allowChangeCost = false;
                    break;
                case 2: // Jump
                    builtInLayer = true;
                    allowChangeCost = true;
                    break;
                default:
                    builtInLayer = false;
                    allowChangeCost = true;
                    break;
            }
            GUI.Label(labelRect, builtInLayer ? ("Built-in " + index) : ("User " + index));
            using(new EditorGUI.DisabledScope(true)) {
                EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
            }
            var defaultValue = costProp.floatValue;

            using(new EditorGUI.DisabledScope(!allowChangeCost)) {
                temporaryReference[index] = EditorGUI.FloatField(costRect, temporaryReference[index] * defaultValue) / defaultValue;
            }

            temporaryReference[index] = EditorGUI.FloatField(modifierRect, temporaryReference[index]);

            using(new EditorGUI.DisabledScope(true)) {
                EditorGUI.FloatField(defaultRect, defaultValue);
            }
        }

        #endregion

        #region Area Rects

        private static void GetAreaListRects(Rect rect, out Rect stripeRect, out Rect labelRect, out Rect nameRect, out Rect costRect) {
            float stripeWidth = EditorGUIUtility.singleLineHeight * 0.8f;
            float labelWidth = EditorGUIUtility.singleLineHeight * 5;
            float costWidth = EditorGUIUtility.singleLineHeight * 4;
            float nameWidth = rect.width - stripeWidth - labelWidth - costWidth;
            float x = rect.x;
            stripeRect = new Rect(x, rect.y, stripeWidth - 4, rect.height);
            x += stripeWidth;
            labelRect = new Rect(x, rect.y, labelWidth - 4, rect.height);
            x += labelWidth;
            nameRect = new Rect(x, rect.y, nameWidth - 4, rect.height);
            x += nameWidth;
            costRect = new Rect(x, rect.y, costWidth, rect.height);
        }

        private static void GetAreaListRects(Rect rect, out Rect stripeRect, out Rect labelRect, out Rect nameRect, out Rect costRect, out Rect modifierRect, out Rect defaultRect) {
            float stripeWidth = EditorGUIUtility.singleLineHeight * 0.8f;
            float labelWidth = EditorGUIUtility.singleLineHeight * 5;
            float costWidth = EditorGUIUtility.singleLineHeight * 4;
            float modifierWidth = EditorGUIUtility.singleLineHeight * 3;
            float defaultWidth = EditorGUIUtility.singleLineHeight * 3;
            float nameWidth = rect.width - stripeWidth - labelWidth - costWidth - modifierWidth - defaultWidth;
            float x = rect.x;
            stripeRect = new Rect(x, rect.y, stripeWidth - 4, rect.height);
            x += stripeWidth;
            labelRect = new Rect(x, rect.y, labelWidth - 4, rect.height);
            x += labelWidth;
            nameRect = new Rect(x, rect.y, nameWidth - 4, rect.height);
            x += nameWidth;
            costRect = new Rect(x, rect.y, costWidth, rect.height);
            x += costWidth;
            modifierRect = new Rect(x, rect.y, modifierWidth, rect.height);
            x += modifierWidth;
            defaultRect = new Rect(x, rect.y, defaultWidth, rect.height);
        }

        #endregion

        #region Color

        private static int Bit(int a, int b) => (a & (1 << b)) >> b;
        // UnityEditor.NavigationWindow.cs (2019.3)
        public static Color GetAreaColor(int i, float alpha = 0.5f) {
            if(i == 0)
                return new Color(0, 0.75f, 1.0f, 0.5f);
            int r = (Bit(i, 4) + Bit(i, 1) * 2 + 1) * 63;
            int g = (Bit(i, 3) + Bit(i, 2) * 2 + 1) * 63;
            int b = (Bit(i, 5) + Bit(i, 0) * 2 + 1) * 63;
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, alpha);
        }

        #endregion
    }
}
