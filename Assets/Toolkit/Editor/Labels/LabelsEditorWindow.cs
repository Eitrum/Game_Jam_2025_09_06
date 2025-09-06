using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit {
    public class LabelsEditorWindow : SimpleEditorWindow {

        public enum ViewMode {
            All,
            ByGroups,
            ByTypes,
        }

        #region Variables

        [System.NonSerialized] private static string[] tabs = new string[]{ "Apply", "Labels", "Types", "Groups" };
        [System.NonSerialized] private int selectedTab = 0;
        [System.NonSerialized] private ViewMode mode;
        [System.NonSerialized] private LabelData selected;
        [System.NonSerialized] private bool allowAdd = false;
        [System.NonSerialized] private Vector2 scroll;
        [System.NonSerialized] private Vector2[] scrolls = new Vector2[64];
        [System.NonSerialized] private string searchText;
        [System.NonSerialized] private List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();

        [System.NonSerialized] private UnityEditorInternal.ReorderableList labelDataTypesArray = null;
        [System.NonSerialized] private UnityEditorInternal.ReorderableList labelDataGroupsArray = null;

        [System.NonSerialized] private UnityEditorInternal.ReorderableList labelTypesArray = null;
        [System.NonSerialized] private UnityEditorInternal.ReorderableList labelGroupsArray = null;

        #endregion

        #region Init

        private void OnEnable() {
            labelDataTypesArray = new UnityEditorInternal.ReorderableList(null, typeof(int));
            labelDataGroupsArray = new UnityEditorInternal.ReorderableList(null, typeof(int));

            labelDataTypesArray.headerHeight = 0;
            labelDataTypesArray.drawElementCallback += DrawLabelDataType;
            labelDataGroupsArray.headerHeight = 0;
            labelDataGroupsArray.drawElementCallback += DrawLabelGroupType;


            labelTypesArray = new UnityEditorInternal.ReorderableList(null, typeof(LabelTypeBinding));
            labelGroupsArray = new UnityEditorInternal.ReorderableList(null, typeof(string));

            labelTypesArray.headerHeight = 0;
            labelTypesArray.displayAdd = false;
            labelTypesArray.displayRemove = false;
            labelTypesArray.drawElementCallback += DrawLabelType;
            labelTypesArray.onReorderCallback += OnLabelTypeReorder;
            labelGroupsArray.headerHeight = 0;
            labelGroupsArray.displayAdd = false;
            labelGroupsArray.displayRemove = false;
            labelGroupsArray.drawElementCallback += DrawLabelGroup;
            labelGroupsArray.onReorderCallback += OnGroupReorder;


            autoRepaintOnSceneChange = true;

            Selection.selectionChanged -= OnSelectUpdate;
            Selection.selectionChanged += OnSelectUpdate;
        }

        private void OnSelectUpdate() {
            selectedObjects.Clear();
            selectedObjects.AddRange(Selection.objects);
        }

        private void OnLabelTypeReorder(ReorderableList list) {
            LabelTypeBindings.Save();
        }

        private void OnGroupReorder(ReorderableList list) {
            LabelGroupBindings.Save();
        }

        #endregion

        #region List Drawers

        private void DrawLabelDataType(Rect rect, int index, bool isActive, bool isFocused) {
            var list = labelDataTypesArray.list as List<int>;
            var value = list[index];
            int typeId = 0;
            if(LabelTypeBindings.TryGet(value, out var binding))
                typeId = binding.TypeId;
            list[index] = LabelTypeBindings.DrawDropdown(rect.Pad(0, 0, 3, 2), typeId);
        }

        private void DrawLabelGroupType(Rect rect, int index, bool isActive, bool isFocused) {
            var list = labelDataGroupsArray.list as List<int>;
            var value = list[index];
            int typeId = 0;
            if(LabelGroupBindings.TryGet(value, out var binding))
                typeId = binding.GetHash32();
            list[index] = LabelGroupBindings.DrawDropdown(rect.Pad(0, 0, 3, 2), typeId);
        }

        private void DrawLabelType(Rect rect, int index, bool isActive, bool isFocused) {
            var list = labelTypesArray.list as List<LabelTypeBinding>;
            LabelTypeBindings.Draw(rect, list[index]);
        }

        private void DrawLabelGroup(Rect rect, int index, bool isActive, bool isFocused) {
            var list = labelGroupsArray.list as List<string>;
            LabelGroupBindings.Draw(rect, list[index]);
        }

        #endregion

        #region Draw Base

        protected override void DrawLayout() {
            selectedTab = GUILayout.SelectionGrid(selectedTab, tabs, tabs.Length);
            switch(selectedTab) {
                case 0: DrawApply(); break;
                case 1: DrawLabels(); break;
                case 2: DrawTypes(); break;
                case 3: DrawGroups(); break;
            }
        }

        #endregion

        #region Draw Apply Tab

        private void DrawApply() {
            foreach(var obj in selectedObjects) {
                using(new EditorGUILayout.HorizontalScope()) {
                    using(new EditorGUI.DisabledScope(true)) {
                        EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), false, GUILayout.Width(200));
                    }
                    var labels = AssetDatabase.GetLabels(obj);
                    foreach(var l in labels) {
                        if(LabelsDrawer.ButtonLayout(l, LabelsDrawer.LabelColor.Blue)) {
                            var str = l;
                            EditorApplication.delayCall += () => {
                                AssetDatabase.SetLabels(obj, labels.Skip(str).ToArray());
                            };
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                if(LabelsDatabase.TryGetLabelsSorted(obj, out var labelsSorted))
                    EditorGUILayout.LabelField($"{labelsSorted.Select(x => x.NameFormatted).Join("_").Replace(' ', '_')}");
            }
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Recently Used", EditorStyles.boldLabel);
            using(var vscope = new EditorGUILayout.VerticalScope("box", GUILayout.MinHeight(60))) {
                var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 60);
                LabelsDrawer.Grid(gridArea, LabelsDatabase.LastUsedLabels, ApplyToSelected, ref scrolls[scrolls.Length - 1]);

            }
            EditorGUILayout.Space(4);
            searchText = EditorGUILayout.TextField("Search:", searchText);
            mode = (ViewMode)EditorGUILayout.EnumPopup("View", mode);
            int scrollIndex = 0;

            switch(mode) {
                case ViewMode.All: {
                        EditorGUILayout.LabelField("All labels", EditorStyles.boldLabel);
                        using(var vscope = new EditorGUILayout.VerticalScope("box")) {
                            var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                            if(string.IsNullOrEmpty(searchText)) {
                                LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => !x.HideInFullList), ApplyToSelected, ref scroll);
                            }
                            else {
                                LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels, ApplyToSelected, ref scroll, (LabelData s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                            }
                        }
                    }
                    break;
                case ViewMode.ByGroups: {
                        using(var sscope = new EditorGUILayout.ScrollViewScope(scroll)) {
                            scroll = sscope.scrollPosition;
                            foreach(var group in LabelGroupBindings.Bindings) {
                                var groupid = group.GetHash32();
                                EditorGUILayout.LabelField(group, EditorStyles.boldLabel);
                                using(var vscope = new EditorGUILayout.VerticalScope("box", GUILayout.MinHeight(60))) {
                                    var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                    if(string.IsNullOrEmpty(searchText)) {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Contains(groupid)), ApplyToSelected, ref scrolls[scrollIndex++]);
                                    }
                                    else {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Contains(groupid)), ApplyToSelected, ref scrolls[scrollIndex++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                    }
                                }
                            }
                            EditorGUILayout.LabelField("No Group", EditorStyles.boldLabel);
                            using(var vscope = new EditorGUILayout.VerticalScope("box", GUILayout.MinHeight(60))) {
                                var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                if(string.IsNullOrEmpty(searchText)) {
                                    LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Count == 0), ApplyToSelected, ref scrolls[scrollIndex++]);
                                }
                                else {
                                    LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Count == 0), ApplyToSelected, ref scrolls[scrollIndex++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                }
                            }
                        }
                    }
                    break;
                case ViewMode.ByTypes: {
                        using(var sscope = new EditorGUILayout.ScrollViewScope(scroll)) {
                            scroll = sscope.scrollPosition;
                            foreach(var typeBinding in LabelTypeBindings.Bindings) {
                                bool toDraw = false;
                                foreach(var so in selectedObjects) {
                                    if(so.GetType() == typeBinding.Type || so.GetType().IsSubclassOf(typeBinding.Type) || typeBinding.Type.IsSubclassOf(so.GetType())) {
                                        toDraw = true;
                                        break;
                                    }
                                }
                                if(!toDraw)
                                    continue;

                                var id = typeBinding.TypeId;
                                EditorGUILayout.LabelField(typeBinding.TypeName, EditorStyles.boldLabel);
                                using(var vscope = new EditorGUILayout.VerticalScope("box", GUILayout.MinHeight(60))) {
                                    var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                    if(string.IsNullOrEmpty(searchText)) {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Contains(id)), ApplyToSelected, ref scrolls[scrollIndex++]);
                                    }
                                    else {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Contains(id)), ApplyToSelected, ref scrolls[scrollIndex++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                    }
                                }
                            }
                            EditorGUILayout.LabelField("No Type", EditorStyles.boldLabel);
                            using(var vscope = new EditorGUILayout.VerticalScope("box", GUILayout.MinHeight(60))) {
                                var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                if(string.IsNullOrEmpty(searchText)) {
                                    LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Count == 0), ApplyToSelected, ref scrolls[scrollIndex++]);
                                }
                                else {
                                    LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Count == 0), ApplyToSelected, ref scrolls[scrollIndex++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                }
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Draw Group Tab

        private void DrawGroups() {
            using(new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space(8);

            labelGroupsArray.list = LabelGroupBindings.Bindings as IList;
            labelGroupsArray.DoLayoutList();

            EditorGUI.BeginChangeCheck();
            var toAdd = EditorGUILayout.DelayedTextField("Add", string.Empty);
            if(EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(toAdd)) {
                LabelGroupBindings.Add(toAdd);
            }
        }

        #endregion

        #region Draw Types Tab

        private void DrawTypes() {
            using(new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space(8);

            labelTypesArray.list = LabelTypeBindings.Bindings as IList;
            labelTypesArray.DoLayoutList();

            EditorGUI.BeginChangeCheck();
            var newObj = EditorGUILayout.ObjectField(null, typeof(UnityEngine.Object), false);
            if(EditorGUI.EndChangeCheck()) {
                if(newObj != null)
                    LabelTypeBindings.Add(newObj.GetType());
            }
        }

        #endregion

        #region Draw Labels Tab

        private void DrawLabels() {
            using(new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space(8);
            using(new EditorGUILayout.HorizontalScope()) {
                searchText = EditorGUILayout.TextField("Search:", searchText);
                using(new EditorGUI.DisabledScope(!allowAdd)) {
                    if(GUILayout.Button("Add", GUILayout.Width(50))) {
                        LabelsDatabase.Add(searchText);
                        SelectLabel(searchText);
                    }
                }
            }

            mode = (ViewMode)EditorGUILayout.EnumPopup("View", mode);
            int index = 0;

            using(new EditorGUILayout.HorizontalScope()) {
                using(new EditorGUILayout.VerticalScope()) {
                    switch(mode) {
                        case ViewMode.All: {
                                EditorGUILayout.LabelField("All labels", EditorStyles.boldLabel);
                                using(var vscope = new EditorGUILayout.VerticalScope("box")) {
                                    var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                    if(string.IsNullOrEmpty(searchText)) {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => !x.HideInFullList), SelectLabel, ref scrolls[index++]);
                                    }
                                    else {
                                        var count = LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels, SelectLabel, ref scrolls[index++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                        allowAdd = count == 0;
                                    }
                                }
                            }
                            break;
                        case ViewMode.ByGroups: {
                                bool anyFound = false;
                                foreach(var group in LabelGroupBindings.Bindings) {
                                    var groupid = group.GetHash32();
                                    EditorGUILayout.LabelField(group, EditorStyles.boldLabel);
                                    using(var vscope = new EditorGUILayout.VerticalScope("box")) {
                                        var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                        if(string.IsNullOrEmpty(searchText)) {
                                            LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Contains(groupid)), SelectLabel, ref scrolls[index++]);
                                        }
                                        else {
                                            var count = LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Contains(groupid)), SelectLabel, ref scrolls[index++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                            anyFound = count > 0;
                                        }
                                    }
                                }
                                EditorGUILayout.LabelField("Groupless", EditorStyles.boldLabel);
                                using(var vscope = new EditorGUILayout.VerticalScope("box")) {
                                    var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                    if(string.IsNullOrEmpty(searchText)) {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Count == 0), SelectLabel, ref scrolls[index++]);
                                        anyFound = true;
                                    }
                                    else {
                                        var count = LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Groups.Count == 0), SelectLabel, ref scrolls[index++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                        anyFound = count > 0;
                                    }
                                }
                                allowAdd = !anyFound;
                            }
                            break;
                        case ViewMode.ByTypes: {
                                bool anyFound = false;
                                foreach(var typeBinding in LabelTypeBindings.Bindings) {
                                    var id = typeBinding.TypeId;
                                    EditorGUILayout.LabelField(typeBinding.TypeName, EditorStyles.boldLabel);
                                    using(var vscope = new EditorGUILayout.VerticalScope("box")) {
                                        var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                        if(string.IsNullOrEmpty(searchText)) {
                                            LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Contains(id)), SelectLabel, ref scrolls[index++]);
                                            anyFound = true;
                                        }
                                        else {
                                            var count = LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Contains(id)), SelectLabel, ref scrolls[index++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                            anyFound = count > 0;
                                        }
                                    }
                                }
                                EditorGUILayout.LabelField("Typeless", EditorStyles.boldLabel);
                                using(var vscope = new EditorGUILayout.VerticalScope("box")) {
                                    var gridArea = GUILayoutUtility.GetRect(1, 9999, 20, 9999);
                                    if(string.IsNullOrEmpty(searchText)) {
                                        LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Count == 0), SelectLabel, ref scrolls[index++]);
                                        anyFound = true;
                                    }
                                    else {
                                        var count = LabelsDrawer.Grid(gridArea, LabelsDatabase.AllLabels.Where(x => x.Types.Count == 0), SelectLabel, ref scrolls[index++], (s) => s.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
                                        anyFound = count > 0;
                                    }
                                }
                                allowAdd = !anyFound;
                            }
                            break;
                    }
                }
                EditorGUILayout.Space(6f);
                using(new EditorGUILayout.VerticalScope("box", GUILayout.Width(200))) {
                    if(selected != null) {
                        Draw(selected);
                    }
                }
            }
        }

        private void Draw(LabelData data) {
            EditorGUI.BeginChangeCheck();
            var oldw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;

            EditorGUILayout.LabelField(data.Name, EditorStyles.boldLabel);
            data.Order = EditorGUILayout.IntField("Order", data.Order);
            data.IgnoreInNaming = EditorGUILayout.Toggle("Ignore In Naming", data.IgnoreInNaming);
            if(!data.IgnoreInNaming)
                data.NamingOverride = EditorGUILayout.TextField("Naming Override", data.NamingOverride);
            data.Color = (LabelsDrawer.LabelColor)EditorGUILayout.EnumPopup("Color", data.Color);
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Groups", EditorStylesUtility.BoldLabel);
            using(new EditorGUI.IndentLevelScope(1)) {
                labelDataGroupsArray.list = data.Groups;
                labelDataGroupsArray.DoLayoutList();
            }
            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField("Types", EditorStylesUtility.BoldLabel);
            using(new EditorGUI.IndentLevelScope(1)) {
                labelDataTypesArray.list = data.Types;
                labelDataTypesArray.DoLayoutList();
            }
            EditorGUIUtility.labelWidth = oldw;
            if(EditorGUI.EndChangeCheck()) {
                LabelsDatabase.Save();
            }
        }

        #endregion

        #region Apply & Select 

        private void ApplyToSelected(LabelData data) {
            EditorApplication.delayCall += () => {
                LabelsDatabase.MarkUsed(data);
                var str = data.Name;
                var selectedObjects = Selection.objects;
                foreach(var obj in selectedObjects) {
                    var labels = AssetDatabase.GetLabels(obj);
                    if(labels.Contains(str))
                        continue;
                    Array.Resize(ref labels, labels.Length + 1);
                    labels[labels.Length - 1] = str;
                    AssetDatabase.SetLabels(obj, labels);
                }
            };
        }

        private void SelectLabel(LabelData data) {
            selected = data;
        }

        private void SelectLabel(string label) {
            if(LabelsDatabase.TryGet(label, out selected)) {
                //LabelsDatabase.MarkUsed(selected);
            }
        }

        #endregion
    }
}
