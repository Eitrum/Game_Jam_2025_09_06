using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [System.Serializable]
    public class LabelData {
        #region Variables

        public string Name;
        public bool IgnoreInNaming;
        public string NamingOverride;
        public int Order = 0;
        public bool HideInFullList;
        public LabelsDrawer.LabelColor Color = LabelsDrawer.LabelColor.Gray;
        public List<int> Groups =  new List<int>();
        public List<int> Types = new List<int>();
        private int id;

        #endregion

        #region Properties

        public string NameFormatted {
            get {
                string toFormat = string.IsNullOrEmpty(NamingOverride) ? Name : NamingOverride;
                return IO.IOUtility.GetValidFileName(toFormat).Replace(' ', '-');
            }
        }

        public int Id {
            get {
                if(id == 0)
                    id = Name.GetHash32();
                return id;
            }
        }

        #endregion

        #region Constructor

        public LabelData() { }

        public LabelData(string name) {
            this.Name = name;
        }

        public LabelData(string name, int order) {
            this.Name = name;
            this.Order = order;
        }

        public LabelData(string name, int order, bool ignoreInNaming) {
            this.Name = name;
            this.Order = order;
            this.IgnoreInNaming = ignoreInNaming;
        }

        public LabelData(string name, string namingOverride, int order) {
            this.Name = name;
            this.NamingOverride = namingOverride;
            this.Order = order;
        }

        #endregion

        #region Add

        public LabelData AddGroup(string group) {
            return AddGroup(LabelGroupBindings.GetId(group));
        }

        public LabelData AddGroup(int groupid) {
            if(!Groups.Contains(groupid))
                Groups.Add(groupid);
            return this;
        }

        public LabelData AddType<T>() {
            if(LabelTypeBindings.TryGet<T>(out var binding))
                AddType(binding.TypeId);
            return this;
        }

        public LabelData AddType(int typeid) {
            if(!Types.Contains(typeid))
                Types.Add(typeid);
            return this;
        }

        #endregion

        #region Set

        public LabelData SetColor(LabelsDrawer.LabelColor color) {
            this.Color = color;
            return this;
        }

        public LabelData SetHidden(bool hideInFullList) {
            this.HideInFullList = hideInFullList;
            return this;
        }

        public LabelData SetIncludeInName(bool includeInNaming) {
            this.IgnoreInNaming = !includeInNaming;
            return this;
        }

        public LabelData SetOrder(int order) {
            this.Order = order;
            return this;
        }

        #endregion
    }
}
