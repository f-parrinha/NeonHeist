using Core.Health.Interfaces;
using UnityEngine;

namespace Core.Common
{
    [RequireComponent(typeof(Collider))]
    public class DeathCollider : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damage = 100;

        public float Damage => damage;


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IHealthHolder>(out var healthHolder))
            {
                healthHolder.Damage(damage);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<IHealthHolder>(out var healthHolder))
            {
                healthHolder.Damage(damage);
            }
        }
    }
}