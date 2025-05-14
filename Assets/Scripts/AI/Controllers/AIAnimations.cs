using AI.Agents;
using System;
using UnityEngine;

namespace AI.Controllers
{
    [RequireComponent(typeof(Animator))]
    public class AIAnimations : MonoBehaviour
    {
        public const string DEATH_ANIM = "Death";

        private Animator animator;
        private GenericEnemyAgent agent;

        private void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<GenericEnemyAgent>();

            agent.Health.AddOnDeathHandler((object sender, EventArgs args) => PlayDeathAnimation(1f));
        }

        private void Update()
        {
            animator.SetFloat("Speed", agent.CurrentSpeed);
        }

        public void PlayDeathAnimation(float fade)
        {
            if (animator.HasState(0, Animator.StringToHash(DEATH_ANIM))) 
            { 
                animator.CrossFade(DEATH_ANIM, fade);
            }
        }
    }
}
