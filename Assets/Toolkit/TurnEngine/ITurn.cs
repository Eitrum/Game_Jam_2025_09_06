using System;

namespace Toolkit.TurnEngine {
    public interface ITurn {
        /// <summary>
        /// Whenever a turn starts.
        /// </summary>
        void OnStart();
        /// <summary>
        /// Whenever the turn is ending.
        /// </summary>
        void OnEnd();
        /// <summary>
        /// To return whether a turn is completed or not.
        /// </summary>
        bool IsComplete { get; }
        /// <summary>
        /// Whether a turn has started or not.
        /// </summary>
        bool HasStarted { get; }
        /// <summary>
        /// If user promted to skip this turn.
        /// </summary>
        bool OnSkip();
        /// <summary>
        /// Used to reset the data of the class and recycle the object in an given instance to keep player order.
        /// </summary>
        void OnReset();
    }
}
