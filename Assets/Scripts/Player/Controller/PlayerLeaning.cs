using Core.UserInput;
using Core.Utilities;
using UnityEngine;

namespace Player.Controller
{
    [RequireComponent(typeof(Player))]
    public class PlayerLeaning : MonoBehaviour
    {
        private float currentLean;

        private Player player;
        private Vector3 targetOffset;
        private Vector3 currentOffset;

        [SerializeField] private float leanSpeed = 3.5F;
        [SerializeField] private float maxLean = 15;

        private void Start()
        {
            player = GetComponent<Player>();
        }


        private void Update()
        {
            AddLean();
            AddRotation();
            AddOffset();
        }

        public void AddLean()
        {
            float leanLeft = InputSystem.Instance.Key(InputKeys.LEAN_LEFT) ? 1 : 0;
            float leanRight = InputSystem.Instance.Key(InputKeys.LEAN_RIGHT) ? -1 : 0;
            float dir = leanLeft + leanRight;
            float max = maxLean;
            
            // TODO: Get max from distance

            float targetLean = dir * max;
            currentLean = MathUtils.Lerp(currentLean, targetLean, leanSpeed, Time.deltaTime);
        }

        private void AddRotation()
        {
            player.Rotate(this, Vector3.forward * currentLean);
        }

        // Simulates the pivot point from the ground
        private void AddOffset()
        {
            var flatRot = Quaternion.Euler(0, transform.eulerAngles.y , 0);
            var offset = Mathf.Sin(currentLean * Mathf.Deg2Rad) * player.Physics.CurrentHeight / 2;
            targetOffset = flatRot * Vector3.right * offset;

            Vector3 delta = targetOffset - currentOffset;
            transform.position -= delta;
            currentOffset = targetOffset;
        }
    }
}