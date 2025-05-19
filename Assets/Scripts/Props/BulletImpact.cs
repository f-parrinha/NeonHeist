using Core.Props.Interfaces.Pools;
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
        [SerializeField] private float parentResetTimer;
        [SerializeField] private AudioClip impactSound;

        public float ParentResetTimer => parentResetTimer;
        public Transform Transform => transform;
        public IPool Pool => pool;

        public void Setup(IPool pool)
        {
            this.pool = pool;
            resetParentTask = new TickTask(TimeUtils.FracToMilli(parentResetTimer), () => transform.SetParent(pool.Transform));
            
            // Get required components
            source = GetComponent<AudioSource>();
            source.clip = impactSound;
        }

        public void SetParent(Transform parent)
        {
            //transform.SetParent(parent);      FOR NOW, LEAVE THIS COMMENTED
            resetParentTask.Restart();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Restart()
        {
            particle.Play();
            source.Play();
        }
    }
}