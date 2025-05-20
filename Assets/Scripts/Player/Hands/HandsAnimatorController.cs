using Core.Common.Interfaces;
using Core.UserInput;
using UnityEngine;

namespace Player.Controller 
{
    /// <summary>
    /// Class <c> HandsAnimatorController </c> handles the generic animator in the hands slot object, applying the same
    ///     animations for all kinds of weapons used by the player. Mainly for moving, jumping, landing, and other types of movement related animations
    /// </summary>
    public class HandsAnimatorController : MonoBehaviour, IZoomable
    {
        [SerializeField] private Player player;
        [SerializeField] private float maxAnimSpeed = 2f;

        private PlayerPhysics pPhysics;
        private PlayerMovement pMovement;
        public Animator Animator { get; private set; }
        public bool IsZooming { get; private set; }

        private void Start()
        {
            Animator = GetComponent<Animator>();

            pPhysics = player.Physics;
            pMovement = player.Movement;
       }


        private void Update()
        {
            bool isMoving = InputSystem.Instance.IsMoving;
            bool isGrounded = pPhysics.IsGrounded;
            Animator.SetBool("IsGrounded", pPhysics.IsGrounded);
            Animator.SetBool("IsMoving", isMoving);
            Animator.SetBool("IsZooming", IsZooming);


            if (InputSystem.Instance.IsMoving)
            {
                Animator.speed = Mathf.Min(pPhysics.Speed / pMovement.MoveSpeed, maxAnimSpeed);
            } else
            {
                Animator.speed = 1;
            }
        }

        public void PlayHolsterGet(float fade)
        {
            Animator.CrossFade("HolsterGet", fade);
        }

        public void PlayHolsterPut(float fade)
        {
            Animator.CrossFade("HolsterPut", fade);
        }

        public void Zoom()
        {
            IsZooming = true;
        }

        public void Unzoom()
        {
            IsZooming = false;
        }
    }
}

