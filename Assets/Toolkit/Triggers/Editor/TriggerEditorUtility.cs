using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger {
    public static class TriggerEditorUtility {
        public enum SearchForTriggerSources {
            Include,
            Exclude,
        }

        public enum FindType {
            Local,
            Parents,
        }

        #region Check For Trigger

        public static void CheckForTrigger<T>(T target, bool checkInParent = true) where T : MonoBehaviour {
            if(target == null) {
                EditorGUILayout.HelpBox($"Target is null!", MessageType.Error);
                return;
            }
            var trigger = checkInParent ? target.GetComponentInParent<ITrigger>() : target.GetComponent<ITrigger>();
            if(trigger == null) {
                EditorGUILayout.HelpBox("Missing a trigger in parents", MessageType.Error);
            }
        }

        public static void CheckForTrigger(UnityEngine.Object target, SearchForTriggerSources triggerSources = SearchForTriggerSources.Include, FindType findType = FindType.Parents) {
            if(target is Component comp) {
                var trigger = (findType == FindType.Parents) ? comp.GetComponentInParent<ITrigger>() : comp.GetComponent<ITrigger>();
                if(trigger == null && triggerSources == SearchForTriggerSources.Include) {
                    var fields = comp.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    foreach(var f in fields) {
                        try {
                            if(f.FieldType == typeof(TriggerSources)) {
                                var tsources = f.GetValue(target) as TriggerSources;
                                if(tsources != null && tsources.Count > 0)
                                    trigger = tsources[0].Trigger;
                                break;
                            }
                            else if(f.FieldType == typeof(TriggerSource)) {
                                var tsource = (TriggerSource)f.GetValue(target);
                                if(tsource.IsValid)
                                    trigger = tsource.Trigger;
                                break;
                            }
                        }
                        catch { }
                    }
                }
                if(trigger == null) {
                    EditorGUILayout.HelpBox("Missing a trigger in parents", MessageType.Error);
                }
                else {
                    EditorGUILayout.LabelField($"Trigger '{(trigger is UnityEngine.Object o ? o.name : "noname")}'", EditorStylesUtility.GrayItalicLabel);
                }
            }
        }



        public static void CheckForTriggerWithOptionalSources(UnityEngine.Object target, SerializedProperty optionalSourcesProperty, FindType findType = FindType.Parents) {
            if(target is Component comp) {
                var trigger = (findType == FindType.Parents) ? comp.GetComponentInParent<ITrigger>() : comp.GetComponent<ITrigger>();
                var sourcesArray = optionalSourcesProperty.FindPropertyRelative("sources");
                if(sourcesArray.arraySize > 0 || trigger == null) { // Check for sources
                    for(int i = sourcesArray.arraySize - 1; i >= 0; i--) {
                        var objRef = sourcesArray.GetArrayElementAtIndex(i).FindPropertyRelative("source").objectReferenceValue;
                        var triggerSource = new TriggerSource(objRef);
                        if(triggerSource.IsValid) {
                            trigger = triggerSource.Trigger;
                            break;
                        }
                    }
                }

                if(trigger == null) {
                    EditorGUILayout.HelpBox("Missing a trigger", MessageType.Error);
                }
                else {
                    EditorGUILayout.LabelField($"Trigger '{(trigger is UnityEngine.Object o ? o.name : "noname")}'", EditorStylesUtility.GrayItalicLabel);
                }
                if(sourcesArray.arraySize > 0 || trigger == null) {
                    EditorGUILayout.PropertyField(optionalSourcesProperty);
                }
            }
        }

        #endregion

        #region Is Circular Dependency

        public static bool IsCircularDependency(ITriggerRelay tr, int safetyDepth = 16) {
            HashSet<ITriggerRelay> currentlyChecking = new HashSet<ITriggerRelay>();
            return IsCircularDependency(tr, tr, currentlyChecking, safetyDepth);
        }

        private static bool IsCircularDependency(ITriggerRelay root, ITriggerRelay tr, HashSet<ITriggerRelay> stored, int safetyDepth) {
            if(safetyDepth < 0)
                return true;
            if(stored.Contains(tr))
                return true;
            stored.Add(tr);
            var trigger = tr as ITrigger;
            foreach(var t in tr.Parents) {
                if(t == trigger)
                    return true;
                if(t is ITriggerRelay ntr && IsCircularDependency(root, ntr, stored, safetyDepth - 1))
                    return true;

            }

            stored.Remove(tr);

            return false;
        }

        #endregion

        #region Editor Runtime Debug

        public static void DrawEditorDebug(Editor editor, UnityEngine.Object target) {
            if(Application.isPlaying) {
                EditorGUILayout.Space();
                using(new EditorGUILayout.VerticalScope("box")) {
                    var toeca = target as ITrigger;
                    EditorGUILayout.LabelField("DEBUG");
                    EditorGUILayout.LabelField("Has Triggered!", (toeca.HasTriggered ? "TRUE" : "FALSE"));
                    if(GUILayout.Button("Cause Trigger", GUILayout.Width(140f))) {
                        toeca.CauseTrigger(new Source(editor, SourceType.Editor));
                    }
                }
            }
        }

        #endregion
    }
}
