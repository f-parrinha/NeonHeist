using Core.Guns.Data;
using Core.Guns.Events;
using Core.Guns.Interfaces;
using Core.UserInput;
using Player.Cameras;
using System;
using UnityEngine;


namespace Player.Controller
{
    [RequireComponent(typeof(Player))]
    public class PlayerGunController : MonoBehaviour, IGunHolder
    {
        private HandsGun handsGun;
        private Player player;

        private EventHandler<OnGunChangeArgs> onGunChange;

        [SerializeField] private HandsAnimatorController animatorController;
        [SerializeField] private Transform handsPivot;
        [SerializeField] private float dropGunForce = 2;


        public IShootable Gun => handsGun;

        private void Start()
        {
            player = gameObject.GetComponent<Player>();
        }

        private void Update()
        {
            AddShootControl();
        }

        public bool AddGun(IShootable shootable)
        {
            if (shootable is not HandsGun hands) {
                return false;
            }

            if (handsGun != null)
            {
                DropWeapon();
            }

            animatorController.PlayHolsterGet(0);
            handsGun = Instantiate(hands, handsPivot) as HandsGun;

            RaiseOnGunChange(handsGun);
            return true;
        }

        private void AddShootControl()
        {
            if (InputSystem.Instance.MouseDown(0) && handsGun != null)
            {
                GunData gunData = handsGun.GunData;
                handsGun.Shoot();
            }
        }

        private void DropWeapon()
        {
            PlayerCamera pCamera = player.Camera;
            Vector3 dropPos = pCamera.transform.position + pCamera.transform.forward;
            var dropped = Instantiate(handsGun.GunData.Pickable, dropPos, Quaternion.identity);

            if (dropped.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(pCamera.transform.forward * dropGunForce, ForceMode.Impulse);
            }

            Destroy(handsGun.gameObject);
        }

        public void AddOnGunChangeHandler(EventHandler<OnGunChangeArgs> handler) => onGunChange += handler;
        protected void RaiseOnGunChange(IShootable shootable) => onGunChange?.Invoke(this, new OnGunChangeArgs { NewGun = shootable });
    }
}