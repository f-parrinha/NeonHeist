using AI.Common;
using AI.Controllers;
using AI.Enums;
using AI.Events;
using AI.Goals;
using Core.Common;
using Core.Health.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using UnityEngine;

namespace AI.Agents
{
    /// <summary>
    /// TODO: Continuous investigation on all targets - right now, it only investigates the closest one
    /// </summary>
    public class SecurityGuardAgent : GenericEnemyAgent
    {
        private const float CLOSE_DISTANCE_POINT = 3f;
        private const float HIT_SOUND_VOLUME = 0.5f;

        private Vector3 initPos;

        // When the AI decides do to go to a close distance point, it does not wait in that position
        private float lastGoalPosDistance;
        private ScannedTarget currentTarget;
        private Vector3 lastGoalPos;

        // Only activates damage collider on the katana for some time. The task is used for that
        private TickTask attackTask;

        [SerializeField] private float alertDetectionFactor = 15f;
        [Header("Patrolling Settings")]
        [SerializeField] private float maxPatrolRadius = 5;
        [SerializeField] private float maxWaitTime = 5f;
        [SerializeField] private float minWaitTime = 2f;
        [Header("Attack Settings")]
        [SerializeField] private float attackDelay = 0.5f;
        [SerializeField] private float attackRadius = 0.5f;
        [SerializeField] private float attackDistance = 0.5f;
        [SerializeField] private Transform attackStartPivot;
        [SerializeField] private float damage = 20f;
        [Header("Sound Settings")]
        [SerializeField][Range(0f, 1f)] private float calmSoundProbability = 0.2f;
        [SerializeField] private AudioClip[] attackHitSounds;

        public override void Initialize()
        {
            initPos = transform.position;
            lastGoalPos = initPos;
            lastGoalPosDistance = 0;

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
            if (closestTarget != null && closestTarget.DetectionLevel >= 50)
            {
                voices.PlayAlertVoice();
                State = AIState.Alert;
                currentTarget = closestTarget;
                movement.MoveTo(currentTarget.Position, AIMoveState.Walk);
                goal = new PositionGoal(this, currentTarget.Position);
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

            // State transition on detection
            if (closestTarget != null && closestTarget.DetectionLevel >= 100)
            {
                Voices.PlayDangerVoice();
                currentTarget = closestTarget;
                State = AIState.Danger;
                movement.MoveTo(currentTarget.Position, AIMoveState.Run);
                goal = new PositionGoal(this, currentTarget.Position);
                return;
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
            if (goal.Evaluate() && !animations.IsAnimationPlaying(AIAnimations.COMBAT_LAYER, AIAnimations.ATTACK_ANIM))
            {
                animations.PlayAttackAnimation(0.1f);
                voices.PlayAttackVoice();

                // Setup damage collider for melee weapon
                attackTask?.Stop();
                attackTask = new TickTimer(TimeUtils.FracToMilli(attackDelay), UponAttackMelee);
                attackTask.Start();
            }

            goal = new PositionGoal(this, currentTarget.Position);
            movement.MoveTo(currentTarget.Position, AIMoveState.Run);
            animations.SetCombatLayerWeight(1);
        }


        protected override void OnTargetsUpdateHandler(object sender, OnTargetsUpdateArgs args)
        {
            // Change to Alert state and lose track of the current target
            if (currentTarget != null && args.LostTargets.Contains(currentTarget))
            {
                State = AIState.Alert;
                goal = new PositionGoal(this, currentTarget.Position);
                movement.MoveTo(currentTarget.Position, AIMoveState.Walk);
                currentTarget = null;
            }
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

        private void UponAttackMelee()
        {
            if (currentTarget == null) return;
            
            Vector3 dir = currentTarget.Position - transform.position;

            if (Physics.SphereCast(transform.position, attackRadius, dir, out var hit, attackDistance))
            {
                if (attackHitSounds.Length > 0)
                {
                    var audioSource = AudioUtils.CreateAudio(hit.point, attackHitSounds[Random.Range(0, attackHitSounds.Length)]);
                    audioSource.volume = HIT_SOUND_VOLUME;
                }

                if (hit.collider.TryGetComponent<IHealthHolder>(out var healthHolder))
                {
                    healthHolder.Damage(damage);
                }
            }
        }
    }
}