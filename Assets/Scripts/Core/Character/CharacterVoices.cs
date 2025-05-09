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
        [SerializeField] private AudioClip[] healVoices;
        [SerializeField] private AudioClip[] modifierVoices;

        private CharacterHealth charLife;
        private CharacterStats charStats;

        public AudioSource AudioSource { get; private set; }

        void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            AudioSource.pitch = pitch;

            charLife = GetComponent<CharacterHealth>();
            charStats = GetComponent<CharacterStats>();


            // Setup event handlers
            charLife.AddOnDamageHandler((object sender, OnHealthChangeArgs args) =>
            {
                if (damageVoices.Length == 0) return;

                int index = Random.Range(0, damageVoices.Length);
                AudioSource.pitch = pitch + Random.Range(-pitchInterval, pitchInterval);
                AudioSource.PlayOneShot(damageVoices[index]);
            });

            charLife.AddOnHealHandler((object sender, OnHealthChangeArgs args) =>
            {
                if (healVoices.Length == 0) return;

                int index = Random.Range(0, healVoices.Length);
                AudioSource.pitch = pitch + Random.Range(-pitchInterval, pitchInterval);
                AudioSource.PlayOneShot(healVoices[index]);
            });

            charStats.AddOnSpeedModifiedHandler((object sender, OnSpeedModifiedArgs args) =>
            {
                if (modifierVoices.Length == 0) return;

                int index = Random.Range(0, modifierVoices.Length);
                AudioSource.pitch = pitch + Random.Range(-pitchInterval, pitchInterval);
                AudioSource.PlayOneShot(modifierVoices[index]);
            });
        }


        public void CleanUp()
        {
            AudioSource.Stop();
        }
    }
}


