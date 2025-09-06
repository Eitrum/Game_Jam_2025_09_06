using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public static class TypeOfUtil {

        private class Cache<T> {
            public static readonly int Id = typeof(T).FullName.GetHash32();
        }

        public static int GetId<T>() => Cache<T>.Id;
    }
}
