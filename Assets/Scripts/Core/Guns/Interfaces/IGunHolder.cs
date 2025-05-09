using Core.Guns.Events;
using System;

namespace Core.Guns.Interfaces
{
    public interface IGunHolder
    {
        IShootable Gun { get; }

        bool AddGun(IShootable shootable);
        void AddOnGunChangeHandler(EventHandler<OnGunChangeArgs> handler);
    }
}