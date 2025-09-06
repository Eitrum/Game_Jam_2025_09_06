using System.Collections;

namespace Toolkit.SceneManagement {
    internal class SceneLoader : MonoSingleton<SceneLoader> {
        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

        public void Load(IEnumerator enumerator) {
            StartCoroutine(enumerator);
        }
    }
}
