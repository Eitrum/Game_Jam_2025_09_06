using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [System.Serializable]
    public class LabelTypeBinding {
        public string Assembly;
        public string TypeName;
        public int TypeId;

        private Type type;
        public Type Type {
            get {
                if(type == null) {
                    var assembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.Equals(Assembly));
                    if(assembly == null) {
                        Debug.LogError(this.FormatLog("Could not find assembly: " + Assembly));
                        return null;
                    }
                    type = assembly.GetType(TypeName);
                    if(type == null) {
                        Debug.LogError(this.FormatLog($"Could not find type '{TypeName}' inside assembly: {Assembly}"));
                    }
                }
                return type;
            }
        }

        public LabelTypeBinding(Type type) {
            Assembly = type.Assembly.GetName().Name;
            TypeName = type.FullName;
            this.type = type;
            this.TypeId = TypeName.GetHash32();
        }

        public void DrawLayout() => LabelTypeBindings.DrawLayout(this);
    }
}
