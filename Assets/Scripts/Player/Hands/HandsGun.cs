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
using Core.Common.Interfaces;
using Player.Hands;

namespace Player.Controller
{
    /// <summary>
    /// Class <c> HandsGun </c> defines the main components associated to the model of the player's hands, while holding guns. 
    ///     It handles shooting (for shotguns, semi-auto and full-auto) and zooming.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class HandsGun : MonoBehaviour, IShootable, IZoomable
    {
        private const float PITCH_INTERVAL = 0.1f;
        private const float MAX_SHOOT_DISTANCE = 500f;
        private const float MAX_OFFSET = 5f;
        private const float OFFSET_MOVING_RESIZER = 2f;
        private const int SHOTGUN_SHOT_COUNT = 8;
        private const int SINGLE_SHOT_COUNT = 1;

        [SerializeField] private GunData gunData;
        [SerializeField] private AudioClip gunShotSound;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem bulletTracer;
        [SerializeField] private HandsRecoil handsRecoil;
        [SerializeField] private Vector3 idlePos;
        [SerializeField] private Vector3 zoomPos;

        private BulletImpactControllerFinder bulletImpactControllerFinder;
        private AudioSource audioSource;
        private TickTimer shootTimer;
        private Vector3 targetZoomPos;

        private Player player;
        private PlayerCamera pCamera;
        private CharacterAmmo charAmmo;


        public GunData GunData => gunData;
        public GunType AmmoType => GunData.GunType;
        public int CurrentAmmo => charAmmo.GetByAmmoType(AmmoType);
        public bool IsZooming { get; private set; }
        public float ShotOffset { get; private set; } 

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(Player.TAG).GetComponent<Player>();
            pCamera = player.Camera;
            charAmmo = player.Ammo;

            audioSource = GetComponent<AudioSource>();

            // Set State
            handsRecoil.SetHandsGun(this);
            bulletImpactControllerFinder = new BulletImpactControllerFinder();
            shootTimer = gunData.GunType == GunType.Rifle ? 
                new TickTimer(gunData.RateOfFireMILLI, () => UponAutoFire()) :
                new TickTimer(gunData.RateOfFireMILLI);
        }

        private void Update()
        {
            // Refresh position for zooming
            transform.localPosition = MathUtils.VectorLerp(transform.localPosition, targetZoomPos, GunData.ZoomSpeed, Time.deltaTime);

            // Refresh shot offset for zooming
            float nonZoomOffset = Mathf.Clamp(gunData.Offset + player.Physics.Speed / OFFSET_MOVING_RESIZER, 0, MAX_OFFSET);
            ShotOffset = MathUtils.Lerp(ShotOffset, IsZooming ? 0f : nonZoomOffset, GunData.ZoomSpeed, Time.deltaTime);
        }

        public void Shoot()
        {
            if (!charAmmo.HasAmmo(gunData.GunType) || shootTimer.IsRunning) return;

            if (gunData.GunType == GunType.Shotgun)
            {
                UponShoot(SHOTGUN_SHOT_COUNT);
                return;
            }

            UponShoot(SINGLE_SHOT_COUNT);
        }

        public void Zoom()
        {
            targetZoomPos = zoomPos;
            IsZooming = true;
        }

        public void Unzoom()
        {
            targetZoomPos = idlePos;
            IsZooming = false;
        }


        /** -------- AUX METHODS -------- */

        private void UponAutoFire()
        {
            if (InputSystem.Instance.Mouse(0) && charAmmo.HasAmmo(gunData.GunType))
            {
                UponShoot(SINGLE_SHOT_COUNT);
            }
        }

        private void UponShoot(int count)
        {
            audioSource.pitch = 1.0f + Random.Range(-PITCH_INTERVAL, PITCH_INTERVAL);
            audioSource.PlayOneShot(gunShotSound);
            muzzleFlash.Play();
            charAmmo.RemoveOne(gunData.GunType);
            pCamera.Recoil.AddRecoil(gunData.VerticalRecoil, gunData.HorizontalRecoil);
            handsRecoil.AddRecoil();

            for (int i = 0; i < count; i++)
            {
                // Randomize offset
                float horizontalOffset = Random.Range(-ShotOffset, ShotOffset) / IShootable.OFFSET_RESIZER;
                float verticalOffset = Random.Range(-ShotOffset, ShotOffset) / IShootable.OFFSET_RESIZER;
                Vector3 dir = (pCamera.transform.forward + 
                    pCamera.transform.right * horizontalOffset + 
                    pCamera.transform.up * verticalOffset).normalized;

                // Create bullet (kind of, its a raycast)
                Ray ray = new(pCamera.transform.position, dir);
                bulletTracer.transform.rotation = Quaternion.LookRotation(dir);
                bulletTracer.Play();
                if (Physics.Raycast(ray, out var hit, MAX_SHOOT_DISTANCE, ~LayerMask.GetMask("Player", "Ingore Raycast")))
                {

                    // Create impact
                    var impact = bulletImpactControllerFinder.Find().GetPool(hit.collider.tag).NextAt(hit.point, Quaternion.LookRotation(hit.normal));
                    impact.SetParent(hit.transform);

                    // Take health
                    hit.collider.TryGetComponent<IHealthHolder>(out var healthHolder);
                    healthHolder?.Damage(GunData.Damage);

                    // Rigidbody
                    hit.collider.TryGetComponent<Rigidbody>(out var rb);
                    rb?.AddForceAtPosition(dir * gunData.ImpactForce, hit.point);
                }
            }

            // Start await for next shot based on rate of fire
            shootTimer.Start();
        }
    }
}