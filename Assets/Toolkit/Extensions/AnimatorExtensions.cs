using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit
{
    public static class AnimatorExtensions
    {
        #region Copy

        public static void CopyStateTo(this Animator animator, Animator target) => new AnimatorSnapshot(animator).CopyTo(target);
        public static void CopyStateFrom(this Animator animator, Animator source) => new AnimatorSnapshot(source).CopyTo(animator);

        #endregion

        #region Snapshot

        public static AnimatorSnapshot GetSnapshot(this Animator animator) {
            return new AnimatorSnapshot(animator);
        }

        public static void SetSnapshot(this Animator animator, AnimatorSnapshot snapshot) {
            snapshot.CopyTo(animator);
        }

        #endregion
    }

    [System.Serializable]
    public class AnimatorSnapshot
    {
        #region Variables

        private Animator source;
        [SerializeField] private LayerState[] layerStates;
        [SerializeField] private Parameter[] parameters;

        #endregion

        #region Properties

        public Animator Source => source;
        public IReadOnlyList<LayerState> LayerStates => layerStates;
        public IReadOnlyList<Parameter> Parameters => parameters;

        #endregion

        #region Constructor

        public AnimatorSnapshot() { }
        public AnimatorSnapshot(Animator anim) {
            CopyFrom(anim);
        }

        #endregion

        #region Copy

        public void CopyFrom(Animator anim) {
            this.source = anim;

            // Copy Layers
            if(layerStates == null || layerStates.Length != anim.layerCount)
                layerStates = new LayerState[anim.layerCount];
            for(int i = 0, length = anim.layerCount; i < length; i++) {
                var si = anim.GetCurrentAnimatorStateInfo(i);
                layerStates[i] = new LayerState(si, i);
            }

            // Copy Parameters
            if(parameters == null || parameters.Length != anim.parameterCount)
                parameters = new Parameter[anim.parameterCount];
            for(int i = 0, length = anim.parameterCount; i < length; i++) {
                parameters[i] = new Parameter(anim, anim.GetParameter(i));
            }
        }

        public void CopyTo(Animator anim) {
            // Copy Layers
            for(int i = 0, length = layerStates.Length; i < length; i++) {
                var state = layerStates[i];
                anim.Play(state.hash, state.layer, state.normalizedTime);
            }

            // Copy Parameters
            for(int i = 0, length = parameters.Length; i < length; i++) {
                parameters[i].CopyTo(anim);
            }
        }

        #endregion

        [System.Serializable]
        public struct LayerState
        {
            #region Variables

            public int hash;
            public int layer;
            public float normalizedTime;

            #endregion

            #region Constructor

            public LayerState(AnimatorStateInfo info) {
                this.hash = info.shortNameHash;
                this.normalizedTime = info.normalizedTime;
                this.layer = 0;
            }

            public LayerState(AnimatorStateInfo info, int layer) {
                this.hash = info.shortNameHash;
                this.normalizedTime = info.normalizedTime;
                this.layer = layer;
            }

            #endregion
        }

        [System.Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct Parameter
        {
            #region Variables

            [FieldOffset(0)] public int hash;
            [FieldOffset(4)] public int @int;
            [FieldOffset(4)] public float @float;
            [FieldOffset(4)] public bool @bool;
            [FieldOffset(8)] public AnimatorControllerParameterType type;

            #endregion

            #region Constructor

            public Parameter(AnimatorControllerParameter parameter) {
                this.hash = parameter.nameHash;
                this.type = parameter.type;
                @bool = false;
                @float = 0f;
                @int = 0;

                switch(parameter.type) {
                    case AnimatorControllerParameterType.Bool:
                    case AnimatorControllerParameterType.Trigger:
                        @bool = parameter.defaultBool;
                        break;
                    case AnimatorControllerParameterType.Float:
                        @float = parameter.defaultFloat;
                        break;
                    case AnimatorControllerParameterType.Int:
                        @int = parameter.defaultInt;
                        break;
                }
            }

            public Parameter(Animator anim, AnimatorControllerParameter parameter) {
                this.hash = parameter.nameHash;
                this.type = parameter.type;
                @bool = false;
                @float = 0f;
                @int = 0;

                switch(parameter.type) {
                    case AnimatorControllerParameterType.Bool:
                        @bool = anim.GetBool(this.hash);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        @bool = anim.GetBool(this.hash);
                        break;
                    case AnimatorControllerParameterType.Float:
                        @float = anim.GetFloat(this.hash);
                        break;
                    case AnimatorControllerParameterType.Int:
                        @int = anim.GetInteger(this.hash);
                        break;
                }
            }

            #endregion

            #region Copy

            public void CopyFrom(Animator animator) {
                switch(type) {
                    case AnimatorControllerParameterType.Bool:
                    case AnimatorControllerParameterType.Trigger:
                        @bool = animator.GetBool(hash);
                        break;
                    case AnimatorControllerParameterType.Float:
                        @float = animator.GetFloat(hash);
                        break;
                    case AnimatorControllerParameterType.Int:
                        @int = animator.GetInteger(hash);
                        break;
                }
            }

            public void CopyTo(Animator animator) {
                switch(type) {
                    case AnimatorControllerParameterType.Bool:
                    case AnimatorControllerParameterType.Trigger:
                        animator.SetBool(hash, @bool);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(hash, @float);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(hash, @int);
                        break;
                }
            }

            #endregion
        }
    }
}
