using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.LoadingScreen {
    [CreateAssetMenu(menuName = "Toolkit/UI/Tips Table")]
    public class TipsTable : ScriptableObject, ISerializationCallbackReceiver {
        #region Variables

        [SerializeField] private string tableId;
        [SerializeField] private string[] tips = { };

        private int cachedIndex = 0;
        private string cachedPPKey;

        #endregion

        #region Properties

        public string TableId => string.IsNullOrEmpty(tableId) ? name : tableId;
        public IReadOnlyList<string> Tips => tips;

        public int Index => cachedIndex;
        public int Length => tips.Length;

        #endregion

        #region Init

        void OnEnable() {
            cachedIndex = PlayerPrefs.HasKey(cachedPPKey) ? Mathf.Clamp(PlayerPrefs.GetInt(cachedPPKey), 0, tips.Length) : -1;
        }

        #endregion

        #region Next / Prev

        public bool Next(out string tip) {
            if(this.tips == null || this.tips.Length == 0) {
                tip = string.Empty;
                return false;
            }

            cachedIndex = (cachedIndex + 1) % tips.Length;
            tip = tips[cachedIndex];

            PlayerPrefs.SetInt(cachedPPKey, cachedIndex);
            PlayerPrefs.Save();
            return true;
        }

        public bool Previous(out string tip) {
            if(this.tips == null || this.tips.Length == 0) {
                tip = string.Empty;
                return false;
            }

            cachedIndex = (cachedIndex - 1);
            if(cachedIndex < 0)
                cachedIndex = tips.Length - 1;

            tip = this.tips[cachedIndex];
            PlayerPrefs.SetInt(cachedPPKey, cachedIndex);
            PlayerPrefs.Save();
            return true;
        }

        #endregion

        #region ISerialization Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            cachedPPKey = "tipstable:" + TableId;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion
    }
}
