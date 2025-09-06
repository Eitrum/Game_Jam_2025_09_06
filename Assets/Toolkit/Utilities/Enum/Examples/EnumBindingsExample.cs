using UnityEngine;

namespace Toolkit {
    public class EnumBindingsExample : MonoBehaviour {

        public Health.DamageTypeDnD damageType = Health.DamageTypeDnD.None;
        public string key;
        public string output;

        public void OnDrawGizmos() {
            if(FastEnum<Health.DamageTypeDnD>.TryGetData(damageType, key, out var obj)) {
                output = obj.ToString();
            }
        }
    }
}
