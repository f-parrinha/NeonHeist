using AI.Agents;
using AI.Common;
using AI.Enums;
using Core.Common.Interfaces;
using Core.Hacking.Interfaces;
using Core.Utilities.Timing;
using UnityEngine;

namespace Props
{
    /// <summary>
    /// Class <c> Security Camera </c> defines the behaviour of a security camera trying to catch enemies (in this case, the Player :)).
    ///     May be expanded to support other factions!
    [RequireComponent(typeof(SoundEmmitter))]
    public class SecurityCamera : MonoBehaviour, IEnableable
    {
        public const Faction FACTION = Faction.Enemy;
        private const int SCAN_LEFT_DIR = -1;
        private const int SCAN_RIGHT_DIR = 1;

        [Header("General Settings")]
        [SerializeField] private int resetAlarmTime = 5000;
        [SerializeField] private float startXRotatation = 50;
        [SerializeField] private float coneAngle = 40;
        [Header("Scan Settings")]
        [SerializeField] private float yScanRange = 50;
        [SerializeField] private float range = 10;
        [SerializeField] private float scanSpeed = 15f;
        [SerializeField] private int waitTime = 4000;
        [SerializeField] private int scanDeltaTime = 500;
        [Header("Light Settings")]
        [SerializeField] private Light spotlight;
        [SerializeField] private Light pointLight;
        [SerializeField] private Color okColor = Color.green;
        [SerializeField] private Color dangerColor = Color.red;
        [SerializeField] private Color disabledColor = Color.yellow;

        private float initYRotation;
        private float currentScanY;
        private int scanDir;
        private TickTimer waitTimer;
        private TickTimer resetAlarm;
        private TickTask scanTask;
        private Transform currentTarget;
        private SoundEmmitter soundEmitter;

        public bool HasTarget => currentTarget != null;
        public bool IsEnabled { get; protected set; }
        public IHackSystem HackSystem => throw new System.NotImplementedException();


        /* --------- UNITY METHODS --------- */

        private void Start()
        {
            // Get componets
            soundEmitter = GetComponent<SoundEmmitter>();

            // Setup lights 
            pointLight.range = 0.5f;
            pointLight.intensity = 1f;
            spotlight.range = range;
            spotlight.spotAngle = coneAngle;
            ChangeLightsColor(okColor);

            // Setup timers
            waitTimer = new TickTimer(waitTime, ChangeScanDirection);
            resetAlarm = new TickTimer(resetAlarmTime, () => { soundEmitter.Disable(); Enable(); });
            scanTask = new TickTask(scanDeltaTime, ScanTargets);
            scanTask.Start();
            
            // Random start direction
            scanDir = Random.Range(0, 2) == 0 ? SCAN_LEFT_DIR : SCAN_RIGHT_DIR;
            initYRotation = transform.rotation.eulerAngles.y;

            // Start enabled by default
            Enable();
        }

        private void Update()
        {
            // Perform runtime os only when enabled
            if (!IsEnabled) return;

            float targetY = GetTargetYRotation();
            RefreshScanDir(targetY);

            currentScanY = Mathf.MoveTowards(currentScanY, targetY, scanSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(startXRotatation, initYRotation + currentScanY, 0);
        }

        /* --------- PUBLIC METHODS --------- */

        public void Enable()
        {
            if (IsEnabled) return;

            ChangeLightsColor(okColor);
            spotlight.enabled = true;
            IsEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled) return;

            pointLight.color = disabledColor;
            spotlight.enabled = false;
            IsEnabled = false;
        }

        public void Alarm()
        {
            Disable();
            soundEmitter.Enable();
            resetAlarm.Restart();
        }


        /* --------- PRIVATE METHODS --------- */

        private float GetTargetYRotation()
        {
            if (HasTarget)
            {
                float angleToTarget = Quaternion.LookRotation(currentTarget.position - transform.position).eulerAngles.y;
                float relativeTargetY = Mathf.DeltaAngle(initYRotation, angleToTarget);
                return Mathf.Clamp(relativeTargetY, -yScanRange, yScanRange);
            }
            else
            {
                return yScanRange * scanDir;
            }
        }

        private void RefreshScanDir(float targetY)
        {
            if (waitTimer.IsRunning || HasTarget) return;
            if (Mathf.Abs(targetY - currentScanY) < 1)
            {
                waitTimer.Restart();
            }
        }

        private void ChangeScanDirection()
        {
            scanDir *= -1;
        }

        private void ChangeLightsColor(Color color)
        {
            pointLight.color = color;
            spotlight.color = color;
        } 

        private void ScanTargets()
        {
            if (!IsEnabled) return;

            Collider[] colliders = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Player", "Agent"));

            foreach (var collider in colliders)
            {
                Vector3 dir = collider.transform.position - transform.position;

                // Is it withing the cone?
                if (Vector3.Angle(transform.forward, dir) > coneAngle / 2) continue;

                // Check for obstruction - is there a wall blocking the view?
                if (Physics.Raycast(transform.position, dir, out var hit, range, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawRay(transform.position, dir * range, Color.yellow, 1f);
                    if (hit.collider.TryGetComponent<SimulationAgent>(out var agent))
                    {
                        if (agent.Faction == FACTION) continue;

                        soundEmitter.Enable();
                        currentTarget = hit.transform;
                        ChangeLightsColor(dangerColor);
                        return;
                    }
                }
            }

            // Did not meet any targets...
            soundEmitter.Disable();
            currentTarget = null;
            ChangeLightsColor(okColor);
        }
    }
}