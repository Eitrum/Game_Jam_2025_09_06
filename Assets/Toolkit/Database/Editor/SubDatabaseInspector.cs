using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit.CodeGenerator;
using System.Linq;
using System.Text;
using System;

namespace Toolkit {
    [CustomEditor(typeof(SubDatabase), true)]
    public class SubDatabaseInspector : Editor {

        private UnityEngine.Object editorReference;

        public override void OnInspectorGUI() {
            var isEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.LabelField("Scripts", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(serializedObject.FindProperty("m_Script").objectReferenceValue, typeof(MonoScript), false);
            if(!editorReference)
                editorReference = AssetDatabaseUtility.Load<MonoScript>(AssetDatabase.FindAssets(typeof(SubDatabaseInspector).Name)[0]);
            EditorGUILayout.ObjectField(editorReference, typeof(MonoScript), false);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = isEnabled;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Verify Items", GUILayout.Width(90f))) {
                Populate(serializedObject);
            }
            if(GUILayout.Button("Export As Code", GUILayout.Width(120f))) {
                ExportToCode(target as SubDatabase);
            }

            if(GUILayout.Button("Add to database system", GUILayout.Width(120f))) {
                Debug.LogError("Not built yet stoopid seeb!");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Content", EditorStyles.boldLabel);
            base.OnInspectorGUI();

        }

        #region Population

        public static void Populate(SubDatabase database) {
            if(database == null) {
                return;
            }
            SerializedObject so = new SerializedObject(database);
            Populate(so);
        }

        public static void Populate(SerializedObject so) {
            if(!(so.targetObject is SubDatabase)) {
                Debug.LogError("Trying to populate a non-subDatabase object");
                return;
            }
            var path = AssetDatabase.GetAssetPath(so.targetObject).Replace("/" + so.targetObject.name + ".asset", "");
            var assets = AssetDatabaseUtility.LoadAssets<Item>(path);
            Sort.Merge(assets, (a, b) => {
                return a.order.CompareTo(b.order);
            });
            assets = assets.Where(x => AssetDatabase.IsMainAsset(x)).Unique().ToArray();
            var items = so.FindProperty("items");
            items.ClearArray();
            for(int i = 0, length = assets.Length; i < length; i++) {
                var asset = assets[i];
                items.InsertArrayElementAtIndex(i);
                items.GetArrayElementAtIndex(i).objectReferenceValue = asset;
            }

            if(so.hasModifiedProperties) {
                so.ApplyModifiedProperties();
            }
            DatabaseVerification.LogDuplicatedAssets(DatabaseVerification.VerifyDuplicatedAssets(so.targetObject as SubDatabase));
        }

        #endregion

        #region Code Export

        public static void ExportToCode(SubDatabase subDatabase) {
            var name = subDatabase.Name;
            var @namespace = subDatabase.Namespace;
            CodeFile file = new CodeFile(name);
            file.UseCleanProcess = true;
            file.AddUsing(typeof(Database));
            CodeClass @class = null;
            if(!string.IsNullOrEmpty(@namespace)) {
                @class = file.AddNamespace(@namespace).AddClass(AccessModifier.PublicStatic, name);
            }
            else
                @class = file.AddClass(AccessModifier.PublicStatic, name);

            var type = subDatabase.GetType();
            @class.AddProperty(new CodeProperty(AccessModifier.PublicStatic, typeof(SubDatabase), "Reference", $"return Database.GetSubDatabaseById<{type.FullName}>({subDatabase.Id});"));
            Node node = new Node(subDatabase.All);
            node.Generate(@class);

            var result = file.CreateFile(System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject)));
            AssetDatabase.ImportAsset(result, ImportAssetOptions.ForceUpdate);
        }

        public class Node {
            #region Variables

            public string nodeName = "";
            public List<Item> items = new List<Item>();
            public Dictionary<string, Node> nodes = new Dictionary<string, Node>();

            #endregion

            #region Constructor

            public Node() {

            }

            public Node(string name) {
                nodeName = name;
            }

            public Node(Item[] items) {
                for(int i = 0, length = items.Length; i < length; i++) {
                    var item = items[i];
                    if(string.IsNullOrEmpty(item.editorPath)) {
                        this.items.Add(item);
                        continue;
                    }

                    var split = item.editorPath.Split('.', '/', '\\');
                    Node node = this;
                    for(int s = 0; s < split.Length; s++) {
                        node = node.GetNode(split[s]);
                    }
                    node.AddItem(item);
                }
            }

            #endregion

            #region Get and Add

            public void AddItem(Item item) {
                items.Add(item);
            }

            public Node GetNode(string node) {
                if(nodes.ContainsKey(node)) {
                    return nodes[node];
                }
                var n = new Node(node);
                nodes.Add(node, n);
                return n;
            }

            #endregion

            #region Generate

            public void Generate(CodeClass @class) {
                for(int i = 0, length = items.Count; i < length; i++) {
                    var item = items[i];
                    var type = item.GetType();
                    @class.AddProperty(new CodeProperty(AccessModifier.PublicStatic,
                        type.FullName, item.ItemName.Replace(" ", "_"), $"Database.GetItemById<{type.FullName}>({item.Id});"));
                }
                foreach(var node in nodes.Values) {
                    var subclass = @class.AddClass(AccessModifier.PublicStatic, node.nodeName.Replace(" ", "_"));
                    node.Generate(subclass);
                }
            }

            #endregion
        }

        #endregion

        #region Sorting // HAS TO BE REPLACED

        public static class Sort<T> {

            static T[] subArray;

            #region Merge Sort

            public static void Merge(IList<T> array, Comparison<T> comparison) {
                subArray = new T[array.Count];
                InternalMerge(array, comparison, 0, array.Count);
            }

            private static void InternalMerge(IList<T> array, Comparison<T> comparison, int from, int to) {
                var diff = to - from;
                if(diff < 2) {
                    //Sorted
                    return;
                }
                var mid = from + diff / 2;
                if(diff == 2 && comparison(array[from], array[mid]) == 1) {
                    // Simple sort
                    var temp = array[from];
                    array[from] = array[mid];
                    array[mid] = temp;
                    return;
                }

                InternalMerge(array, comparison, from, mid);
                InternalMerge(array, comparison, mid, to);

                var index0 = from;
                var index1 = mid;

                for(int i = from; i < to; i++) {
                    var res = comparison(array[index0], array[index1]);
                    if(res == -1) {
                        subArray[i] = array[index0++];
                        if(index0 == index1)
                            break;
                    }
                    else if(res == 1) {
                        subArray[i] = array[index1++];
                        if(index1 >= to) {
                            while(++i < to) {
                                subArray[i] = array[index0++];
                            }
                            break;
                        }
                    }
                    else {
                        subArray[i] = array[index0++];
                        if(index0 == index1)
                            break;
                    }
                }

                for(int i = from; i < index1; i++) {
                    array[i] = subArray[i];
                }

            }

            #endregion

        }

        #endregion
    }
}
