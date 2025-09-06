using UnityEngine;

namespace Toolkit {
    public class EnumBindingsComponent : MonoBehaviour {

        [SerializeField] private EnumBaseBindings[] bindings = { };

        private void Awake() {
            foreach(var binding in bindings)
                binding?.Initialize();
        }
    }
}
