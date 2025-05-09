using Core.Common.Interfaces;
using Core.Common.Interfaces.Info;
using UnityEngine;

namespace Props.Pickables
{
    public abstract class Pickable : MonoBehaviour, IInteractable, IInfoHolder
    {
        protected const float AUDIO_SOURCE_DESTROY_TIME = 3.0f;
        protected const float PITCH_INTERVAL = 0.2f;

        [SerializeField] private AudioClip[] pickSounds;

        public abstract void Interact(Transform interactor);

        protected void PlayRandomSound(AudioSource source)
        {
            if (pickSounds.Length == 0) return;

            source.pitch = 1 + Random.Range(-PITCH_INTERVAL, PITCH_INTERVAL);
            source.PlayOneShot(pickSounds[Random.Range(0, pickSounds.Length)]);
        }

        public string GetInfo()
        {
            return "Pick Up";
        }
    }
}