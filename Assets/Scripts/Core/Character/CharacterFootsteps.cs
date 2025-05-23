using UnityEngine;

namespace Core.Character
{
    [RequireComponent(typeof(CharacterStats))]
    public class CharacterFootsteps : MonoBehaviour
    {
        private const float MIN_SPEED = 0.05f;
        private const float PITCH_INTERVAL = 0.05f;

        private Vector3 latePos;
        private float currentSpeed;
        private float time;
        private float timeMax;

        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float referenceSpeed = 3.5f;
        [SerializeField] private float speedBoost = 1.0f;

        private void Start()
        {
            // Set state
            timeMax = CalculateTimeMax();
            time = 0;
        }

        private void Update()
        {
            currentSpeed = (transform.position - latePos).magnitude / Time.deltaTime;

            Evaluate();

            // Keep at last!
            latePos = transform.position;
        }

        private void Evaluate()
        {
            if (currentSpeed < MIN_SPEED) return;

            time += Time.deltaTime;

            if (time > timeMax)
            {
                PlayFootstep();
                timeMax = CalculateTimeMax();
                time = 0;
            }
        }

        private void PlayFootstep()
        {
            if (footstepSounds == null || footstepSounds.Length == 0) return;

            source.pitch = 1f + Random.Range(-PITCH_INTERVAL, PITCH_INTERVAL);
            source.PlayOneShot(footstepSounds[Random.Range(0,footstepSounds.Length)]);
        }

        private float CalculateTimeMax()
        {
            float speedFactor = currentSpeed == 0 ? 1 : (currentSpeed / referenceSpeed);
            float speedFactorRebalanced = Mathf.Pow(speedFactor, speedBoost);
            return 1 / (frequency * speedFactorRebalanced);
        }
    }
}