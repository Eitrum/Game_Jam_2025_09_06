using System;

namespace Toolkit.Unit {
    [System.AttributeUsage(System.AttributeTargets.Interface | System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class IUnitSupportAttribute : System.Attribute {
        public string Name;
        public Type DefaultImplementation;
        public IUnitSupportAttribute() { }
        public IUnitSupportAttribute(string name) => this.Name = name;
        public IUnitSupportAttribute(string name, Type defaultImplementation) {
            this.Name = name;
            this.DefaultImplementation = defaultImplementation;
        }
    }
}
