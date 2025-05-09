using Core.Health.Events;
using System;

namespace Core.Health.Interfaces
{
    public interface IHealthHolder
    {
        float MaxHealth { get; }
        float Health { get; }

        void Damage(float damage);
        void Heal(float health);

        void AddOnDamageHandler(EventHandler<OnHealthChangeArgs> handler);
        void AddOnHealHandler(EventHandler<OnHealthChangeArgs> handler);
        void AddOnDeathHandler(EventHandler handler);
    }
}