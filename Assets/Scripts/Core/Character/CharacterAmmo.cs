using Core.Character.Events;
using Core.Guns;
using Core.Guns.Enums;
using Core.Guns.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterAmmo : MonoBehaviour, IAmmoHolder 
    {
        [SerializeField] private MaxAmmoTable maxAmmoTable;

        private Dictionary<GunType, int> ammo;

        private event EventHandler<OnAmmoChangeArgs> onAmmoChange;

        [SerializeField] private int startingRifleAmmo = 50;
        [SerializeField] private int stargingShotgunAmmo = 30;
        [SerializeField] private int startingPistolAmmo = 10;

        public int RifleAmmo => ammo[GunType.Rifle];
        public int PistolAmmo => ammo[GunType.Pistol];
        public int ShotgunAmmo => ammo[GunType.Shotgun];


        private void Awake()
        {
            ammo = new Dictionary<GunType, int>()
            {
                [GunType.Rifle] = startingRifleAmmo,
                [GunType.Shotgun] = stargingShotgunAmmo,
                [GunType.Pistol] = startingPistolAmmo,
            };
        }

        public bool HasAmmo(GunType ammoType) => ammo[ammoType] > 0;

        public int GetByAmmoType(GunType ammoType)
        {
            return ammo[ammoType];
        }

        public void AddAmmo(GunType type, int quantity)
        {
            int oldAmmo = ammo[type];
            int newAmmo = ammo[type] = Mathf.Min(ammo[type] + quantity, maxAmmoTable.GetMaxAmmo(type));

            RaiseOnAmmoChange(type, oldAmmo, newAmmo);
        }

        public void RemoveQuantity(GunType type, int quantity)
        {
            int oldAmmo = ammo[type];
            int newAmmo = ammo[type] = Mathf.Max(ammo[type] - quantity, 0);

            RaiseOnAmmoChange(type, oldAmmo, newAmmo);
        }

        public void RemoveOne(GunType type) => RemoveQuantity(type, 1);


        public void AddOnAmmoChangeHandler(EventHandler<OnAmmoChangeArgs> handler) => onAmmoChange += handler;
        protected void RaiseOnAmmoChange(GunType type, int oldAmmo, int newAmmo) => onAmmoChange?.Invoke(this, new OnAmmoChangeArgs 
        { 
            OldAmmo = oldAmmo,
            NewAmmo = newAmmo,
            AmmoType = type
        });
    }
}

