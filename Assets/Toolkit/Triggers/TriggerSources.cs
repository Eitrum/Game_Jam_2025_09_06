using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [System.Serializable]
    public class TriggerSources {
        #region Variables

        [SerializeField] private List<TriggerSource> sources = new List<TriggerSource>();

        #endregion

        #region Properties

        public IReadOnlyList<TriggerSource> Sources => sources;
        public int Count => sources.Count;

        public event OnTriggerDelegate OnTrigger{
            add {
                for(int i = sources.Count - 1; i >= 0; i--)
                    sources[i].OnTrigger += value;
            }
            remove {
                for(int i = sources.Count - 1; i >= 0; i--)
                    sources[i].OnTrigger -= value;
            }
        }

        public IEnumerable<Transform> TransformTargets {
            get {
                foreach(var s in sources)
                    switch(s.Source) {
                        case GameObject go: yield return go.transform; break;
                        case Transform t: yield return t; break;
                        case Component comp: yield return comp.transform; break;
                    }
            }
        }

        public TriggerSource this[int index] => sources[index];

        public bool IsAnyValid {
            get {
                if(sources.Count == 0)
                    return false;

                for(int i = sources.Count - 1; i >= 0; i--)
                    if(sources[i].IsValid)
                        return true;
                return false;
            }
        }

        public bool IsAllValid {
            get {
                if(sources.Count == 0)
                    return false;

                for(int i = sources.Count - 1; i >= 0; i--)
                    if(!sources[i].IsValid)
                        return false;

                return true;
            }
        }

        #endregion

        #region Constructor

        public TriggerSources() { }

        public TriggerSources(TriggerSource source) {
            this.sources.Add(source);
        }

        public TriggerSources(List<TriggerSource> sources) {
            this.sources = sources;
        }

        #endregion

        #region Add / Remove

        public void Add<T>(T source) where T : UnityEngine.Object {
            sources.Add(new TriggerSource(source));
        }

        public void Remove<T>(T source) where T : UnityEngine.Object {
            for(int i = sources.Count - 1; i >= 0; i--) {
                if(sources[i].Source == source) {
                    sources.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}
