using Character;
using Core.Health.Events;
using Core.Health.Interfaces;
using System;
using UnityEngine;

namespace Core.Character
{
    public class CharacterHealthReader : MonoBehaviour, IHealthHolder
    {
        [SerializeField] private CharacterHealth mainHealth;
        [SerializeField] private float damageBoost = 1f;

        public float MaxHealth => mainHealth.MaxHealth;

        public float Health => mainHealth.Health;

        public void AddOnDamageHandler(EventHandler<OnHealthChangeArgs> handler)
        {
           mainHealth.AddOnDamageHandler(handler);
        }

        public void AddOnDeathHandler(EventHandler handler)
        {
            mainHealth.AddOnDeathHandler(handler);
        }

        public void AddOnHealHandler(EventHandler<OnHealthChangeArgs> handler)
        {
            mainHealth.AddOnHealHandler(handler);
        }

        public void Damage(float damage)
        {
            mainHealth.Damage(damage * damageBoost);
        }

        public void Heal(float health)
        {
            mainHealth.Heal(health);
        }
    }
}