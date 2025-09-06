using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Threading
{
    public static class ThreadedUpdateSystem
    {
        private static ThreadedUpdate threadedUpdate;

        public static TLinkedListNode<IThreadedUpdate> Subscribe(IThreadedUpdate update, ThreadType type) {
            if(threadedUpdate == null)
                threadedUpdate = new ThreadedUpdate("Threaded Update System");
            return threadedUpdate.Subscribe(update);
        }

        public static bool Unsubcribe(IThreadedUpdate update) {
            if(threadedUpdate == null)
                return false;
            return threadedUpdate.Unsubscribe(update);
        }
    }
}
