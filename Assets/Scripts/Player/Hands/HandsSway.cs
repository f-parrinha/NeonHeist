using Core.Common.Queue;
using Core.UserInput;
using Player.Controller;
using UnityEngine;

namespace Player.Controller
{
    public class HandsSway : MonoBehaviour
    {
        private const float RotToPosRescaler = 0.02f;

        [Header("General Settings")]
        [SerializeField] private Transform swayPivot;
        [SerializeField] private Player player;
        [Header("Move Sway Settings")]
        [SerializeField] private float moveSway;
        [SerializeField] private float moveSwaySpeed;
        [SerializeField] private float maxSpeedFactor = 1.5f;
        [SerializeField] [Range(0f, 1f)] private float zoomFactor = 0.2f;
        [Header("Mouse Sway Settings")]
        [SerializeField] private float mouseSway;
        [SerializeField] private float mouseSwayZ;
        [SerializeField] private float mouseSwayMax;
        [SerializeField] private float mouseSwayDeceleration;

        private PlayerPhysics pPhysics;
        private PlayerMovement pMovement;
        private BoolQueue invertDirQueue;
        private Vector3 swayPivotInitPos;
        private Vector3 currentMouseSway; 
        private Vector3 currentMoveSway;
        private Vector3 currentRotationMoveSway;

        protected float SwayIntensity { get; set; } = 1.0f;
        protected float LimitIntensity { get; set; } = 1.0f;
        public bool InvertDirection => invertDirQueue.Evaluate();

        private void Start()
        {
            invertDirQueue = new BoolQueue();

            pPhysics = player.Physics;
            pMovement = player.Movement;

            swayPivotInitPos = swayPivot.transform.localPosition;
        }

        private void Update()
        {
            swayPivot.transform.localPosition = swayPivotInitPos + GetMoveSway();
            swayPivot.transform.localRotation = Quaternion.Euler(GetMouseSway() + GetRotationMoveSway());
        }

        public void SetInvertDirection(object setter, bool value)
        {
            invertDirQueue.Set(setter, value);
        }
        public void UnsetInvertDirection(object setter)
        {
            invertDirQueue.Unset(setter);
        }

        /** Adds translation sway based on keyboard input */
        private Vector3 GetMoveSway()
        {
            var x = InputSystem.Instance.MoveAxis.x;
            var z = InputSystem.Instance.MoveAxis.z; z *= InputSystem.Instance.MoveAxis.z < 0 ? 0.5f : 1;   // Add more sway when moving forward
            var y = InputSystem.Instance.MoveAxis.z; y *= y > 0 ? 0.5f : 0;
            var zoomFactor = player.GunController.IsZooming ? this.zoomFactor : 1f;
            var factor = Mathf.Min(pPhysics.Speed / pMovement.MoveSpeed, maxSpeedFactor);
            var sway = pPhysics.IsGrounded ? zoomFactor * factor * moveSway * RotToPosRescaler *  new Vector3(x, -y, -z) : Vector3.zero;

            currentMoveSway = Vector3.Lerp(currentMoveSway, sway, moveSwaySpeed * Time.deltaTime);
            return currentMoveSway;
        }

        /** Adds rotation sway based on keyboard input */
        private Vector3 GetRotationMoveSway()
        {
            var x = InputSystem.Instance.MoveAxis.x;
            var factor = pPhysics.Speed / pMovement.MoveSpeed;
            var rotationMoveSway = pPhysics.IsGrounded ? factor * moveSway * new Vector3(0, x, -2 * x) : Vector3.zero;

            currentRotationMoveSway = Vector3.Lerp(currentRotationMoveSway, rotationMoveSway, moveSwaySpeed * Time.deltaTime);
            return currentRotationMoveSway;
        }



        /** Adds rotation sway based on mouse input */
        private Vector3 GetMouseSway()
        {
            var mouseX = InputSystem.Instance.MouseAxis.x;
            var mouseY = InputSystem.Instance.MouseAxis.y;

            // Add sway
            var dir = InvertDirection ? -1 : 1;
            currentMouseSway += new Vector3(2 * mouseY, -mouseX, mouseX * mouseSwayZ) * (dir * mouseSway * Time.deltaTime * SwayIntensity);
            currentMouseSway = Vector3.ClampMagnitude(currentMouseSway, LimitIntensity * mouseSwayMax);

            // Add deceleration
            currentMouseSway = Vector3.Lerp(currentMouseSway, Vector3.zero, mouseSwayDeceleration * Time.deltaTime);
            return currentMouseSway;
        }
    }

}