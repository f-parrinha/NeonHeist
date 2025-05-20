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
        private const float RECOIL_RESIZER = 30f;

        [SerializeField] private float resetSpeed = 5f;
        [SerializeField] private float springDamping = 3f;
        [SerializeField] private float springRestitution = 250f;
        [SerializeField] private float springSmoothness = 20f;

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

            float recoil = handsGun.GunData.VerticalRecoil;
            Vector3 pivotPos = transform.localPosition;
            pivotPos.z -= recoil / RECOIL_RESIZER;

            transform.localPosition = pivotPos;
            spring.AddSpringAction(-Vector3.right * recoil);
        }
    }
}