using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(PhysicsLayerSettings))]
    public class PhysicsLayerSettingsInspector : Editor
    {
        #region Variables

        private static MethodInfo layerMatrixGUIDrawMethod;
        private static System.Type getValueFunc;
        private static System.Type setValueFunc;
        private object[] parameters = new object[4];
        private static GUIContent layerMatrix = new GUIContent("Layer Matrix");
        private Vector2 scroll;

        private SerializedProperty layerNameOverrides;
        private SerializedProperty collisonMask;

        #endregion

        #region Constructor

        static PhysicsLayerSettingsInspector() {
            var lmgui = typeof(UnityEditor.EditorApplication).Assembly.GetType("UnityEditor.LayerMatrixGUI");
            layerMatrixGUIDrawMethod = lmgui.GetMethod("DoGUI", BindingFlags.Static | BindingFlags.Public);
            getValueFunc = typeof(UnityEditor.EditorApplication).Assembly.GetType("UnityEditor.LayerMatrixGUI+GetValueFunc");
            setValueFunc = typeof(UnityEditor.EditorApplication).Assembly.GetType("UnityEditor.LayerMatrixGUI+SetValueFunc");
        }

        #endregion

        #region Init

        private void OnEnable() {
            parameters[0] = layerMatrix;
            parameters[2] = System.Delegate.CreateDelegate(getValueFunc, this, "InvertedIsIgnore");
            parameters[3] = System.Delegate.CreateDelegate(setValueFunc, this, "InvertedSetIgnore");

            layerNameOverrides = serializedObject.FindProperty("layerNameOverrides");
            collisonMask = serializedObject.FindProperty("collisonMask");
        }

        #endregion

        #region Invertion Fixes

        private bool InvertedIsIgnore(int l0, int l1) {
            var t = (PhysicsLayerSettings)target;
            return !t.IsIgnore(l0, l1);
        }

        private void InvertedSetIgnore(int l0, int l1, bool ignore) {
            var t = (PhysicsLayerSettings)target;
            t.SetIgnore(l0, l1, !ignore);
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                if(layerNameOverrides.arraySize != 32)
                    layerNameOverrides.arraySize = 32;
                if(collisonMask.arraySize != 32)
                    collisonMask.arraySize = 32;

                using(var s = new EditorGUILayout.ScrollViewScope(scroll, "box")) {
                    parameters[1] = true;
                    layerMatrixGUIDrawMethod.Invoke(null, parameters);
                    scroll = s.scrollPosition;
                }
            }
        }

        #endregion
    }
}
