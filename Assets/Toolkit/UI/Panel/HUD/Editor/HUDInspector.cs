using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Toolkit.Unit;

namespace Toolkit.UI.PanelSystem {
    [CustomEditor(typeof(HUD))]
    public class HUDInspector : Editor {


        public override void OnInspectorGUI() {
            using(var editor = new ToolkitEditorUtility.InspectorScope(this)) {
                editor.DrawAll();
            }

            if(!Application.isPlaying)
                return;

            using(new EditorGUILayout.VerticalScope("box")) {
                var hud = (HUD)target;
                var unitAsObject = hud.Unit as UnityEngine.Object;
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                var newObject = EditorGUILayout.ObjectField("Unit", unitAsObject, typeof(UnityEngine.Object), true);
                if(EditorGUI.EndChangeCheck()) {
                    IUnit newUnit = null;
                    switch(newObject) {
                        case IUnit tUnit:
                            newUnit = tUnit;
                            break;
                        case GameObject go:
                            newUnit = go.GetComponent<IUnit>();
                            break;
                        case Transform tr:
                            newUnit = tr.GetComponent<IUnit>();
                            break;
                        case Component comp:
                            newUnit = comp.GetComponent<IUnit>();
                            break;
                    }

                    if(newUnit != null)
                        hud.Assign(newUnit);
                }
            }
        }
    }
}
