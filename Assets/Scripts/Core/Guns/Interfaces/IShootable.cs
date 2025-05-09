using Core.Guns.Data;

namespace Core.Guns.Interfaces
{
    public interface IShootable
    {
        public const float OFFSET_RESIZER = 30;

        GunData GunData { get; }
        void Shoot();
    }
}