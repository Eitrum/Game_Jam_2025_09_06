using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Utility/Destroy In Build")]
    public class DestroyInBuild : MonoBehaviour
    {
#if !UNITY_EDITOR
        private void Awake() {
            Destroy(this.gameObject);
    }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(DestroyInBuild))]
    public class DestroyInBuildEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
        }
    }
#endif
}
