using Core.Common.Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Class <c> PlayerPhysics </c> has the responsability of handling forces: internal, like gravity or friction; and external
    ///     like, an explosion or an outside move command.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerPhysics : MonoBehaviour
    {
        public const RigidbodyConstraints CONSTRAINTS = RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;

        // Bool queues to change "Use" state of properties
        private BoolQueue gravityBool;
        private BoolQueue frictionBool;
        private BoolQueue dampingBool;
        private Dictionary<object, MoveEntry> moveQueue;
        
        private CapsuleCollider capsuleCollider;
        private Rigidbody rb;
        private float currentHeight;

        [Header("General Settings")]
        [SerializeField] private float friction = 2f;
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private float gravityBooster = 2f;
        [Header("Sliding Settings")]
        [SerializeField] private float slideAngle = 30f;
        [SerializeField] private float slideForce = 5f;
        [Header("Body Settings")]
        [SerializeField] private float height = 2f;
        [SerializeField] private float stepHeight = 0.5f;
        [Header("Spring Settings")]
        [SerializeField] private float springStiffness = 60f;
        [SerializeField] private float springDamping = 20f;

        private event EventHandler<OnLandArgs> OnLand;

        // Use params properties
        public bool UseGravity => gravityBool.Evaluate();
        public bool UseFriction => frictionBool.Evaluate();
        public bool UseDamping => dampingBool.Evaluate();

        // Physics properties
        public float Gravity => gravity;
        public float Friction => friction;
        public Vector3 Velocity => rb.linearVelocity;
        public float Speed => Vector3.Magnitude(rb.linearVelocity);
        public float  Height => height;
        public float CurrentHeight => currentHeight;
        public float Radius => capsuleCollider.radius;
        public Vector3 FeetPosition => transform.position - Vector3.up * currentHeight / 2;
        public float SpringStiffness { get => springStiffness;  set => springStiffness = value; }
        public float SpringHeight => currentHeight / 2;
        public float SpringDamping { get => springDamping; set => springDamping = value; }

        // State properties
        public bool IsSliding { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsInAir => !IsGrounded;


        private void Awake()
        {
            frictionBool = new BoolQueue();
            gravityBool = new BoolQueue();
            dampingBool = new BoolQueue();
            moveQueue = new Dictionary<object, MoveEntry>();

            rb = GetComponent<Rigidbody>();
            rb.constraints = CONSTRAINTS;
            rb.useGravity = false;

            capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.material = CreatePhysicsMat();

            frictionBool.Set(this, true);
            gravityBool.Set(this, true);
            dampingBool.Set(this, true);

            SetHeight(height);
        }


        private void FixedUpdate()
        {
            ApplyGrounding(out var groundNormal);
            ApplyFriction(groundNormal);
            ApplySlopeSlide(groundNormal);
            ApplyGravity();

            EvaluateMoveQueue(groundNormal);
        }


        public static PhysicsMaterial CreatePhysicsMat() => new()
        {
            bounciness = 0,
            staticFriction = 0,
            dynamicFriction = 0,
            frictionCombine = PhysicsMaterialCombine.Minimum,
            bounceCombine = PhysicsMaterialCombine.Minimum,
        };

        public void AddForce(Vector3 force, ForceMode mode) => rb.AddForce(force, mode);
        public void SetUseFriction(object setter, bool useFriction) => frictionBool.Set(setter, useFriction);
        public void SetUseGravity(object setter, bool useGravity) =>  gravityBool.Set(setter, useGravity);
        public void SetUseDamping(object setter, bool useDamping) => dampingBool.Set(setter, useDamping);
        public void SetVelocity(Vector3 velocity) => rb.linearVelocity = velocity;
        public void SetHeight(float height)
        {
            float heightOffset = stepHeight / 2;

            currentHeight = height;

            capsuleCollider.height = height - stepHeight;
            capsuleCollider.center = new(0, heightOffset, 0);
        }

        public void Move(object mover, Vector3 targetVelocity, float impulse = 0)
        {
            if (moveQueue.ContainsKey(mover))
            {
                var entry = moveQueue[mover];
                entry.Set(targetVelocity, impulse);
                return;
            }
            
            moveQueue.Add(mover, new MoveEntry()
            {
                Velocity = targetVelocity,
                Impulse = impulse
            });
        }

        public void StopMove(object mover)
        {
            if (!moveQueue.ContainsKey(mover)) return;
            
            moveQueue.Remove(mover);
        }



        private void ApplyGravity()
        {
            if (UseGravity)
            {
                float boost = rb.linearVelocity.y < 0 && IsInAir ? gravityBooster : 0f;
                rb.AddForce(new(0, -rb.mass * (gravity + boost), 0));
            }
        }

        /// <summary>
        /// Updates IsGrounded status, evaluates ground events (e.g. OnLand) and retrieves the current ground normal
        /// </summary>
        /// <param name="groundNormal"> out variable for the current ground normal vector </param>
        private void ApplyGrounding(out Vector3 groundNormal)
        {
            groundNormal = Vector3.up;
            var isGroundedOld = IsGrounded;

            if (IsGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, SpringHeight, ~0, QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;

                // Check if landed
                if (isGroundedOld != IsGrounded) RaiseOnLand(Velocity);

                float relVel = Vector3.Dot(groundNormal, rb.linearVelocity);
                float relDist = SpringHeight - hit.distance;
                float stiffness = relDist * springStiffness;
                float damping = relVel * (UseDamping ? springDamping : 0f);
                rb.AddForce(rb.mass * Vector3.up * (stiffness - damping));
            }
            else
            {
                transform.parent = null;
            }
        }

        private void ApplyFriction(Vector3 groundNormal)
        {
            if (UseFriction && IsGrounded && IsMoveQueueFree() && friction > 0)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = 0;

                var projection = Quaternion.FromToRotation(Vector3.up, groundNormal);
                var force = GetForceForVelocity(vel, Vector3.zero, friction);
                rb.AddForce(projection * force);
            }
        }

        
        private void ApplySlopeSlide(Vector3 groundNormal)
        {
            float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

            if (IsSliding = slopeAngle >= slideAngle)
            {
                var slideDir = groundNormal;
                slideDir.y = -slideDir.y;

                rb.AddForce(rb.mass * slideDir * slideForce);
            }
        }


        private void EvaluateMoveQueue(Vector3 groundNormal)
        {
            if (moveQueue.Count == 0) return;

            var force = Vector3.zero;
            var projection = Quaternion.FromToRotation(Vector3.up, groundNormal);
            var vel = rb.linearVelocity;
            vel.y = 0;

            foreach (var (_, move) in moveQueue)
            {
                force += GetForceForVelocity(vel, move.Velocity, move.Impulse);
            }

            rb.AddForce(projection * force);
        }

        private bool IsMoveQueueFree()
        {
            if (moveQueue.Count == 0) return true;

            foreach (var (_, move) in moveQueue)
            {
                if (!move.IsFree) return false;
            }

            return true;
        }

        // Calculates the force to move the player with that velocity. Clamps the force if impulse is given
        private Vector3 GetForceForVelocity(Vector3 currentVel, Vector3 targetVel, float impulse = 0)
        {
            var velDiff = targetVel - currentVel;
            var force = rb.mass * velDiff / Time.fixedDeltaTime;
            return impulse == 0 ? force : Vector3.ClampMagnitude(force, rb.mass * impulse);
        }



        public void AddOnLandListener(EventHandler<OnLandArgs> listener) => OnLand += listener;
        protected void RaiseOnLand(Vector3 velocity) => OnLand?.Invoke(this, new OnLandArgs { Velocity = velocity });


        private class MoveEntry
        {
            public const float IS_FREE_THRESHOLD = 0.001f;

            public Vector3 Velocity { get; set; }
            public float Impulse { get; set; }
            public bool IsFree => (Velocity - Vector3.zero).sqrMagnitude <= (IS_FREE_THRESHOLD * IS_FREE_THRESHOLD);

            public void Set(Vector3 newVel, float newImpulse)
            {
                Velocity = newVel;
                Impulse = newImpulse;
            }
        }

        public class OnLandArgs : EventArgs
        {
            public Vector3 Velocity;
        }
    }
}
