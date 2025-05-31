using Core.Utilities;
using Player.Controller;
using UnityEngine;

namespace Player.Hands
{
    /// <summary>
    /// Class <c> HandsRecoil </c> defines the behaviour for adding procedural animations to the Player's hands (while holding the gun)
    /// <para> This component is applied to a pivot point for the recoil animation </para>
    /// </summary>
    public class HandsRecoil : MonoBehaviour
    {
        private const float PULLBACK_RECOIL_RESIZER = 30f;

        [SerializeField] private float resetSpeed = 5f;
        [SerializeField] private float springDamping = 3f;
        [SerializeField] private float springRestitution = 250f;
        [SerializeField] private float springSmoothness = 20f;
        [SerializeField] private float springResizer = 1f;

        private Spring spring;
        private HandsGun handsGun;
        private Vector3 initPivotPos;

        private void Start()
        {
            initPivotPos = transform.localPosition;
            spring = new Spring(springDamping, springRestitution, springSmoothness);
        }

        private void Update()
        {
            spring.Simulate();
            transform.localPosition = MathUtils.VectorLerp(transform.localPosition, initPivotPos, resetSpeed, Time.deltaTime);
            transform.localRotation = Quaternion.Euler(spring.Value);
        }

        public void SetHandsGun(HandsGun handsGun)
        {
            this.handsGun = handsGun;
        }

        public void AddRecoil()
        {
            if (handsGun == null) return;

            float vRecoil = handsGun.GunData.VerticalRecoil;
            float hRecoil = handsGun.GunData.HorizontalRecoil;

            // Add pullback
            Vector3 pivotPos = transform.localPosition;
            pivotPos.z -= vRecoil / PULLBACK_RECOIL_RESIZER;

            // Animate
            transform.localPosition = pivotPos;
            spring.AddSpringAction(springResizer * (- Vector3.right * vRecoil + Vector3.up * Random.Range(-hRecoil, hRecoil)));
        }
    }
}