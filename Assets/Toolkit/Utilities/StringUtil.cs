using System;
using System.Collections.Generic;
using System.Text;

namespace Toolkit {
    public static class StringUtil {
        #region Repeating Chars

        public class RepeatingCharCache {
            public const int MAX_REPEAT = 32;

            public string[] cache;

            public string this[int count] => Get(count);

            public RepeatingCharCache(char c) {
                cache = new string[MAX_REPEAT];
                string s = "";
                for(int i = 0; i < MAX_REPEAT; i++) {
                    s += c;
                    cache[i] = s;
                }
            }

            public string Get(int count) {
                if(count == 0)
                    return string.Empty;
                if(count > MAX_REPEAT)
                    return cache[MAX_REPEAT - 1];
                return cache[count - 1];
            }
        }

        private static Dictionary<char, RepeatingCharCache> repeatingCaches = new Dictionary<char, RepeatingCharCache>();

        public static RepeatingCharCache GetRepeatingCharCache(char c) {
            if(!repeatingCaches.TryGetValue(c, out var cache)) {
                cache = new RepeatingCharCache(c);
                repeatingCaches[c] = cache;
            }
            return cache;
        }

        public static string Get(char c, int count)
            => GetRepeatingCharCache(c).Get(count);

        public static string GetTabs(int count)
            => GetRepeatingCharCache('\t').Get(count);

        public static string GetSpaces(int count)
            => GetRepeatingCharCache(' ').Get(count);

        #endregion
    }
}
