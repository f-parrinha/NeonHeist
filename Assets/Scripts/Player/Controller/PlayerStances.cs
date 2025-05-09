using Character.Enums;
using Core.UserInput;
using Core.Utilities;
using System;
using UnityEngine;

namespace Player.Controller
{
    public class PlayerStances : MonoBehaviour
    {
        public const float MIN_CROUCH_HEIGHT = 0.5f;

        private PlayerPhysics pPhysics;
        private PlayerMovement pMovement;
        private float targetHeight;
        private float maxHeight;

        [SerializeField] private float crouchHeight = 1.5f;
        [SerializeField] private float crouchingSpeed = 5f;

        private EventHandler<OnStanceChangeArgs> onStanceChange;

        public CharacterStances Stance { get; private set; }
        public float CrouchHeight => crouchHeight;


        private void Start()
        {
            var player = GetComponent<Player>();
            pPhysics = player.Physics;
            pMovement = player.Movement;

            Stance = CharacterStances.Stand;
            crouchHeight = Mathf.Max(crouchHeight, MIN_CROUCH_HEIGHT);
            targetHeight = pPhysics.Height;
        }

        private void Update()
        {
            UpdateStance();
            UpdateHeight();
            UpdateMaxHeight();
            AddToggleCrouch();
        }

        public void SetStance(CharacterStances stance)
        {
            switch (stance)
            {
                case CharacterStances.Stand:
                    targetHeight = pPhysics.Height;
                    pMovement.ResetInputSpeed();
                    break;
                case CharacterStances.Crouch:
                    targetHeight = crouchHeight;
                    break;
                default:
                    targetHeight = pPhysics.Height;
                    break;
            }
        }


        private void AddToggleCrouch()
        {
            if (InputSystem.Instance.KeyUp(InputKeys.CROUCH))
            {
                SetStance(Stance == CharacterStances.Crouch ? CharacterStances.Stand : CharacterStances.Crouch);
            }
        }



        private void UpdateStance()
        {
            var oldStance = Stance;
            var crouchDelta = maxHeight - crouchHeight;
            var isInCrouchThreshold = pPhysics.CurrentHeight < crouchHeight + crouchDelta / 2;
            Stance = isInCrouchThreshold ? CharacterStances.Crouch : CharacterStances.Stand;

            // Raise event
            if (oldStance != Stance)
            {
                RaiseOnStanceChange(new OnStanceChangeArgs { OldStance = oldStance, NewStance = Stance });
            }
        }

        private void UpdateMaxHeight()
        {
            maxHeight = pPhysics.Height;

            if (Physics.SphereCast(transform.position, pPhysics.Radius, Vector3.up, out var hit, pPhysics.Height / 2))
            {
                maxHeight = hit.point.y;
            } 
        }

        private void UpdateHeight()
        {

            float direction = pPhysics.IsGrounded ? 1 : -1;
            float prevHeight = pPhysics.CurrentHeight;
            float height = MathUtils.Lerp(prevHeight, targetHeight, crouchingSpeed, Time.deltaTime);

            /*
             * Updates the position of the player according how much height has changed, simulating
             *  anchor points
             * 
             * Direction is used because, when the player is grounded, the torso goes up, while the legs
             *  pull the body up, but when it is in the air, the legs go up. The head remains at the same position
             */

            transform.Translate((height - prevHeight) * direction * Vector3.up / 2);
            pPhysics.SetHeight(height);
        }


        public void AddOnStanceChangeListener(EventHandler<OnStanceChangeArgs> listener) => onStanceChange += listener;
        protected void RaiseOnStanceChange(OnStanceChangeArgs args) => onStanceChange?.Invoke(this, args);

        
        // Classes

        public class OnStanceChangeArgs : EventArgs
        {
            public CharacterStances NewStance;
            public CharacterStances OldStance;
        }
    }
}