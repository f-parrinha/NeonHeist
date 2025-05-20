using Core.Guns.Enums;
using Core.Guns.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using UnityEngine;

namespace Core.Guns.Data
{
    [CreateAssetMenu(fileName = "NewGunData", menuName = "Scriptable Objects/Gun/GunData")]
    public class GunData : ScriptableObject
    {
        [SerializeField] private GunType ammoType;
        [SerializeField] private float damage;
        [SerializeField] private float verticalRecoil = 1f;
        [SerializeField] private float horizontalRecoil = 2f;
        [SerializeField] private float offset = 2f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private int rateOfFireRPM = 500;
        [SerializeField] private GameObject pickable;
        [SerializeField] private GameObject handsObject;

        private IShootable shootable;

        private void OnEnable()
        {
            if (handsObject == null)
            {
                Log.Warning(this, "OnEnable", "No hands object was provided");
            }

            handsObject.TryGetComponent<IShootable>(out shootable);
            if (shootable == null)
            {
                Log.Warning(this, "OnEnable", "The provided hands object is not IShootable");
            }
        }

        public GunType GunType => ammoType;
        public float Damage => damage;
        public float VerticalRecoil => verticalRecoil;
        public float HorizontalRecoil => horizontalRecoil;
        public float Offset => offset;
        public float RateOfFireRPM => rateOfFireRPM;
        public float ZoomSpeed => zoomSpeed;
        public int RateOfFireMILLI => TimeUtils.FracToMilli(60f / rateOfFireRPM);
        public GameObject Pickable => pickable;
        public GameObject HandsObject => handsObject;
        public IShootable Shootable => shootable;

    }
}