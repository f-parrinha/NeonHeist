using Core.Character.Events;
using Core.Guns.Enums;
using System;

namespace Core.Guns.Interfaces
{
    public interface IAmmoHolder
    {
        bool HasAmmo(GunType type);
        int GetByAmmoType(GunType ammoType);
        void AddAmmo(GunType type, int quantity);
        void RemoveQuantity(GunType type, int quantity);
        void RemoveOne(GunType type);
        void AddOnAmmoChangeHandler(EventHandler<OnAmmoChangeArgs> handler);
    }
}