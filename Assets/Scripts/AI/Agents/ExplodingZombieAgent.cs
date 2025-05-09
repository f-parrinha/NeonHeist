using AI.Enums;
using AI.Events;
using Core.Props.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Agents
{
    public class ExplodingZombieAgent : AIAgent, IExplodable
    {
        [SerializeField] [Range(100, 2000)] private int blowupTimer = 1000;
        [SerializeField] private GameObject explosionObject;
        [SerializeField] private float minDistanceToBlowup = 1.5f;
        [SerializeField] private AudioClip[] blowupScreams;

        private TickTimer selfDestructTimer;
        private bool isBlowingUp;

        public override void Initialize()
        {
            if (IsInitialized) return;

            if (!explosionObject.TryGetComponent<IExplodable>(out _))
            {
                Log.Warning(this, "Initialize", "Explosion object is not IExplodable");
            }

            selfDestructTimer = new TickTimer(blowupTimer, () => Health.Damage(Health.MaxHealth));
            IsInitialized = true;
        }


        public override void CleanUp()
        {
            base.CleanUp();

            selfDestructTimer.Stop();
            selfDestructTimer = null;
        }
        public void Explode()
        {
            Instantiate(explosionObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }


        /* ACTIONS */

        protected override void CalmAction()
        {
            DefaultCalmAction();
        }

        protected override void DangerAction()
        {
            goal.Set(vision.ClosestTarget.transform.position);
            movement.MoveTo(goal.Position, AIMoveState.Run);

            // It's close to the target. DIEEEEEE!
            if (goal.Distance < minDistanceToBlowup && !isBlowingUp)
            {
                if (blowupScreams.Length > 0)
                {
                    AudioUtils.CreateAudio(transform.position, blowupScreams[Random.Range(0, blowupScreams.Length)], 2f, voices.AudioSource.pitch);
                }

                selfDestructTimer.Start();
                isBlowingUp = true;
                return;
            }
        }

        protected override void OnDeathHandler(object sender, EventArgs args)
        {
            Explode();
        }

        protected override void OnTargetFoundHandler(object sender, OnTargetFoundArgs args)
        {
            State = AIState.Danger;
        }

        protected override void OnTargetLostHandler(object sender, OnTargetLostArgs args)
        {
            State = AIState.Calm;
        }
    }
}