using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Disable At Start")]
    public class DisableAtStart : MonoBehaviour
    {
        void Start() {
            gameObject.SetActive(false);
        }
    }
}
