using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Auto Destroy")]
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private bool useRange = false;
        [SerializeField] private MinMax range = new MinMax(5f, 15f);

        private void Awake() {
            Destroy(gameObject, useRange ? range.Random : range.min);
        }
    }
}
