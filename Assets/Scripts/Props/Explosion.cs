using Core.Health.Interfaces;
using Core.Props.Interfaces;
using UnityEngine;

namespace Props
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Explosion : MonoBehaviour, IExplodable, IDamageable
    {
        private ParticleSystem particle;

        [SerializeField] private float explosionRadius = 8f;
        [SerializeField] private float forceBoost = 2f;
        [SerializeField] private float damage = 50f;

        public float Damage => damage;

        private void Start()
        {
            particle = GetComponent<ParticleSystem>();

            Explode();
        }

        public void Explode()
        {
            particle.Play();

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            if (colliders.Length == 0) return;

            foreach (Collider collider in colliders)
            {
                var dir = collider.transform.position - transform.position;
                var distance = dir.magnitude;
                var distanceFactor = Mathf.Clamp01(1 - (distance / explosionRadius));
                var force = distanceFactor * forceBoost * dir.normalized;

                Physics.Raycast(transform.position, dir, out var hit, distance);

                if (collider == hit.collider)
                {
                    if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.AddForce(force, ForceMode.Impulse);
                    }

                    if (hit.collider.TryGetComponent<IHealthHolder>(out var health))
                    {
                        health.Damage(damage * distanceFactor);
                    }
                }
            }
        }
    }
}