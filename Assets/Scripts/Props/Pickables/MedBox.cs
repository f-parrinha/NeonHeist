using Core.Health.Interfaces;
using Core.Interactions;
using Core.Utilities;
using UnityEngine;

namespace Props.Pickables
{
    public class MedBox : Pickable
    {
        [SerializeField] private float healFactor = 50f;

        public override void Initialize()
        {
            if (IsInitialized) return;

            SetInteractions(new Interaction("Pick Up", UponPickUp));

            IsInitialized = true;
        }


        private void UponPickUp(Transform interactor)
        {
            interactor.TryGetComponent<IHealthHolder>(out var healthHolder);

            if (healthHolder == null) return;

            healthHolder.Heal(healFactor);
            AudioSource source = AudioUtils.CreateAudioWithoutClip(transform.position, AUDIO_SOURCE_DESTROY_TIME);
            PlayRandomSound(source);
            Destroy(gameObject);
        }
    }
}
