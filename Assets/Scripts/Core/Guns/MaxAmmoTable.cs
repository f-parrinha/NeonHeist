using Core.Common.Interfaces;
using Core.Guns.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Guns
{
    [CreateAssetMenu(fileName = "NewMaxAmmoTable", menuName = "Scriptable Objects/Core/MaxAmmoTable")]
    public class MaxAmmoTable : ScriptableObject, IInitializable
    {
        [NonSerialized] Dictionary<GunType, int> maxAmmoTable;
        [NonSerialized] private bool isInitialized;

        [SerializeField] private int maxRifleAmmo = 300;
        [SerializeField] private int maxPistolAmmo = 50;
        [SerializeField] private int maxShotgunAmmo = 40;


        public int MaxRifleAmmo => maxAmmoTable[GunType.Rifle];
        public int MaxPistolAmmo => maxAmmoTable[GunType.Pistol];
        public int MaxShotgunAmmo => maxAmmoTable[GunType.Shotgun];
        public bool IsInitialized { get => isInitialized; set => isInitialized = value; }


        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (isInitialized) return;


            maxAmmoTable = new Dictionary<GunType, int>
            {
                [GunType.Rifle] = maxRifleAmmo,
                [GunType.Shotgun] = maxShotgunAmmo,
                [GunType.Pistol] = maxPistolAmmo
            };
            IsInitialized = true;
        }

        public int GetMaxAmmo(GunType type) => maxAmmoTable[type];
    }
}