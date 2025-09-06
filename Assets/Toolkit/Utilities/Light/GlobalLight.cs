using UnityEngine;

namespace Toolkit
{
    [RequireComponent(typeof(Light))]
    public class GlobalLight : MonoBehaviour
    {
        private Light lightReference;

        private void Start() {
            lightReference = GetComponent<Light>();
            GlobalLightRules.LightAdded(lightReference);
        }

        private void OnDestroy() {
            GlobalLightRules.LightRemoved(lightReference);
        }
    }
}
