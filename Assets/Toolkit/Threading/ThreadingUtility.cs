using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Toolkit.Threading {
    public static class ThreadingUtility {
        #region Main Thread

#if UNITY_EDITIOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeMainThreadReference() => mainThread = Thread.CurrentThread;

        private static Thread mainThread;
        public static bool IsMainThread => Thread.CurrentThread == mainThread;

        #endregion

        #region Processors

        private static int processorCount = 0;
        public static int ProcessorCount {
            get {
                if(processorCount == 0)
                    processorCount = System.Environment.ProcessorCount;
                return processorCount;
            }
        }

        #endregion
    }
}
