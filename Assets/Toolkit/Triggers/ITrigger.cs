using System.Collections;
using System.Collections.Generic;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.Trigger {
    public interface ITrigger {
        bool HasTriggered { get; }
        void CauseTrigger(Source source);
        event OnTriggerDelegate OnTrigger;
    }

    public static class TriggerUtil {
        public static void CauseTrigger<T>(this T trigger) where T : ITrigger {
            using(var s = Source.Create(trigger))
                trigger?.CauseTrigger(s);
        }
    }

    // Utility to check for circular dependencies
    public interface ITriggerRelay {
        IEnumerable<ITrigger> Parents { get; }
    }

    public delegate void OnTriggerDelegate(Source source);
}
