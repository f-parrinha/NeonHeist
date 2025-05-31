using Core.Character;
using Core.Interactions;
using Core.Utilities;
using Props.Pickables;
using UnityEngine;

namespace Props
{
    public class SpeedInjector : Pickable
    {
        [SerializeField] [Range(1f, 3f)] private float speedModifier = 1.5f;
        [SerializeField] [Range(0f, 20f)] private float modifierTimeout = 10f;

        public override void Initialize()
        {
            if (IsInitialized) return;

            SetInteractions(new Interaction("Pick Up", UponPickUp));

            IsInitialized = true;
        }

        private void UponPickUp(Transform interactor)
        {
            interactor.TryGetComponent<CharacterStats>(out var charStats);

            if (charStats == null) return;

            charStats.SetSpeedModifier(speedModifier, modifierTimeout);
            AudioSource source = AudioUtils.CreateAudioWithoutClip(transform.position, AUDIO_SOURCE_DESTROY_TIME);
            PlayRandomSound(source);
            Destroy(gameObject);
        }
    }
}