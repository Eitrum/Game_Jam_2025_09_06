using System;


namespace Toolkit.TurnEngine
{
    public static class TurnUtility
    {

    }

    public delegate void OnRoundPassedCallback(int round);
    public delegate void OnNewTurnDelegate(ITurn turn);
    public delegate void OnEndTurnDelegate(ITurn turn);

    [System.Flags]
    public enum TurnMode
    {
        None = 0,
        /// <summary>
        /// Recycle mode enables 
        /// </summary>
        Recycle = 1,
        /// <summary>
        /// Enables a round indicator between a set of players.
        /// Allows adding new ITurn before next round.
        /// </summary>
        RoundIndicator = 2,
        /// <summary>
        /// Enables update to check when next turn should be called.
        /// </summary>
        Update = 4,
        /// <summary>
        /// When there is no more turns in the instance, it will attempt to destroy it.
        /// </summary>
        DestroyWhenEmpty = 8,

        All = ~0
    }

    public class RoundIndicator : ITurn
    {
        private bool complete = false;

        private System.Action onRoundPassed;

        public bool IsComplete => complete;
        public bool HasStarted => complete;

        public RoundIndicator(System.Action callback) {
            this.onRoundPassed = callback;
        }

        public void OnStart() {
            onRoundPassed.Invoke();
            complete = true;
        }
        public void OnEnd() {
        }
        public bool OnSkip() => false;
        public void OnReset() {
            complete = false;
        }
    }
}
