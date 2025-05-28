using Core.Common.Interfaces;
using UnityEngine;


namespace Character
{ 
    [RequireComponent(typeof(AudioSource))]
    public class CharacterVoices : MonoBehaviour, ICleanable
    {
        [SerializeField] private float pitchInterval = 0.1f;
        [SerializeField] private float pitch = 1f;
        [SerializeField] private AudioClip[] damageVoices;
        [SerializeField] private AudioClip[] deathVoices;


        public AudioSource Source { get; protected set; }

        protected virtual void Start()
        {
            Source = GetComponent<AudioSource>();
            Source.pitch = pitch;
        }

        public void PlayDamageVoice()
        {
            PlayRandomSound(damageVoices);
        }
        public void PlayDeathVoice()
        {
            PlayRandomSound(deathVoices);
        }

        public void CleanUp()
        {
            Source.Stop();
        }

        protected void PlaySound(AudioClip sound)
        {
            Source.pitch = pitch + Random.Range(-pitchInterval, pitchInterval);
            Source.clip = sound;
            Source.Play();
        }

        protected void PlayRandomSound(AudioClip[] voices)
        {
            if (voices.Length == 0) return;

            PlaySound(voices[Random.Range(0, voices.Length)]);
        }
    }
}


