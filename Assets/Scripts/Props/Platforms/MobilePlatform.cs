using UnityEngine;

namespace Props.Platforms
{
    [RequireComponent(typeof(AudioSource))]
    public class MobilePlatform : MonoBehaviour
    {
        [SerializeField] private MobilePlatformController controller;
        [SerializeField] private float maxVolume = 0.5f;

        private AudioSource source;
        private Vector3 lastPos;
        private float currentSpeed;

        public Vector3 StartPosition { get; private set; }
        public Vector3 Position { get => transform.position; set => transform.position = value; }

        private void Start()
        {
            source = GetComponent<AudioSource>();

            StartPosition = transform.position;
            lastPos = transform.position;
        }

        private void Update()
        {
            currentSpeed = (transform.position - lastPos).magnitude / Time.deltaTime;

            ControlMoveVolume();

            lastPos = transform.position;
        }

        private void ControlMoveVolume()
        {
            source.volume =  Mathf.Min(currentSpeed / controller.DesiredPlatformSpeed, maxVolume);
        }
    }
}