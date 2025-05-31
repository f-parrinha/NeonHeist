using Core.Common.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Character
{
    /// <summary>
    /// Class <c> RagdollController </c> handles ragdoll parts, enabling and disabling them, and as consequence, controlls the entire regdoll system for a character
    /// 
    /// <para>
    ///     Ragdoll parts should be children of the controller. Preferably, the controller is in the character's parent gameObject
    /// </para>
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class RagdollController : MonoBehaviour, IEnableable
    {
        private Animator animator;
        private RagdollPart[] ragParts;

        public bool IsEnabled { get; private set; }


        private void Start()
        {
            // Get components
            ragParts = gameObject.GetComponentsInChildren<RagdollPart>();
            animator = gameObject.GetComponent<Animator>();

            // By default, disable ragdolling
            Enable();
            Disable();
        }

        /// <summary>
        /// Enables the ragdolling system. Animator is off at this stage
        /// </summary>
        public void Enable()
        {
            if (IsEnabled) return;

            animator.enabled = false;
            foreach (RagdollPart part in ragParts)
            {
                part.Enable();
            }

            IsEnabled = true;
        }

        /// <summary>
        /// Disables the ragdolling system. Animator is on at this stage
        /// </summary>
        public void Disable()
        {
            if (!IsEnabled) return;

            animator.enabled = true;
            foreach (RagdollPart part in ragParts)
            {
                part.Disable();
            }

            IsEnabled = false;
        }
    }
}
