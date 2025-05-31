using Core.Utilities;
using UnityEngine;

namespace AI.Common
{
    public class AISoundEmitter : MonoBehaviour
    {
        private float RESIZE_SMOOTHNESS = 5f;

        [Header("General Settings")]
        [SerializeField] private SphereCollider rangeCollider;
        [SerializeField] private AudioSource source;
        [SerializeField] private float rangeOffset = 5;
        [SerializeField] private float range;
        [SerializeField] private bool startOnAwake;
        [Header("Speed Based Settings")]
        [SerializeField] private bool useSpeed;
        [SerializeField] private float baseSpeed = 3.5f;
        [Header("Ground Based Settings")]
        [SerializeField] private bool checkIsGrounded;
        [SerializeField] private float isGroundedRayLength;

        private Vector3 lastPos;

        public float CurrentSpeed { get; private set; }
        public bool IsEnabled { get; private set; }

        private void Start()
        {
            // Setup range collider
            rangeCollider.isTrigger = true;
            rangeCollider.radius = range - rangeOffset;
            
            // Setup audio source
            source.maxDistance = range;
            source.spatialBlend = 1f;
            source.playOnAwake = false;

            lastPos = rangeCollider.transform.position;

            // Start state
            if (startOnAwake)
            {
                IsEnabled = false;
                Play();
            }
            else
            {
                IsEnabled = true;
                Stop();
            }

            // Check collider if is sound layer - GetMask return 1000... where 1 one was shifted .layer times to the left
            if (1 << rangeCollider.gameObject.layer != LayerMask.GetMask(LayerNames.SOUND))
            {
                Log.Warning(this, "Start", "Range Collider's GameObject is not of layer Sound");
            }
        }

        private void Update()
        {
            CurrentSpeed = (rangeCollider.transform.position - lastPos).magnitude / Time.deltaTime;

            float isGroundedFactor = GetIsGroundedFactor();
            float speedFactor = GetSpeedFactor();

            // Update range
            float nextRange = range * speedFactor * isGroundedFactor;
            rangeCollider.radius = Mathf.Lerp(rangeCollider.radius, Mathf.Clamp(nextRange - rangeOffset, 0, nextRange), RESIZE_SMOOTHNESS * Time.deltaTime);
            source.maxDistance = Mathf.Lerp(source.maxDistance, nextRange, RESIZE_SMOOTHNESS * Time.deltaTime);

            // Keep at last!
            lastPos = rangeCollider.transform.position;
        }

        public void Play()
        {
            if (!IsEnabled)
            {
                IsEnabled = true;
                rangeCollider.enabled = true;
                source.Play();
            }
        }

        public void Stop()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                rangeCollider.enabled = false;
                source.Stop();
            }
        }

        private float GetSpeedFactor()
        {
            if (useSpeed)
            {
                return CurrentSpeed / baseSpeed;
            } 
            else
            {
                return 1f;
            }
        }

        private float GetIsGroundedFactor()
        {
            if (checkIsGrounded)
            {
                if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, isGroundedRayLength, ~0, QueryTriggerInteraction.Ignore))
                {
                    return 1f;
                }

                // In the air...
                return 0f;
            }

            // Does not check the ground
            return 1f;
        }
    }
}