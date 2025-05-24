using Core.Common.Interfaces;
using Core.Health.Events;
using Core.Health.Interfaces;
using System;
using UnityEngine;


namespace Character
{
    public class CharacterHealth : MonoBehaviour, IHealthHolder, ICleanable, IInitializable
    {
        [SerializeField] private float maxHealth;

        private event EventHandler onDeath;
        private event EventHandler<OnHealthChangeArgs> onDamage;
        private event EventHandler<OnHealthChangeArgs> onHeal;

        public float MaxHealth => maxHealth;
        public float Health { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            Health = maxHealth;
        }

        private void Start()
        {
            Initialize();
        }


        public virtual void Initialize()
        {
            if (IsInitialized) return;

            IsInitialized = true;

            // Let other classes implement the Initialize method (override)
        }

        public void Heal(float health)
        {
            float oldHealth = Health;
            Health = Mathf.Clamp(Health + health, 0, MaxHealth);

            RaiseOnHeal(health, oldHealth, Health);
        }

        public void Damage(float damage)
        {
            float oldHealth = Health;
            Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

            RaiseOnDamage(damage, oldHealth, Health);

            if (Health == 0)
            {
                RaiseOnDeath();
            }
        }


        public void AddOnHealHandler(EventHandler<OnHealthChangeArgs> handler) => onHeal += handler;
        public void AddOnDamageHandler(EventHandler<OnHealthChangeArgs> handler) => onDamage += handler;
        public void AddOnDeathHandler(EventHandler handler) => onDeath += handler;
        protected void RaiseOnDeath() => onDeath?.Invoke(this, EventArgs.Empty);
        protected void RaiseOnHeal(float change, float oldHealth, float newHealth) => onHeal?.Invoke(this, new OnHealthChangeArgs()
        {
            Change = change,
            OldHealth = oldHealth,
            NewHealth = newHealth
        });
        protected void RaiseOnDamage(float change, float oldHealth, float newHealth) => onDamage?.Invoke(this, new OnHealthChangeArgs()
        {
            Change = change,
            OldHealth = oldHealth,
            NewHealth = newHealth
        });

        public void CleanUp()
        {
            onDamage = null;
            onHeal = null;
            onDeath = null;
        }
    }
}
