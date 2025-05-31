using Core.Props.Interfaces.Pools;
using Core.Utilities;
using Core.Utilities.Timing;
using UnityEngine;

namespace Props
{
    [RequireComponent(typeof(AudioSource))]
    public class BulletImpact : MonoBehaviour, IPooledObject
    {
        private IPool pool;
        private TickTask resetParentTask;
        private AudioSource source;

        [SerializeField] private ParticleSystem particle;
        [SerializeField] private AudioClip[] impactSounds;
        [SerializeField] private float pitchInterval = 0.1f;
        [SerializeField] private float parentResetTimer;

        public float ParentResetTimer => parentResetTimer;
        public Transform Transform => transform;
        public IPool Pool => pool;

        public void Setup(IPool pool)
        {
            this.pool = pool;
            resetParentTask = new TickTask(TimeUtils.FracToMilli(parentResetTimer), () => transform.SetParent(pool.Transform));
            
            // Get required components
            source = GetComponent<AudioSource>();
        }

        public void SetParent(Transform parent)
        {
            // TODO: Add upon destroy handler for objects that can be Destroy(), to unset these parenting
            transform.SetParent(parent);
            resetParentTask.Restart();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Restart()
        {
            particle.Play();
            PlayImpactSound();
        }

        public void PlayImpactSound()
        {
            if (impactSounds == null || impactSounds.Length == 0) return;

            source.clip = impactSounds[Random.Range(0, impactSounds.Length)];
            source.pitch = AudioUtils.BASE_PITCH + Random.Range(-pitchInterval, pitchInterval);
            source.Play();
        }
    }
}