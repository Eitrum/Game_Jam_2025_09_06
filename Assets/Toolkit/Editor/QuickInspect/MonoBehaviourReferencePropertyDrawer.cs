using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Toolkit.QuickInspect {
    [CustomPropertyDrawer(typeof(Component), true)]
    public class MonoBehaviourReferencePropertyDrawer : PropertyDrawer {

        #region Nesting Fixes

        private static int recursionLimit => QuickInspectSettings.RecursionLimit;

        private static HashSet<MonoBehaviour> alreadyRendering = new HashSet<MonoBehaviour>();
        private static int myNesting = 0;

        private struct Nesting : System.IDisposable {

            private int previousNesting;
            private MonoBehaviour monoBehaviour;

            public static Nesting Create(SerializedProperty property) {
                var n = new Nesting();
                n.previousNesting = myNesting++;
                n.monoBehaviour = property.objectReferenceValue as MonoBehaviour;
                alreadyRendering.Add(n.monoBehaviour);
                return n;
            }

            public void Dispose() {
                myNesting = previousNesting;
                alreadyRendering.Remove(monoBehaviour);
            }
        }

        #endregion

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var result = 0f;

            if(property.objectReferenceValue == property.serializedObject.targetObject)
                return EditorGUIUtility.singleLineHeight;

            if(property.objectReferenceValue == null || !property.isExpanded || !QuickInspectSettings.Enabled)
                return EditorGUIUtility.singleLineHeight;

            if(myNesting < recursionLimit)
                QuickInspectUtil.SetNestingLevel(property, 0);

            using(Nesting.Create(property)) {
                using(SerializedObject so = new SerializedObject(property.objectReferenceValue)) {
                    var iterator = so.GetIterator();
                    if(!iterator.NextVisible(true))
                        return EditorGUIUtility.singleLineHeight;

                    if(iterator.name.Equals("m_Script") && !iterator.NextVisible(false))
                        return EditorGUIUtility.singleLineHeight * 2 + 8;

                    do {
                        result += EditorGUI.GetPropertyHeight(iterator, true);
                    } while(iterator.NextVisible(false));
                }
            }

            return result + 8 + EditorGUIUtility.singleLineHeight;
        }

        #endregion

        #region Draw

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(QuickInspectSettings.RenderShadow)
                EditorGUI.DrawRect(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight), new Color(0.1f, 0.1f, 0.1f, 0.1f));
            var topArea = new Rect(position.x, position.y, position.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            // Handle object not drawn
            if(property.objectReferenceValue == null || (property.objectReferenceValue == property.serializedObject.targetObject) || !QuickInspectSettings.Enabled) {
                EditorGUI.PropertyField(topArea, property, label);
                if(property.objectReferenceValue == null)
                    DrawAdd(new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height), property);
                else
                    DrawRemove(new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height), property);
                return;
            }

            EditorGUI.ObjectField(topArea, property, label);
            property.isExpanded = EditorGUI.Foldout(topArea, property.isExpanded, label, true);
            var didRemove = DrawRemove(new Rect(topArea.x + topArea.width, topArea.y, topArea.height, topArea.height), property);

            if(!property.isExpanded || didRemove)
                return;

            if(myNesting < recursionLimit)
                QuickInspectUtil.SetNestingLevel(property, 0);

            using(Nesting.Create(property))
                DrawShowcase(position, property);
        }

        private void DrawShowcase(Rect position, SerializedProperty property) {
            using(new EditorGUI.DisabledScope(!QuickInspectSettings.AllowEdit)) {
                var color = QuickInspectUtil.GetColor(EditorGUI.indentLevel);
                using(new EditorGUI.IndentLevelScope(1)) {
                    position.y += 2f;
                    position.height -= 4;
                    position.x += 4f;
                    position.width -= 4f;
                    if(QuickInspectSettings.RenderColor) {
                        var outline = EditorGUI.IndentedRect(position);
                        EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + EditorGUIUtility.singleLineHeight, 2f, outline.height - EditorGUIUtility.singleLineHeight), color);
                        EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + outline.height - 2f, outline.width, 2f), color);
                    }

                    var posy = position.y + EditorGUIUtility.singleLineHeight;
                    using(var so = new SerializedObject(property.objectReferenceValue)) {
                        so.UpdateIfRequiredOrScript();
                        var iterator = so.GetIterator();
                        if(!iterator.NextVisible(true))
                            return;
                        if(iterator.name.Equals("m_Script") && !iterator.NextVisible(false)) {
                            EditorGUI.HelpBox(EditorGUI.IndentedRect(new Rect(position.x, posy, position.width, EditorGUIUtility.singleLineHeight)), "Contains no properties", MessageType.Info);
                            return;
                        }
                        do {
                            var height = EditorGUI.GetPropertyHeight(iterator, true);
                            var area = new Rect(position.x, posy, position.width, height);
                            EditorGUI.PropertyField(area, iterator, true);
                            posy += height;
                        } while(iterator.NextVisible(false));
                        so.ApplyModifiedProperties();
                    }
                }
            }
        }

        private bool ShouldDrawAdd() {
            var t = fieldInfo.FieldType;
            if(t.IsAbstract)
                return false;
            if(t == typeof(Component))
                return false;
            if(t == typeof(GameObject))
                return false;
            if(t == typeof(Transform))
                return false;
            if(t == typeof(MonoBehaviour))
                return false;
            if(t == typeof(Behaviour))
                return false;

            return true;
        }

        private bool ShouldAddComponent() {
            var t = fieldInfo.FieldType;
            if(t.IsAbstract)
                return false;
            if(t == typeof(Collider))
                return false;
            if(t == typeof(Renderer))
                return false;

            return true;
        }

        private void DrawAdd(Rect addArea, SerializedProperty property) {
            if(!ShouldDrawAdd())
                return;

            GUI.Label(addArea, EditorGUIUtility.IconContent("d_ol_plus"));

            try {
                var ev = Event.current;
                if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && addArea.Contains(ev.mousePosition) && property.serializedObject.targetObject is Component comp) {
                    if(!QuickInspectUtil.IsHoldingAlt) {
                        var existingComp = comp.GetComponentInChildren(fieldInfo.FieldType) ?? comp.GetComponentInParent(fieldInfo.FieldType);
                        if(existingComp != null) {
                            property.objectReferenceValue = existingComp;
                            return;
                        }
                    }
                    if(!ShouldAddComponent())
                        return;

                    var newComp =  Undo.AddComponent(comp.gameObject, fieldInfo.FieldType);
                    property.objectReferenceValue = newComp;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        private bool DrawRemove(Rect clearArea, SerializedProperty property) {
            GUI.Label(clearArea, EditorGUIUtility.IconContent("d_ol_minus"));

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && clearArea.Contains(ev.mousePosition)) {
                var obj = property.objectReferenceValue;
                property.objectReferenceValue = null;

                if(property.serializedObject.targetObjects.Length == 1 &&
                    QuickInspectUtil.IsHoldingAlt &&
                    obj is Component comp &&
                    (obj is not (Transform or GameObject)) &&
                    (!QuickInspectSettings.DialogueDeleteComponent || EditorUtility.DisplayDialog("Delete", $"Are you sure you want to delete '{comp.GetType().Name}' on GameObject '{comp.gameObject.name}'", "Yes", "Cancel"))
                    ) {
                    Undo.DestroyObjectImmediate(comp);
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}
