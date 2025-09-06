using System;

namespace Toolkit.Unit
{
    public interface IStatusEffect
    {
        string Name { get; }
        string Description { get; }
        StatusEffectType Type { get; }
        float Duration { get; }

        void OnApply();
        void OnUpdate(float dt);
        void OnRemove();

        IStatusEffect Copy(IUnit unit);
    }

    public class BasicStatusEffect : IStatusEffect
    {
        private StatusEffectType type = StatusEffectType.None;
        private float duration = 0f;

        public string Name => StatusEffectUtility.GetDefaultName(type);
        public string Description => StatusEffectUtility.GetDefaultDescription(type);
        public StatusEffectType Type => type;
        public float Duration => duration;

        public BasicStatusEffect(StatusEffectType type, float duration) {
            this.type = type;
            this.duration = duration;
        }

        public void OnApply() { }
        public void OnUpdate(float dt) => duration -= dt;
        public void OnRemove() { }

        public IStatusEffect Copy(IUnit unit) {
            return new BasicStatusEffect(type, duration);
        }
    }
}
