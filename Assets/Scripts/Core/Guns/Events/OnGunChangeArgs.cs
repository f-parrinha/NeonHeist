using Core.Guns.Data;
using Core.Guns.Interfaces;

namespace Core.Guns.Events
{
    public class OnGunChangeArgs
    {
        public IShootable NewGun { get; set; }
    }
}