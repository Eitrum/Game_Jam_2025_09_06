using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public static class StateUtility {
        #region Phase

        public static bool IsInTransition(IState state) {
            if(state == null) {
                return false;
            }
            switch(state.Phase) {
                case Phase.Entering:
                case Phase.Exiting: return true;
            }
            return false;
        }

        #endregion

        #region GetMetadata

        public static bool TryGetMetadata(IState state, out IStateMetadata metadata) {
            metadata = state as IStateMetadata;
            return metadata != null;
        }

        public static string GetName(IState state) {
            if(state == null)
                return "null";
            if(TryGetMetadata(state, out IStateMetadata metadata))
                return metadata.Name;
            return state.GetType().Name;
        }

        public static string GetDescription(IState state) {
            if(state == null)
                return "null";
            if(TryGetMetadata(state, out IStateMetadata metadata))
                return metadata.Description;
            return "missing";
        }

        #endregion
    }
}
