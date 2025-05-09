using Core.Guns.Enums;

namespace Core.Character.Events
{
    public class OnAmmoChangeArgs
    {
        public int OldAmmo { get; set; }
        public int NewAmmo { get; set; }
        public GunType AmmoType { get; set; }
    }
}