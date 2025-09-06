using UnityEngine;

namespace Toolkit.Network {
    public enum RPCTarget {
        /// <summary>
        /// Unassigned target.
        /// This will in the most cases produce an error
        /// </summary>
        None = 0,

        /// <summary>
        /// Send to self. 
        /// Used to send RPC to itself
        /// </summary>
        Local = 1,

        /// <summary>
        /// Send to server.
        /// Server deals with how to relay it.
        /// </summary>
        Server = 2,

        /// <summary>
        /// Indicates that it should be relayed to every client
        /// </summary>
        Remotes = 3,

        /// <summary>
        /// Send to the owner of the NetworkView
        /// </summary>
        Owner = 4,
    }

    public class RPCReceiveData {

        #region Variables

        public readonly NetworkView View;
        public readonly NetworkPlayer Sender;

        #endregion

        #region Properties

        public bool IsFromOwner => View.Owner == Sender;
        public bool IsFromServer => Sender == NetworkPlayer.Server;

        public bool IsUnknown => Sender == NetworkPlayer.Unknown;

        #endregion

        #region Constructor

        public RPCReceiveData(NetworkView view, NetworkPlayer player) {
            this.View = view;
            this.Sender = player;
        }

        #endregion
    }
}
