using Core.Common.Queue;
using Core.UserInput;
using UnityEngine;

namespace Player.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Player player; 
        [SerializeField] private float mouseSensitivity = 2;

        private PlayerCameraRecoil pCameraRecoil;
        private Vector3Queue rotQueue;
        private Vector3 mouseRotation;

        public PlayerCameraRecoil Recoil => pCameraRecoil == null ? pCameraRecoil = GetComponent<PlayerCameraRecoil>() : pCameraRecoil;
        public float MouseSensitivity => mouseSensitivity;


        private void Start()
        {
            rotQueue = new Vector3Queue();

            mouseRotation = new Vector3(player.transform.rotation.y, 0, 0);
        }

        private void Update()
        {
            AddMouseRotation();
        }

        private void LateUpdate()
        {
            StickToHead();
            transform.localRotation = Quaternion.Euler(rotQueue.Evaluate());
        }

        public void AddRotation(object setter, Vector3 eulerAngle)
        {
            rotQueue.Set(setter, eulerAngle);
        }

        private void StickToHead()
        {
            transform.position = player.transform.position + player.transform.up * player.Physics.CurrentHeight / 2;
        }

        private void AddMouseRotation()
        {
            mouseRotation.x += InputSystem.Instance.MouseX * mouseSensitivity;
            mouseRotation.y -= InputSystem.Instance.MouseY * mouseSensitivity;
            mouseRotation.y = Mathf.Clamp(mouseRotation.y, -90f, 90f);

            player.Rotate(this, Vector3.up * mouseRotation.x);
            rotQueue.Set(this, Vector3.right * mouseRotation.y);
        }
    }
}