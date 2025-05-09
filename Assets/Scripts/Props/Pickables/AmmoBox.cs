using Core.Guns.Enums;
using Core.Guns.Interfaces;
using Core.Health.Interfaces;
using Core.Utilities;
using UnityEngine;

namespace Props.Pickables
{
    public class AmmoBox : Pickable
    {
        [SerializeField] private int quantity = 50;
        [SerializeField] private GunType type;


        public override void Interact(Transform interactor)
        {
            interactor.TryGetComponent<IAmmoHolder>(out var ammoHolder);

            if (ammoHolder == null) return;

            ammoHolder.AddAmmo(type, quantity);
            AudioSource source = AudioUtils.CreateAudioWithoutClip(transform.position, AUDIO_SOURCE_DESTROY_TIME);
            PlayRandomSound(source);
            Destroy(gameObject);
        }
    }
}
