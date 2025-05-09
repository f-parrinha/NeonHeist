using Core.Health.Interfaces;
using Core.Utilities.Timing;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Props.Traps
{
    [RequireComponent(typeof(AudioSource))]
    public class Trap : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem particleEffect;
        [SerializeField] protected float damage = 35f;
        [SerializeField] protected float damageTaskPeriod = 1f;
        [SerializeField] protected float activePeriod = 5f;
        [SerializeField] protected float pitch = 1f;
        [SerializeField] protected float pitchInterval = 0.1f; 

        protected AudioSource audioSource;
        protected List<IHealthHolder> healthHolders;
        protected TickTask damageTask;
        protected TickTimer activeTimer;
        
        public bool IsActive { get; protected set; }
        public float Damage => damage;

        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
            healthHolders = new List<IHealthHolder>();

            damageTask = new TickTask(TimeUtils.FracToMilli(damageTaskPeriod), DamageTask);
            activeTimer = new TickTimer(TimeUtils.FracToMilli(activePeriod), Deactivate);

            particleEffect?.Stop();
        }


        public virtual void Activate()
        {
            if (IsActive) return;

            IsActive = true;
            
            damageTask.Restart();
            activeTimer.Restart();

            audioSource.pitch = pitch + Random.Range(-pitchInterval, pitchInterval);
            audioSource.Play();
            particleEffect?.Play();
            
            DamageTask();
        }

        public void Deactivate()
        {
            IsActive = false;

            particleEffect?.Stop();
            damageTask.Stop();
            activeTimer.Stop();
            audioSource.Stop();
        }

        private void DamageTask()
        {
            if (!IsActive) return;

            foreach (var healthHolder in healthHolders)
            {
                healthHolder.Damage(damage);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent<IHealthHolder>(out var healthHolder);

            if (healthHolder == null) return;

            healthHolders.Add(healthHolder);
        }

        private void OnTriggerExit(Collider other)
        {
            other.TryGetComponent<IHealthHolder>(out var healthHolder);

            if (healthHolder == null) return;

            healthHolders.Remove(healthHolder);
        }
    }
}
