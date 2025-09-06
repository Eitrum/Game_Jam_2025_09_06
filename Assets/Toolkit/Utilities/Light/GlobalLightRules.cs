using UnityEngine;

namespace Toolkit
{
    public static class GlobalLightRules // TODO: Make editor only, less runtime intialize cost
    {
        private static OnLightAddedDelegate onLightAdded;
        private static OnLightRemovedDelegate onLightRemoved;

        public static event OnLightAddedDelegate OnLightAdded {
            add => onLightAdded += value;
            remove => onLightAdded -= value;
        }
        public static event OnLightRemovedDelegate OnLightRemoved {
            add => onLightRemoved += value;
            remove => onLightRemoved -= value;
        }

        internal static void LightAdded(Light light) => onLightAdded?.Invoke(light);
        internal static void LightRemoved(Light light) => onLightRemoved?.Invoke(light);

        public delegate void OnLightAddedDelegate(Light light);
        public delegate void OnLightRemovedDelegate(Light light);
    }
}
