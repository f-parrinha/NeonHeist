using Core.Guns.Data;
using Core.Guns.Interfaces;
using Core.Utilities;
using UnityEngine;

namespace Props.Pickables
{
    public class Gun : Pickable
    {
        [SerializeField] private GunData gunData;

        public override void Interact(Transform interactor)
        {
            interactor.TryGetComponent<IGunHolder>(out var gunHolder);

            if (gunHolder == null) return;

            gunHolder.AddGun(gunData.Shootable);
            AudioSource source = AudioUtils.CreateAudioWithoutClip(transform.position, AUDIO_SOURCE_DESTROY_TIME);
            PlayRandomSound(source);
            Destroy(gameObject);
        }
    }
}