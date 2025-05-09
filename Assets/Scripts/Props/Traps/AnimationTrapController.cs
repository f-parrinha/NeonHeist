
using UnityEngine;

namespace Props.Traps
{

    public class TrapAnimationController : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Spikes_On"))
            {
                animator.SetBool("Active", false);
            }
        }
    }
}