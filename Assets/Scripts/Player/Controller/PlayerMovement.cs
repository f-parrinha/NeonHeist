using Character.Enums;
using Core.Character;
using Core.UserInput;
using Core.Utilities.Timing;
using UnityEngine;


namespace Player.Controller
{

    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(CharacterStats))]
    public class PlayerMovement : MonoBehaviour
    {
        public const int RESET_JUMP_MS = 500;

        [Header("Speed Settings")]
        [SerializeField] private float moveImpulse = 10f;
        [SerializeField] private float crouchpeed = 2;
        [SerializeField] private float moveSpeed = 3.5f;
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 4f;
        [SerializeField] private float jumpRecoveryTime = 0.2f;

        private PlayerPhysics pPhysics;
        private PlayerStances pStances;
        private CharacterStats charStats;
        private TickTimer jumpRecoveryTimer;
        private TickTimer jumpResetTimer;
        private bool jumpLock;
        private float inputSpeed;
        private float speed;

        // State properties
        public bool IsJumpLocked => jumpLock || !jumpRecoveryTimer.IsFinished;
        public bool IsSprinting { get; private set; }


        // Speed properties
        public float CrouchSpeed => crouchpeed;
        public float MoveSpeed => moveSpeed;


        private void Start()
        {
            jumpRecoveryTimer = new TickTimer((int) (jumpRecoveryTime * 1000));
            jumpResetTimer = new TickTimer(RESET_JUMP_MS, () => ResetJump());

            var player = GetComponent<Player>();
            pPhysics = player.Physics;
            pPhysics.AddOnLandListener((_, _) => ResetJump());
            pStances = player.Stances;
            pStances.AddOnStanceChangeListener((_, args) => { if (args.NewStance == CharacterStances.Crouch) StopSprint(); });

            charStats = GetComponent<CharacterStats>();

            inputSpeed = moveSpeed;
            speed = inputSpeed;
        }

        private void Update()
        {
            AddSpeedControl();
            AddMoveControl();
            AddJumpControl();
        }

        public void StopSprint()
        {
            IsSprinting = false;
        }

        /// <summary>
        /// Sets the internal target speed value to the "move" speed, which is standard speed
        /// </summary>
        public void ResetInputSpeed()
        {
            inputSpeed = moveSpeed;
        }

        private void AddMoveControl()
        {
            Vector3 moveAxis = InputSystem.Instance.MoveAxis;

            if (moveAxis == Vector3.zero || pPhysics.IsInAir)
            {
                pPhysics.StopMove(this);
                return;
            }

            var flatRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            var direction = flatRot * moveAxis;
            pPhysics.Move(this, charStats.SpeedModifier * speed * direction, moveImpulse);   
        }


        // Adds speed control via mouse scroll and adds sprinting speed
        private void AddSpeedControl()
        {

            // Add variable speed according to height and input
            float deltaCurrentHeight = pPhysics.CurrentHeight - pStances.CrouchHeight;
            float deltaHeight = pPhysics.Height - pStances.CrouchHeight;
            float maxSpeed = Mathf.Lerp(crouchpeed, moveSpeed, deltaCurrentHeight / deltaHeight);

            speed = Mathf.Min(moveSpeed, maxSpeed);  
        }


        private void AddJumpControl()
        {
            if (InputSystem.Instance.KeyDown(InputKeys.JUMP) && pPhysics.IsGrounded && !IsJumpLocked)
            {
                jumpLock = true;

                var vel = pPhysics.Velocity;
                var jumpVel = new Vector3(vel.x, Mathf.Min(jumpForce, vel.y + jumpForce), vel.z);

                jumpResetTimer.Restart();
                pPhysics.SetUseDamping(this, false);
                pPhysics.SetVelocity(jumpVel);
                IsSprinting = false;
            }
        }

        private void ResetJump()
        {
            if (pPhysics.IsInAir) return;

            jumpLock = false;
            jumpRecoveryTimer.Restart();
            pPhysics.SetUseDamping(this, true);
        }
    }
}