using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Toolkit.SpellCheck
{
    public static class SpellCheckEditor
    {
        #region Variables

        private static bool isInitialized = false;
        private static TextAsset file;
        private static Dictionary<int, List<string>> fastLookup = new Dictionary<int, List<string>>();

        private static System.Action onWordListLoaded;

        #endregion

        #region Properties

        public static event System.Action OnWordListLoaded {
            add {
                onWordListLoaded += value;
                if(isInitialized)
                    value?.Invoke();
            }
            remove => onWordListLoaded -= value;
        }

        #endregion

        #region Initialize

        public static void Initialize() {
            if(isInitialized)
                return;
            isInitialized = true;
            file = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Toolkit/Utilities/SpellCheck/words.txt");

            if(!file) {
                Debug.LogError($"Could not find the spell check file!");
                return;
            }

            UnityEditor.EditorUtility.DisplayProgressBar("Spell Check", "Preparing words", 0f);

            var words = file.text.Split('\n');

            for(int i = 0, length = words.Length; i < length; i++) {
                UnityEditor.EditorUtility.DisplayProgressBar("Spell Check", words[i], i / (float)length);
                var word = words[i].Trim();
                var hash = word.GetHash32();
                if(fastLookup.TryGetValue(hash, out List<string> arr))
                    arr.Add(word);
                else {
                    List<string> newArr = new List<string>();
                    newArr.Add(word);
                    fastLookup.Add(hash, newArr);
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log($"<color=cyan>[Spell Checker]</color> - Loaded {fastLookup.Count} hashes");
            onWordListLoaded?.Invoke();
        }

        [MenuItem("Toolkit/Spell Check/Reload")]
        public static void Reload() {
            fastLookup.Clear();
            isInitialized = false;
        }

        #endregion

        #region Custom Words

        public static void AddCustom(string word) {
            if(string.IsNullOrEmpty(word))
                return;
            Initialize();
            word = word.ToLower();
            var hash = word.GetHash32();

            if(fastLookup.TryGetValue(hash, out List<string> arr)) {
                if(!arr.Contains(word))
                    arr.Add(word);
            }
            else {
                List<string> newArr = new List<string>();
                newArr.Add(word);
                fastLookup.Add(hash, newArr);
            }
        }

        #endregion

        #region Check

        public static string[] CheckSentance(string sentance) {
            Initialize();
            sentance = sentance.Replace("-", " ");
            Regex reg = new Regex(@"<[^>]*>|\[[^>]*\]|[\d-]|[+%]");
            return reg.Replace(sentance, string.Empty).FindWords().Where(x => !string.IsNullOrEmpty(x) && !Check(x.ToLower())).ToArray();
        }

        public static bool Check(string word) {
            Initialize();
            if(fastLookup.TryGetValue(word.GetHash32(), out List<string> arr))
                return arr.Contains(word);

            return false;
        }

        #endregion
    }
}
