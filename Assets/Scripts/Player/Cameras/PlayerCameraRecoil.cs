using Core.UserInput;
using Core.Utilities;
using UnityEngine;

namespace Player.Cameras
{
    [RequireComponent(typeof(PlayerCamera))]
    public class PlayerCameraRecoil : MonoBehaviour
    {
        private PlayerCamera pCamera;

        [SerializeField] private float smoothness = 30f;
        [SerializeField] private float vRecoilRecovery = 4f;
        [SerializeField] private float hRecoilRecovery = 5f;

        private float vRecoilRecoverySpeed;
        private float vRecoil;
        private float hRecoil;
        private Vector3 smoothenedRecoil;

        private void Start()
        {
            pCamera = GetComponent<PlayerCamera>();
        }

        public void Update()
        {
            var recoil = new Vector3(-vRecoil, hRecoil, 0);
            smoothenedRecoil = MathUtils.VectorLerp(smoothenedRecoil, recoil, smoothness, Time.deltaTime);

            pCamera.AddRotation(this, smoothenedRecoil);

            float mouseRecovery = Mathf.Min(InputSystem.Instance.MouseAxis.y * pCamera.MouseSensitivity, 0);

            vRecoilRecoverySpeed += vRecoilRecovery * Time.deltaTime;
            vRecoil = Mathf.Max(vRecoil - vRecoilRecoverySpeed + mouseRecovery, 0);
            hRecoil = MathUtils.Lerp(hRecoil, 0, hRecoilRecovery, Time.deltaTime);
        }

        public void AddRecoil(float vRecoil, float hRecoil)
        {
            vRecoilRecoverySpeed = 0;

            this.vRecoil += vRecoil;
            this.hRecoil += Random.Range(-hRecoil, hRecoil);
        }
    }
}