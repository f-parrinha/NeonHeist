using Core.Character;
using Core.Character.Events;
using Core.Common.Interfaces;
using Core.Health.Events;
using UnityEngine;


namespace Character
{ 
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CharacterHealth))]
    [RequireComponent(typeof(CharacterStats))]
    public class CharacterVoices : MonoBehaviour, ICleanable
    {
        [SerializeField] private float pitchInterval = 0.1f;
        [SerializeField] private float pitch = 1f;
        [SerializeField] private AudioClip[] damageVoices;

        private CharacterHealth charLife;

        public AudioSource Source { get; private set; }

        void Start()
        {
            Source = GetComponent<AudioSource>();
            Source.pitch = pitch;

            charLife = GetComponent<CharacterHealth>();


            // Setup event handlers
            charLife.AddOnDamageHandler((object sender, OnHealthChangeArgs args) =>
            {
                if (damageVoices.Length == 0) return;

                int index = Random.Range(0, damageVoices.Length);
                PlaySound(damageVoices[index]);
            });
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
    }
}


