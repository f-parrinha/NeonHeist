using Core.Common.Interfaces;
using Core.Guns.Events;
using System;

namespace Core.Guns.Interfaces
{
    public interface IGunHolder : IZoomable
    {
        IShootable Gun { get; }

        bool AddGun(IShootable shootable);
        void AddOnGunChangeHandler(EventHandler<OnGunChangeArgs> handler);
    }
}