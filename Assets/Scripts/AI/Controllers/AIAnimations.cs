using AI.Agents;
using Core.Utilities;
using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace AI.Controllers
{
    [RequireComponent(typeof(Animator))]
    public class AIAnimations : MonoBehaviour
    {
        public const string DEATH_ANIM = "Death";
        public const string ATTACK_ANIM = "Attack";
        public const string COMBAT_IDLE_ANIM = "Combat Idle";
        public const int COMBAT_LAYER = 1;
        public const int BASE_LAYER = 0;

        private Animator animator;
        private GenericEnemyAgent agent;
        private float combatLayerWeight;

        [SerializeField] private float combatLayerSmoothness = 2f;

        private void Start()
        {
            // Setup components
            animator = GetComponent<Animator>();
            agent = GetComponent<GenericEnemyAgent>();

            // Setup event handlers
            agent.Health.AddOnDeathHandler((object sender, EventArgs args) => PlayDeathAnimation(1f));
        }

        private void Update()
        {
            animator.SetFloat("Speed", agent.Movement.CurrentSpeed);

            RefreshLayerWeights();
        }

        public void SetCombatLayerWeight(float w)
        {
            combatLayerWeight = w;
        }

        public void PlayDeathAnimation(float fade = 1)
        {
            if (animator.HasState(BASE_LAYER, Animator.StringToHash(DEATH_ANIM)))
            {
                animator.CrossFade(DEATH_ANIM, fade);
            }
        }

        public void PlayAttackAnimation(float fade = 1)
        {
            if (animator.HasState(COMBAT_LAYER, Animator.StringToHash(ATTACK_ANIM))) 
            {
                animator.CrossFade(COMBAT_IDLE_ANIM, fade);
                animator.CrossFade(ATTACK_ANIM, fade);
            }
        }


        public bool IsAnimationPlaying(int layer, string animationName)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            int animHash = Animator.StringToHash(animationName);
            return stateInfo.shortNameHash == animHash && stateInfo.normalizedTime < 1f;
        }

        private void RefreshLayerWeights()
        {
            float currentCombatLayerWeight = animator.GetLayerWeight(COMBAT_LAYER);
            currentCombatLayerWeight = MathUtils.Lerp(currentCombatLayerWeight, this.combatLayerWeight, combatLayerSmoothness, Time.deltaTime);
            animator.SetLayerWeight(COMBAT_LAYER, currentCombatLayerWeight);
        }
    }
}
