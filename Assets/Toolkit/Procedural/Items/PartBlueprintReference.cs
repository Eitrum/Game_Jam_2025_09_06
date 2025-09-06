using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    public class PartBlueprintReference : MonoBehaviour
    {
        private PartAssembly assembly = null;
        private PartBlueprint blueprint = null;

        public PartBlueprint Blueprint => blueprint;
        public PartAssembly Assembly => assembly;

        public void Assign(PartBlueprint blueprint, PartAssembly assembly) {
            this.blueprint = blueprint;
            this.assembly = assembly;
        }

        public void Print() {
            Debug.Log(blueprint.ToString(assembly));
        }
    }
}
