using UnityEngine;

namespace Props.Traps
{
    public class AnimationTrap : Trap
    {
        [SerializeField] private Animator animator;


        public override void Activate()
        {
            if (animator.GetBool("Active")) return;

            base.Activate();
            animator.SetBool("Active", true);
        }
    }
}