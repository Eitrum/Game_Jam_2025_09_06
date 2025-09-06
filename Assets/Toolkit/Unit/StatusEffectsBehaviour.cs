using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Unit
{
    [AddComponentMenu("Toolkit/Unit/Status Effects")]
    public class StatusEffectsBehaviour : MonoBehaviour, IStatusEffectList, IUpdate, IFixedUpdate, IEarlyUpdate, ILateUpdate, IPostUpdate, IOnBeforeRender
    {
        #region Variables

        [SerializeField] private UpdateModeMask updateMode = UpdateModeMask.None;

        private List<IStatusEffect> effects = new List<IStatusEffect>();
        private List<UpdateBlock> updateList = new List<UpdateBlock>();
        private OnBeforeAddStatusEffectCallback onBeforeAddStatusEffect;
        private OnBeforeRemoveStatusEffectCallback onBeforeRemoveStatusEffect;
        private OnAfterAddStatusEffectCallback onAfterAddStatusEffect;
        private OnAfterRemoveStatusEffectCallback onAfterRemoveStatusEffect;

        #endregion

        #region Properties

        public IReadOnlyList<IStatusEffect> Effects => effects;

        public bool IsNull => this == null;

        public event OnBeforeAddStatusEffectCallback OnBeforeAddStatusEffect { add => onBeforeAddStatusEffect += value; remove => onBeforeAddStatusEffect -= value; }
        public event OnBeforeRemoveStatusEffectCallback OnBeforeRemoveStatusEffect { add => onBeforeRemoveStatusEffect += value; remove => onBeforeRemoveStatusEffect -= value; }
        public event OnAfterAddStatusEffectCallback OnAfterAddStatusEffect { add => onAfterAddStatusEffect += value; remove => onAfterAddStatusEffect -= value; }
        public event OnAfterRemoveStatusEffectCallback OnAfterRemoveStatusEffect { add => onAfterRemoveStatusEffect += value; remove => onAfterRemoveStatusEffect -= value; }

        #endregion

        #region Unity Methods

        void OnEnable() {
            UpdateSystem.Subscribe(this, updateMode);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(this, updateMode);
        }

        #endregion

        #region IStatusEffect Impl

        public void Add(IStatusEffect effect) {
            bool shouldAdd = true;
            onBeforeAddStatusEffect?.Invoke(effect, ref shouldAdd);
            if(shouldAdd) {
                effects.Add(effect);
                effect.OnApply();
                onAfterAddStatusEffect?.Invoke(effect);
            }
        }

        public int GetStacks(StatusEffectType type) {
            int res = 0;
            for(int i = effects.Count - 1; i >= 0; i--) {
                if(effects[i].Type == type) {
                    res++;
                }
            }
            return res;
        }

        public T GetEffect<T>() where T : IStatusEffect {
            for(int i = 0, length = effects.Count; i < length; i++) {
                if(effects[i] is T t) {
                    return t;
                }
            }
            return default;
        }

        public T[] GetEffects<T>() where T : IStatusEffect {
            List<T> temp = new List<T>();
            for(int i = 0, length = effects.Count; i < length; i++) {
                if(effects[i] is T t) {
                    temp.Add(t);
                }
            }
            return temp.ToArray();
        }

        public IStatusEffect FindEffect(System.Func<IStatusEffect, bool> searchFunction) {
            for(int i = 0, length = effects.Count; i < length; i++) {
                if(searchFunction(effects[i])) {
                    return effects[i];
                }
            }
            return default;
        }

        public IStatusEffect[] FindEffects(System.Func<IStatusEffect, bool> searchFunction) {
            return effects.Where(x => searchFunction(x)).ToArray();
        }

        public bool HasEffect(StatusEffectType type) {
            for(int i = 0, length = effects.Count; i < length; i++) {
                if(effects[i].Type == type)
                    return true;
            }
            return false;
        }

        public void Remove(IStatusEffect effect) {
            onBeforeRemoveStatusEffect?.Invoke(effect);
            if(effects.Remove(effect)) {
                effect.OnRemove();
                onAfterRemoveStatusEffect?.Invoke(effect);
            }
        }

        public void RemoveAll() {
            for(int i = effects.Count - 1; i >= 0; i--) {
                var eff = effects[i];
                onBeforeRemoveStatusEffect?.Invoke(eff);
                effects.RemoveAt(i);
                eff.OnRemove();
                onAfterRemoveStatusEffect?.Invoke(eff);
            }
        }

        public void RemoveAll(StatusEffectType type) {
            for(int i = effects.Count - 1; i >= 0; i--) {
                var eff = effects[i];
                if(eff.Type == type) {
                    onBeforeRemoveStatusEffect?.Invoke(eff);
                    effects.RemoveAt(i);
                    eff.OnRemove();
                    onAfterRemoveStatusEffect?.Invoke(eff);
                }
            }
        }

        public void UpdateStatusEffects(float dt) {
            updateList.Clear();
            var count = effects.Count;
            for(int i = 0; i < count; i++) {
                updateList.Add(new UpdateBlock(effects[i]));
            }
            for(int i = updateList.Count - 1; i >= 0; i--) {
                var block = updateList[i];
                var eff = block.effect;
                eff.OnUpdate(dt);
                if(eff.Duration <= 0f) {
                    onBeforeRemoveStatusEffect?.Invoke(eff);
                    block.remove = true;
                }
                updateList[i] = block;
            }
            for(int i = count - 1; i >= 0; i--) {
                var block = updateList[i];
                if(block.remove) {
                    Remove(block.effect);
                }
            }
        }

        #endregion

        #region IUpdateSystem Impl

        void IPostUpdate.PostUpdate(float dt) => UpdateStatusEffects(dt);
        void ILateUpdate.LateUpdate(float dt) => UpdateStatusEffects(dt);
        void IEarlyUpdate.EarlyUpdate(float dt) => UpdateStatusEffects(dt);
        void IFixedUpdate.FixedUpdate(float dt) => UpdateStatusEffects(dt);
        void IUpdate.Update(float dt) => UpdateStatusEffects(dt);
        void IOnBeforeRender.OnBeforeRender(float dt) => UpdateStatusEffects(dt);

        #endregion

        private struct UpdateBlock
        {
            public bool remove;
            public IStatusEffect effect;

            public UpdateBlock(IStatusEffect effect) {
                this.effect = effect;
                this.remove = false;
            }
        }
    }
}
