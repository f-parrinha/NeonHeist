using Character;
using Core.Guns.Enums;
using Core.Guns.Interfaces;
using Core.Utilities;
using Core.Guns.Data;
using UnityEngine;
using Player.Cameras;
using Core.Health.Interfaces;
using Core.Common.Finders.Pools;
using Core.Utilities.Timing;
using Core.UserInput;

namespace Player.Controller
{
    [RequireComponent(typeof(AudioSource))]
    public class HandsGun : MonoBehaviour, IShootable
    {
        private const float PITCH_INTERVAL = 0.1f;
        private const float MAX_SHOOT_DISTANCE = 500f;
        private const int SHOTGUN_SHOT_COUNT = 8;
        private const int SINGLE_SHOT_COUNT = 1;

        [SerializeField] private GunData gunData;
        [SerializeField] private AudioClip gunShotSound;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem bulletTracer;


        private BulletImpactPoolFinder bulletImpactPoolFinder;
        private AudioSource audioSource;
        private TickTimer shootTimer;

        private PlayerCamera pCamera;
        private CharacterAmmo charAmmo;


        public GunData GunData => gunData;
        public GunType AmmoType => GunData.GunType;
        public int CurrentAmmo => charAmmo.GetByAmmoType(AmmoType);


        private void Start()
        {
            Player player = GameObject.FindGameObjectWithTag(Player.TAG).GetComponent<Player>();
            pCamera = player.Camera;
            charAmmo = player.Ammo;

            audioSource = GetComponent<AudioSource>();

            bulletImpactPoolFinder = new BulletImpactPoolFinder();

            shootTimer = gunData.GunType == GunType.Rifle ? new TickTimer(gunData.RateOfFireMILLI, () => AutoFire()) :
                new TickTimer(gunData.RateOfFireMILLI);
        }

        public void Shoot()
        {
            if (!charAmmo.HasAmmo(gunData.GunType) || shootTimer.IsRunning) return;

            if (gunData.GunType == GunType.Shotgun)
            {
                OnShoot(SHOTGUN_SHOT_COUNT);
                return;
            }

            OnShoot(SINGLE_SHOT_COUNT);
        }


        private void AutoFire()
        {
            if (InputSystem.Instance.Mouse(0) && charAmmo.HasAmmo(gunData.GunType))
            {
                OnShoot(SINGLE_SHOT_COUNT);
            }
        }


        private void OnShoot(int count)
        {
            audioSource.pitch = 1.0f + Random.Range(-PITCH_INTERVAL, PITCH_INTERVAL);
            audioSource.PlayOneShot(gunShotSound);
            muzzleFlash.Play();
            charAmmo.RemoveOne(gunData.GunType);
            pCamera.Recoil.AddRecoil(gunData.VerticalRecoil, gunData.HorizontalRecoil);

            for (int i = 0; i < count; i++)
            {
                // Randomize offset
                float horizontalOffset = Random.Range(-gunData.Offset, gunData.Offset) / IShootable.OFFSET_RESIZER;
                float verticalOffset = Random.Range(-gunData.Offset, gunData.Offset) / IShootable.OFFSET_RESIZER;
                Vector3 dir = (pCamera.transform.forward + 
                    pCamera.transform.right * horizontalOffset + 
                    pCamera.transform.up * verticalOffset).normalized;



                // Create bullet (kind of, its a raycast)
                Ray ray = new(pCamera.transform.position, dir);
                bulletTracer.transform.rotation = Quaternion.LookRotation(dir);
                bulletTracer.Play();
                if (Physics.Raycast(ray, out var hit, MAX_SHOOT_DISTANCE, LayerUtils.IGNORE_PLAYER_LAYER, QueryTriggerInteraction.Ignore))
                {

                    // Create impact
                    var impact = bulletImpactPoolFinder.Find().NextAt(hit.point, Quaternion.LookRotation(hit.normal));
                    impact.SetParent(hit.transform);

                    // Take health
                    hit.collider.TryGetComponent<IHealthHolder>(out var healthHolder);
                    healthHolder?.Damage(GunData.Damage);
                }
            }

            // Start await for next shot based on rate of fire
            shootTimer.Start();
        }
    }
}