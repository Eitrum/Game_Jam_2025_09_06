using System;

namespace Toolkit.Health
{
    public delegate void DeathCallback();
    public delegate void ReviveCallback();
    public delegate void PreDamageCallback(DamageInstance data);
    public delegate void DamageCallback(DamageInstance data);
    public delegate void PreHealCallback(HealInstance data);
    public delegate void HealCallback(HealInstance data);
    public delegate void HealthChangedCallback(IHealth source, float oldHealth, float newHealth);

    public interface IHealth
    {
        event HealthChangedCallback OnHealthChanged;
        event DeathCallback OnDeath;
        event ReviveCallback OnRevive;

        float Current { get; set; }
        float Full { get; set; }
        float Percentage { get; set; }

        IHealth Root { get; }
        bool IsAlive { get; }

        void Damage(DamageInstance data);
        void Heal(HealInstance data);
        void Kill();

        void Revive(float health);
        void Restore(bool triggerEvent = true);
    }

    public interface IReadOnlyHealth
    {
        float Current { get; }
        float Full { get; }
        float Percentage { get; }

        IHealth Root { get; }
        bool IsAlive { get; }
    }

    public interface IDamageEvent
    {
        event PreDamageCallback OnPreDamage;
        event DamageCallback OnDamage;
    }

    public interface IHealEvent
    {
        event PreHealCallback OnPreHeal;
        event HealCallback OnHeal;
    }
}
