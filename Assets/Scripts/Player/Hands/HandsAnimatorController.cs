using Core.UserInput;
using UnityEngine;

namespace Player.Controller 
{
    public class HandsAnimatorController : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float maxAnimSpeed = 2f;

        private PlayerPhysics pPhysics;
        private PlayerMovement pMovement;
        public Animator Animator { get; private set; }

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
    }
}

