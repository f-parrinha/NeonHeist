using Character;
using UnityEngine;

namespace AI.Common
{
    public class AIVoices : CharacterVoices
    {
        [SerializeField] private AudioClip[] calmVoices;
        [SerializeField] private AudioClip[] alertVoices;
        [SerializeField] private AudioClip[] dangerVoices;

        public void PlayCalmVoice()
        {
            PlayRandom(calmVoices);
        }

        public void PlayAlertVoice()
        {
            PlayRandom(alertVoices);
        }

        public void PlayDangerVoice()
        {
            PlayRandom(dangerVoices);
        }

        private void PlayRandom(AudioClip[] voices)
        {
            if (voices.Length == 0) return;

            PlaySound(voices[Random.Range(0, voices.Length)]);
        }
    }
}