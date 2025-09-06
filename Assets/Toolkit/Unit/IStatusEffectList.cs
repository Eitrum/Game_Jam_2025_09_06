using System;
using System.Collections.Generic;

namespace Toolkit.Unit
{
    public interface IStatusEffectList
    {
        IReadOnlyList<IStatusEffect> Effects { get; }
        event OnBeforeAddStatusEffectCallback OnBeforeAddStatusEffect;
        event OnBeforeRemoveStatusEffectCallback OnBeforeRemoveStatusEffect;
        event OnAfterAddStatusEffectCallback OnAfterAddStatusEffect;
        event OnAfterRemoveStatusEffectCallback OnAfterRemoveStatusEffect;
        void Add(IStatusEffect effect);
        void Remove(IStatusEffect effect);
        void RemoveAll();
        void RemoveAll(StatusEffectType type);
        bool HasEffect(StatusEffectType type);
        int GetStacks(StatusEffectType type);
        T GetEffect<T>() where T : IStatusEffect;
        T[] GetEffects<T>() where T : IStatusEffect;
        IStatusEffect FindEffect(System.Func<IStatusEffect, bool> searchFunction);
        IStatusEffect[] FindEffects(System.Func<IStatusEffect, bool> searchFunction);
        void UpdateStatusEffects(float dt);
    }

    public delegate void OnBeforeAddStatusEffectCallback(IStatusEffect effect, ref bool shouldAdd);
    public delegate void OnBeforeRemoveStatusEffectCallback(IStatusEffect effect);
    public delegate void OnAfterAddStatusEffectCallback(IStatusEffect effect);
    public delegate void OnAfterRemoveStatusEffectCallback(IStatusEffect effect);
}
