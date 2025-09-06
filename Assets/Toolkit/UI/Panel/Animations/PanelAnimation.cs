using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;
using System.Linq;

namespace Toolkit.UI.PanelSystem.Animations {
    public class PanelAnimation : MonoBehaviour, IPanelAnimation {

        public enum State {
            None,
            Showing,
            Hiding,
            Finishing,
            Complete,
        }

        #region Variables

        [SerializeField] private bool playSequenceInReverseOnHide = true;
        [SerializeField] private SequenceItem[] show = { new SequenceItem() };
        [SerializeField] private SequenceItem[] hide = { new SequenceItem() };

        private List<IPanelAnimationObject> animations = new List<IPanelAnimationObject>();

        private State state = State.None;

        private SequenceItem currentSequenceItem;
        private int sequenceIndex = -1;
        private float durationTimer = 0f;

        #endregion

        #region Properties

        public bool IsPlaying => animations.Count > 0;
        public bool IsComplete => state == State.Complete;
        public IReadOnlyList<SequenceItem> ShowSequence => show;
        public IReadOnlyList<SequenceItem> HideSequence => hide;

        #endregion

        #region Init

        void Awake() {
            if(playSequenceInReverseOnHide)
                hide = show.Reverse().ToArray();
        }

        void OnDisable()
            => Cancel();

        #endregion

        #region Update

        void LateUpdate() {
            var dt = Time.deltaTime;

            for(int i = animations.Count - 1; i >= 0; i--) {
                animations[i]?.Update(dt);
                if(animations[i].IsComplete)
                    animations.ReplaceWithLastAndRemove(i);
            }

            switch(state) {
                case State.Finishing:
                    if(animations.Count == 0) {
                        state = State.Complete;
                    }
                    break;
                case State.Showing:
                case State.Hiding: {
                        var mode = currentSequenceItem?.DelayMode ?? SequenceItem.Mode.None;
                        switch(mode) {
                            case SequenceItem.Mode.None:
                                GoNext();
                                break;
                            case SequenceItem.Mode.Duration:
                                durationTimer -= dt;
                                if(durationTimer <= 0f)
                                    GoNext();
                                break;
                            case SequenceItem.Mode.WaitUntilAllComplete:
                                if(animations.Count == 0)
                                    GoNext();
                                break;
                        }
                    }
                    break;
            }
        }

        private void GoNext() {
            switch(state) {
                case State.Showing: {
                        sequenceIndex++;
                        if(sequenceIndex >= show.Length) {
                            state = State.Finishing;
                            currentSequenceItem = null;
                        }
                        else if(sequenceIndex >= 0) {
                            SetSequence(show[sequenceIndex]);
                        }
                    }
                    break;
                case State.Hiding: {
                        sequenceIndex--;
                        if(sequenceIndex < 0) {
                            state = State.Finishing;
                            currentSequenceItem = null;
                        }
                        else if(sequenceIndex < hide.Length) {
                            int inversedIndex = (hide.Length - 1) - sequenceIndex;
                            SetSequence(hide[inversedIndex]);
                        }
                    }
                    break;
            }
        }

        private void SetSequence(SequenceItem item) {
            currentSequenceItem = item;
            durationTimer = currentSequenceItem.Duration;
            foreach(var t in currentSequenceItem.ObjectReferences) {
                if(!t.IsValid)
                    continue;
                if(state == State.Showing)
                    t.Reference.Show(durationTimer);
                else
                    t.Reference.Hide(durationTimer);
                animations.Add(t.Reference);
            }
        }

        #endregion

        #region Methods

        [ContextMenu("Cancel")]
        public void Cancel() {
            foreach(var a in animations) a?.Cancel();
            animations.Clear();
            state = State.None;
        }

        [ContextMenu("Show")]
        public void Show() {
            Cancel();
            state = State.Showing;
            sequenceIndex--;
            GoNext();
        }

        [ContextMenu("Hide")]
        public void Hide() {
            Cancel();
            state = State.Hiding;
            sequenceIndex++;
            GoNext();
        }

        #endregion

        [System.Serializable]
        public class SequenceItem {
            public enum Mode {
                None,
                WaitUntilAllComplete,
                Duration,
            }

            #region Variables

            [SerializeField] private IObjRef<IPanelAnimationObject>[] objRefs = { };
            [SerializeField] private Mode mode = Mode.WaitUntilAllComplete;
            [SerializeField, Min(0f)] private float duration = 0.4f;

            #endregion

            #region Properties

            public IReadOnlyList<IObjRef<IPanelAnimationObject>> ObjectReferences => objRefs;
            public Mode DelayMode => mode;
            public float Duration => duration;

            #endregion
        }
    }
}
