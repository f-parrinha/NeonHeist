using AI.Common;
using AI.Controllers;
using AI.Enums;
using AI.Events;
using Character;
using Core.Common.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Agents
{
    [RequireComponent(typeof(AIMovement))]
    [RequireComponent(typeof(AIAnimations))]
    [RequireComponent(typeof(AIVision))]
    [RequireComponent(typeof(CharacterHealth))]
    [RequireComponent(typeof(CharacterVoices))]
    public abstract class GenericEnemyAgent : MonoBehaviour, IInitializable, ICleanable
    {    
        private TickTask actionDecisionTask;

        protected AIMovement movement;
        protected AIAnimations animations;
        protected AIVision vision;
        protected CharacterHealth health;
        protected CharacterVoices voices;
        protected Goal goal;

        [SerializeField][Range(100, 2000)] private int refreshRate = 500;

        public AIMovement Movement => movement == null ? GetComponent <AIMovement>() : movement;
        public AIAnimations Animations => animations == null ? GetComponent<AIAnimations>() : animations;
        public AIVision Vision => vision == null ? GetComponent<AIVision>() : vision;
        public CharacterHealth Health => health == null ? GetComponent<CharacterHealth>() : health;
        public CharacterVoices Voices => health == null ? GetComponent<CharacterVoices>() : voices;
        public AIState State { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public float CurrentSpeed { get; private set; }


        private void Start()
        {
            // Get components
            movement = GetComponent<AIMovement>();
            animations = GetComponent<AIAnimations>();
            vision = GetComponent<AIVision>();
            health = GetComponent<CharacterHealth>();
            voices = GetComponent<CharacterVoices>();

            // Setup event handlers
            health.AddOnDeathHandler(OnDeathHandler);
            vision.AddOnTargetFoundHandler(OnTargetFoundHandler);
            vision.AddOnTargetLostHandler(OnTargetLostHandler);

            // setup state variables 
            State = AIState.Calm;
            goal = new Goal(this);
            goal.Set(transform.position);

            // Setup tasks
            actionDecisionTask = new TickTask(refreshRate, OnActionDecision);
            actionDecisionTask.Start();

            
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
        protected abstract void DangerAction();
        protected abstract void OnTargetFoundHandler(object sender, OnTargetFoundArgs args);
        protected abstract void OnTargetLostHandler(object sender, OnTargetLostArgs args);
        protected virtual void OnDeathHandler(object sender, EventArgs args)
        {
            CleanUp();
        }

        protected void StopActionDecisionTask()
        {
            actionDecisionTask.Stop();
        }

        protected void ResumeActionDecisionTask()
        {
            actionDecisionTask.Resume();
        }


        private void OnActionDecision()
        {
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
                    // TODO
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