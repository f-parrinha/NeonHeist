using AI.Goals;
using AI.Controllers;
using AI.Enums;
using AI.Events;
using Character;
using Core.Common.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System;
using UnityEngine;
using System.Collections.Generic;
using AI.Common;
using Core.Character;
using Core.Health.Events;
using Core.Common.Finders;

namespace AI.Agents
{
    [RequireComponent(typeof(AIMovement))]
    [RequireComponent(typeof(AIAnimations))]
    [RequireComponent(typeof(AIVision))]
    [RequireComponent(typeof(AIVoices))]
    [RequireComponent(typeof(AIEars))]
    [RequireComponent(typeof(CharacterHealth))]
    [RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(CharacterFootsteps))]
    [RequireComponent(typeof(RagdollController))]
    public abstract class GenericEnemyAgent : SimulationAgent, IInitializable, ICleanable
    {    
        private TickTask actionDecisionTask;

        protected AIMovement movement;
        protected AIAnimations animations;
        protected AIVision vision;
        protected AIVoices voices;
        protected AIEars ears;
        protected CharacterHealth health;
        protected CharacterStats stats;
        protected CharacterFootsteps footsteps;
        protected RagdollController ragdoll;

        protected Goal goal;
        protected PauseControllerFinder pauseControllerFinder; 


        [SerializeField][Range(100, 2000)] protected int refreshRate = 500;
        [SerializeField] protected List<Faction> enemyFactions;
        [SerializeField] protected List<Faction> friendlyFactions;
        [SerializeField] protected float detectionFactor = 10f;

        public AIMovement Movement => movement == null ? movement = GetComponent <AIMovement>() : movement;
        public AIAnimations Animations => animations == null ? animations = GetComponent<AIAnimations>() : animations;
        public AIVision Vision => vision == null ? vision = GetComponent<AIVision>() : vision;
        public AIVoices Voices => voices == null ? voices = GetComponent<AIVoices>() : voices;
        public AIEars Ears => ears == null ? ears = GetComponent<AIEars>() : ears;
        public CharacterHealth Health => health == null ? health = GetComponent<CharacterHealth>() : health;
        public CharacterStats Stats => stats == null ? stats = GetComponent<CharacterStats>() : stats;
        public CharacterFootsteps Footsteps => footsteps == null ? footsteps = GetComponent<CharacterFootsteps>() : footsteps;
        public RagdollController Ragdoll => ragdoll == null ? ragdoll = GetComponent<RagdollController>() : ragdoll;
        public AIState State { get; protected set; }
        public bool IsInitialized { get; protected set; }


        private void Start()
        {
            // Get components
            movement = GetComponent<AIMovement>();
            animations = GetComponent<AIAnimations>();
            vision = GetComponent<AIVision>();
            voices = GetComponent<AIVoices>();
            ears = GetComponent<AIEars>();
            health = GetComponent<CharacterHealth>();
            stats = GetComponent<CharacterStats>();
            footsteps = GetComponent<CharacterFootsteps>();
            ragdoll = GetComponent<RagdollController>();

            // Setup event handlers
            health.AddOnDeathHandler(OnDeathHandler);
            health.AddOnDamageHandler(OnDamageHandler);
            vision.AddOnTargetsUpdateHandler(OnTargetsUpdateHandler);

            // Setup state variables 
            State = AIState.Calm;
            goal = new StartGoal(this);

            // Setup tasks
            actionDecisionTask = new TickTask(refreshRate, OnActionDecision);
            actionDecisionTask.Start();

            pauseControllerFinder = new PauseControllerFinder();

            Initialize();
        }


        // To help remove memory leaks
        public virtual void CleanUp()
        {
            actionDecisionTask.Stop();
            actionDecisionTask = null;

            movement.CleanUp();
            vision.CleanUp();
            voices.CleanUp();
            health.CleanUp();
        }


        public abstract void Initialize();

        protected abstract void CalmAction();
        protected abstract void AlertAction();
        protected abstract void DangerAction();
        protected abstract void OnTargetsUpdateHandler(object sender, OnTargetsUpdateArgs args);
        protected virtual void OnDeathHandler(object sender, EventArgs args)
        {
            voices.PlayDeathVoice();
            ragdoll.Enable();
            movement.Disable();
            StopActionDecisionTask();
            CleanUp();
        }

        protected virtual void OnDamageHandler(object sender, OnHealthChangeArgs args)
        {
            voices.PlayDamageVoice();
        }

        protected void StopActionDecisionTask()
        {
            actionDecisionTask.Stop();
        }

        protected void ResumeActionDecisionTask()
        {
            actionDecisionTask.Resume();
        }

        /// <summary>
        /// Looks at the closest target it can find. If there is no target, look at default position
        /// </summary>
        /// <returns> Closets target </returns>
        protected ScannedTarget LookAtClosestTarget(params Faction[] factions)
        {
            if (vision.HasTargets)
            {
                ScannedTarget closest = vision.GetClosestTarget(factions);
                vision.SetLookAtTarget(closest.Agent.transform);
                return closest;
            }

            vision.ResetLookAtTarget();
            return null;
        }

        protected ScannedTarget InvestigateTarget(ScannedTarget target, float detectBoost = 1f)
        {
            if (target == null)
            {
                return null;
            }

            target.Detect(detectBoost * ((float) refreshRate / 1000), vision.VisionRange);
            return target;
        }

        private void OnActionDecision()
        {
            if (pauseControllerFinder.Find().IsPaused) return;
            if (!gameObject.activeSelf)
            {
                CleanUp();
                Log.Warning(this, "OnActionDecision", "This should happen at least once. More than that = memory leak!");
                return;
            }


            switch (State)
            {
                case AIState.Calm:
                    CalmAction();
                    break;
                case AIState.Alert:
                    AlertAction();
                    break;
                case AIState.Danger:
                    DangerAction();
                    break;
                default:
                    Log.Warning(this, "Decide", "Unkown AIState was given");
                    break;
            }
        }
    }
}