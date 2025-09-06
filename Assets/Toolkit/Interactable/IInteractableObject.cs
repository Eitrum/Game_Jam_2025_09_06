using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Interactables {
    public interface IInteractableObject {
        bool IsHidden { get; }
        void Interact(Source source, int index);
        IReadOnlyList<IInteractableOption> Options { get; }

        void Register(IInteractableOption option);
        void Unregister(IInteractableOption option);
    }

    public class InteractableOption : IInteractableOption {

        #region Variables

        public string Name { get; private set; }
        public string Description { get; private set; } = string.Empty;

        public OptionState State { get; private set; }
        public int Order { get; private set; }

        private System.Action<Source> method;

        #endregion

        #region Constructor

        public InteractableOption(string name, System.Action<Source> method) {
            this.Name = name;
            this.method = method;
        }

        public InteractableOption(string name, OptionState state, System.Action<Source> method) {
            this.Name = name;
            this.State = state;
            this.method = method;
        }

        #endregion

        #region Option Impl

        public void Interact(Source source) {
            method?.Invoke(source);
        }

        public OptionState GetState(Source source) {
            return State;
        }

        #endregion
    }

    public class InteractableDynamicOption : IInteractableOption {
        #region Variables

        public string Name { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public int Order { get; private set; }

        private System.Action<Source> method;
        private System.Func<Source, OptionState> stateCheck;

        #endregion

        #region Constructor

        public InteractableDynamicOption(string name, System.Action<Source> method, System.Func<Source, OptionState> stateCheck) {
            this.Name = name;
            this.method = method;
            this.stateCheck = stateCheck;
        }

        public InteractableDynamicOption(string name, string description, System.Action<Source> method, System.Func<Source, OptionState> stateCheck) {
            this.Name = name;
            this.Description = description;
            this.method = method;
            this.stateCheck = stateCheck;
        }

        #endregion

        #region Option Impl

        public void Interact(Source source) {
            method?.Invoke(source);
        }

        public OptionState GetState(Source source) {
            return stateCheck?.Invoke(source) ?? OptionState.None;
        }

        #endregion
    }
}
