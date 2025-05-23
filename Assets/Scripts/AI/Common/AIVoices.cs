using Character;
using Core.Utilities.Timing;
using UnityEngine;

namespace AI.Common
{
    public class AIVoices : CharacterVoices
    {
        [Header("General Settings")]
        [SerializeField] private AudioClip[] calmVoices;
        [SerializeField] private AudioClip[] alertVoices;
        [SerializeField] private AudioClip[] dangerVoices;
        [SerializeField] private AudioClip[] attackVoices;
        [Header("Calm Voice Settings")]
        [SerializeField] private int calmVoiceMinWaitTime = 2000;
        [SerializeField] private int calmVoiceMinWaitTimeOffset = 1000;

        private TickTimer canDoCalmVoiceTimer;
        private bool canDoCalmVoice;

        protected override void Start()
        {
            base.Start();

            canDoCalmVoice = true;
            ResetCanDoCalmVoiceTimer();
        }

        public void PlayCalmVoice()
        {
            if (canDoCalmVoice)
            {
                PlayRandom(calmVoices);
                canDoCalmVoice = false;
                canDoCalmVoiceTimer.Start();
            }
        }

        public void PlayAlertVoice()
        {
            PlayRandom(alertVoices);
        }

        public void PlayDangerVoice()
        {
            PlayRandom(dangerVoices);
        }

        public void PlayAttackVoice()
        {
            PlayRandom(attackVoices);
        }

        private void PlayRandom(AudioClip[] voices)
        {
            if (voices.Length == 0) return;

            PlaySound(voices[Random.Range(0, voices.Length)]);
        }

        private void ResetCanDoCalmVoiceTimer()
        {
            int offset = Random.Range(-calmVoiceMinWaitTimeOffset, calmVoiceMinWaitTime);
            canDoCalmVoiceTimer = new TickTimer(calmVoiceMinWaitTime + offset, () =>
            {
                canDoCalmVoice = true;
                ResetCanDoCalmVoiceTimer();
            });
        }
    }
}