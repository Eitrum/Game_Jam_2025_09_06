using System;
using System.Collections.Generic;

namespace Toolkit
{
    public static class ForEnumerable
    {
        public static IEnumerable<int> CreateEnumerator(this int length) {
            for(int i = 0; i < length; i++) {
                yield return i;
            }
        }

        public static IEnumerable<int> Run(int length) {
            for(int i = 0; i < length; i++) {
                yield return i;
            }
        }

        public static IEnumerable<int> RunBetween(int start, int end) {
            for(int i = start; i < end; i++) {
                yield return i;
            }
        }
    }
}
