using Core.Utilities.Timing;
using UnityEngine;

namespace Props.Platforms
{
    public class MobilePlatformController : MonoBehaviour
    {
        [SerializeField] private float waitTime = 2f;
        [SerializeField] private float speed = 5f;
        [SerializeField] private MobilePlatform platform;
        [SerializeField] private Transform waypoint;


        // Automatic movement
        private TickTimer timer;
        private float journeyTime;
        private float journeyProgress;
        private bool arrived;
        private bool isReturning;

        public float DesiredPlatformSpeed => speed;

        private void Start()
        {
            timer = new TickTimer(TimeUtils.FracToMilli(waitTime), () =>
            {
                isReturning = !isReturning;
                arrived = false;
                journeyProgress = 0;
            });

            journeyTime = Vector3.Distance(platform.StartPosition, waypoint.position) / speed;
        }

        private void Update()
        {
            MovePlatform();
        }

        private void MovePlatform()
        {
            if (arrived) return;

            journeyProgress = Mathf.Clamp(journeyProgress + (Time.deltaTime / journeyTime), 0f, 1f);
            platform.Position = isReturning ? Vector3.Lerp(waypoint.position, platform.StartPosition, journeyProgress) :
                Vector3.Lerp(platform.StartPosition, waypoint.position, journeyProgress);

            if (journeyProgress >= 1f)
            {
                arrived = true;
                timer.Restart();
            }
        }
    }
}
