using Core.Interactions;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Info;
using UnityEngine;

namespace Props.Pickables
{
    public abstract class Pickable : MultiInteractable, IInitializable
    {
        protected const float AUDIO_SOURCE_DESTROY_TIME = 3.0f;
        protected const float PITCH_INTERVAL = 0.2f;

        [SerializeField] private float volume = 0.5f;
        [SerializeField] private AudioClip[] pickSounds;

        public bool IsInitialized { get; protected set; }

        private void Start()
        {
            Initialize();
        }

        public abstract void Initialize();

        protected void PlayRandomSound(AudioSource source)
        {
            if (pickSounds.Length == 0) return;

            source.volume = volume;
            source.pitch = 1 + Random.Range(-PITCH_INTERVAL, PITCH_INTERVAL);
            source.PlayOneShot(pickSounds[Random.Range(0, pickSounds.Length)]);
        }
    }
}