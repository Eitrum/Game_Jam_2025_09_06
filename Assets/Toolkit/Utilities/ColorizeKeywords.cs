using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

namespace Toolkit.Utility {
    [CreateAssetMenu(menuName = "Toolkit/Utility/Colorize Keywords")]
    public class ColorizeKeywords : ScriptableObject {
        #region Variables

        private const string TAG = "[Toolkit.Utility.ColorizeKeywords] - ";

        [SerializeField] private KeywordColorPair[] keywords = null;

        private static Dictionary<string, Color> cachedWords = new Dictionary<string, Color>(System.StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Init

        [RuntimeInitializeOnLoadMethod]
        private static void Init() {
            var defaultKeywords = Resources.Load<ColorizeKeywords>("ColorizeKeywords");
            if(defaultKeywords)
                Register(defaultKeywords);
        }

        #endregion

        #region Register

        public static void Register(ColorizeKeywords config) {
            foreach(var c in config.keywords) {
                if(!cachedWords.TryAdd(c.Keyword, c.Color)) {
                    Debug.LogError(TAG + "Keyword already exist: " + c.Keyword);
                }
            }
        }

        public static bool Register(string keyword, Color color)
            => cachedWords.TryAdd(keyword, color);

        #endregion

        #region Colorize

        public static string Colorize(string text) {
            StringBuilder sb = new StringBuilder();
            var span = text.AsSpan();
            int start = 0;
            bool isWord = false;
            int length = text.Length;
            for(int i = 0; i < length; i++) {
                var ischar = Char.IsLetter(span[i]);
                if(ischar && !isWord) {
                    start = i;
                    isWord = true;
                }
                else if(!ischar) {
                    if(isWord) {
                        isWord = false;
                        var wordlen = (i - start);
                        if(cachedWords.TryGetValue(span.Slice(start, wordlen).ToString(), out Color c)) {
                            sb.Append($"<color=#{c.ToHex32()}>{span.Slice(start, wordlen).ToString()}</color>");
                        }
                        else
                            sb.Append(span.Slice(start, wordlen));
                    }
                    sb.Append(span[i]);
                }
            }

            return sb.ToString();
        }

        #endregion

        [System.Serializable]
        public class KeywordColorPair {
            #region Variables

            [SerializeField] private string keyword;
            [SerializeField] private Color color;

            #endregion

            #region Properties

            public string Keyword => keyword;
            public Color Color => color;

            #endregion
        }
    }
}
