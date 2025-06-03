using AI.Common;
using AI.Controllers;
using AI.Enums;
using AI.Events;
using AI.Goals;
using Character;
using Core.Common.Queue;
using Core.Health.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Agents
{
    /// <summary>
    /// TODO: Continuous investigation on all targets - right now, it only investigates the closest one
    /// </summary>
    public class BossGuardAgent : GenericEnemyAgent
    {
        public const float TO_ALERT_THRESHOLD = 40f;
        public const float TO_DANGER_THRESHOLD = 100f;

        private const float CLOSE_DISTANCE_POINT = 3f;
        private const float HIT_SOUND_VOLUME = 0.5f;

        private Vector3 initPos;

        // When the AI decides do to go to a close distance point, it does not wait in that position
        private float lastGoalPosDistance;
        private ScannedTarget currentTarget;
        private Vector3 lastGoalPos;

        // Only activates damage collider on the katana for some time. The task is used for that
        private TickTimer shootWait;

        private int toShoot;
        private TickTimer shootTimer;

        [SerializeField] private float alertDetectionFactor = 15f;
        [Header("Patrolling Settings")]
        [SerializeField] private float maxPatrolRadius = 5;
        [SerializeField] private float maxWaitTime = 5f;
        [SerializeField] private float minWaitTime = 2f;
        [Header("Attack Move Settings")]
        [SerializeField] private float minAttMoveDist = 3f;
        [SerializeField] private float maxAttMoveDist = 5f;
        [SerializeField] private float minAttMoveWait = 1f;
        [SerializeField] private float maxAttMoveWait = 2f;
        [Header("Shoot Settings")]
        [SerializeField] private float shootOffset = 8f;
        [SerializeField] private float shootCooldown = 10f;
        [SerializeField] private float rateOfFire = 200;
        [SerializeField] private float rifleDamage = 15f;
        [SerializeField] private float impactForce = 100f;
        [SerializeField] private int shotCount = 10;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem bulletTracer;
        [SerializeField] private AudioSource gunAudioSource;
        [Header("Sound Settings")]
        [SerializeField][Range(0f, 1f)] private float calmSoundProbability = 0.2f;
        [SerializeField] private AudioClip[] attackHitSounds;
        [Header("Heal Ability Settings")]
        [SerializeField] private float healRate = 20f;
        [SerializeField] private float minHealthPrctg = 0.8f;

        public float RateOfFireRPM => rateOfFire;
        public float RateOfFirePSec => 60 / rateOfFire;

        public override void Initialize()
        {
            // State
            initPos = transform.position;
            lastGoalPos = initPos;
            lastGoalPosDistance = 0;
            shootTimer = new TickTimer(TimeUtils.FracToMilli(RateOfFirePSec));
            shootWait = new TickTimer(TimeUtils.FracToMilli(shootCooldown));

            // Setup vision to scan enemies
            Faction[] factionsArray = enemyFactions.ToArray();
            if (factionsArray.Length > 0)
            {
                vision.ScanFactionFilter = factionsArray;
            }
        }

        /// <summary>
        /// Defines random patrolling, and target investigatio. The agent patrols around its sector and investigates the closest target.
        ///     Changes its state to alert state
        /// </summary>
        protected override void CalmAction()
        {
            animations.SetCombatLayerWeight(0);
            ScannedTarget closestTarget = LookAtClosestTarget();
            InvestigateTarget(closestTarget, detectionFactor);

            // State transition on detection
            if (closestTarget != null && closestTarget.DetectionLevel >= TO_ALERT_THRESHOLD)
            {
                State = AIState.Alert;
                voices.PlayAlertVoice();
                currentTarget = closestTarget;
                goal = new PositionGoal(this, movement.MoveTo(currentTarget.Position, AIMoveState.Walk));
                return;
            }

            // State transition on new sound heard
            if (ears.HasTargets)
            {
                Vector3 soundPos = ears.ClosestTarget.transform.position;
                State = AIState.Alert;
                Voices.PlayAlertVoice();
                goal = new PositionGoal(this, movement.MoveTo(soundPos, AIMoveState.Run));
                return;
            }

            // Play calm sound
            if (Random.Range(0, 1f) < calmSoundProbability)
            {
                Voices.PlayCalmVoice();
            }

            // Main goal execution
            if (goal.Evaluate())
            {
                switch (goal)
                {
                    case PositionGoal _:
                        UponPositionGoalFinished();
                        break;
                    case TimeGoal _:
                        UponTimeGoalFinished();
                        break;
                    default:
                        Log.Warning(this, "CalmAction", "An Undefined goal was given");
                        break;
                }
            }
        }

        /// <summary>
        /// Patrols around a current supposed target. Detection is faster in this state. Can still pick-up new targets (other than the current one)
        ///     Moves the current (alert) state to a danger one.
        /// </summary>
        protected override void AlertAction()
        {
            animations.SetCombatLayerWeight(0);
            ScannedTarget closestTarget = LookAtClosestTarget();
            InvestigateTarget(closestTarget, alertDetectionFactor);

            // Move to visible target
            if (closestTarget != null && closestTarget.DetectionLevel >= TO_DANGER_THRESHOLD)
            {
                currentTarget = closestTarget;
                goal = new PositionGoal(this, movement.MoveTo(currentTarget.Position, AIMoveState.Run));

                // State transition on detection
                if (closestTarget.DetectionLevel >= TO_DANGER_THRESHOLD)
                {

                    Voices.PlayDangerVoice();
                    goal = null;
                    State = AIState.Danger;
                    return;
                }
            }

            if (closestTarget == null && ears.HasTargets)
            {
                goal = new PositionGoal(this, movement.MoveTo(ears.ClosestTarget.transform.position, AIMoveState.Run));
            } 

            // Main goal execution
            if (goal.Evaluate()) 
            {
                if (goal is TimeGoal)
                {
                    State = AIState.Calm;
                    return;
                }

                goal = new TimeGoal(this, 1f);
            }
        }

        /// <summary>
        /// Defines moving to player and trying to attack him
        /// </summary>
        protected override void DangerAction()
        {
            animations.SetCombatLayerWeight(1);

            Action newMoveAction = () => {
                float rotation = Random.Range(0, 360f);
                float distance = Random.Range(minAttMoveDist, maxAttMoveDist);
                Vector3 pos = Quaternion.Euler(0, rotation, 0) * Vector3.forward * distance + transform.position;
                goal = new PositionGoal(this, movement.MoveTo(pos, AIMoveState.Run)); 
            };

            Vector3 toTarget = currentTarget.Position - transform.position;
            float distToTarget = toTarget.magnitude;

            if (goal == null)
            {
                newMoveAction();
            }

            // Shoot
            if (shootTimer.IsFinished && vision.HasTargets)
            {
                Shoot();
                shootTimer.Restart();
            }

            // Move around
            if (goal.Evaluate())
            {
                // Heal action
                if (!vision.HasTargets && health.Health < health.MaxHealth * minHealthPrctg)
                {
                    health.Heal(healRate * (refreshRate / 1000f));
                    return;
                }

                // Interval between wait and move
                switch(goal)
                {
                    case PositionGoal _:
                        goal = new TimeGoal(this, Random.Range(minAttMoveWait, maxAttMoveWait));
                        break;
                    case TimeGoal _:
                        newMoveAction();
                        break;
                    default:

                        break;
                }
            }
        }


        protected override void OnTargetsUpdateHandler(object sender, OnTargetsUpdateArgs args)
        {
            // Leave empty
        }


        /** ---------- STATE GOAL/TASK METHODS ---------- */

        /// <summary>
        /// Sets a new TimeGoal if the last point was far enough, making the AI wait a bit. Otherwise just set a n ew position goal to keep it moving
        /// </summary>
        private void UponPositionGoalFinished()
        {
            if (lastGoalPosDistance < CLOSE_DISTANCE_POINT)
            {
                UponTimeGoalFinished();
                return;
            }

            // Create new time goal
            float waitTime = Random.Range(maxWaitTime, minWaitTime);
            goal = new TimeGoal(this, waitTime);
        }


        /// <summary>
        /// Creates a new position goal 
        /// </summary>
        private void UponTimeGoalFinished()
        {
            Vector3 newPos = Movement.CreateRandomPosition(maxPatrolRadius, initPos);
            lastGoalPosDistance = Vector3.Distance(lastGoalPos, newPos);
            lastGoalPos = newPos;

            // Move and assign goal
            Movement.MoveTo(newPos, AIMoveState.Walk);
            goal = new PositionGoal(this, newPos);
        }

        private void Shoot(Action onFinished = null)
        {
            Vector3 toTarget = (currentTarget.Position - transform.position).normalized;

            // Define start behaviour
            if (toShoot == 0) {
                toShoot = shotCount;
            }

            if (shootTimer.IsRunning) return;
            
            toShoot--;

            // Detect shooting end
            if (toShoot == 0)
            {
                onFinished?.Invoke();
                return;
            }
            
            // Shoot
            Quaternion aimRotation = Quaternion.LookRotation(toTarget);
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-shootOffset, shootOffset),
                Random.Range(-shootOffset, shootOffset), 0);

            Vector3 shotDir = aimRotation * spreadRotation * Vector3.forward;
            gunAudioSource.pitch = 1f + Random.Range(-0.05f, 0.05f);
            gunAudioSource.PlayOneShot(gunAudioSource.clip);

            bulletTracer.transform.rotation = Quaternion.LookRotation(shotDir);
            bulletTracer.Play();
            muzzleFlash.Play();

            shootTimer = new TickTimer(TimeUtils.FracToMilli(RateOfFirePSec), () => Shoot(onFinished));
            shootTimer.Start();

            if (Physics.Raycast(muzzleFlash.transform.position, shotDir, out var hit, Mathf.Infinity, 
                ~LayerMask.GetMask("Ignore Raycast", "Sound", "PCG")))
            {
                if (hit.collider.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    rigidbody.AddForceAtPosition(shotDir * impactForce, hit.point);
                }
                
                if (!hit.collider.TryGetComponent<SimulationAgent>(out var agent) || agent.Faction == Faction)
                {
                    return;
                }

                if (hit.collider.TryGetComponent<CharacterHealth>(out var health))
                {
                    health.Damage(rifleDamage);
                }
            }
           
        }
    }
}