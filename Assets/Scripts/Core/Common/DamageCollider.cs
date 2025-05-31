using Core.Health.Interfaces;
using UnityEngine;

namespace Core.Common
{
    [RequireComponent(typeof(Collider))]
    public class DamageCollider : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damage = 100;

        private Collider coll;

        public bool IsEnabled => coll.enabled;
        public float Damage => damage;


        private void Start()
        {
            coll = GetComponent<Collider>();
        }

        public void Enable()
        {
            coll.enabled = true;
        }

        public void Disable()
        {
            coll.enabled = false;
        }

        public void Toggle()
        {
            coll.enabled = !coll.enabled;
        }

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