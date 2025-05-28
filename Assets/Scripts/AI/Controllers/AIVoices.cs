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
        [SerializeField] private AudioSource source;
        [Header("Calm Voice Settings")]
        [SerializeField] private int calmVoiceMinWaitTime = 2000;
        [SerializeField] private int calmVoiceMinWaitTimeOffset = 1000;

        private TickTimer canDoCalmVoiceTimer;
        private bool canDoCalmVoice;

        protected override void Start()
        {
            base.Start();

            if (source != null) Source = source;
            canDoCalmVoice = true;
            ResetCanDoCalmVoiceTimer();
        }

        public void PlayCalmVoice()
        {
            if (canDoCalmVoice)
            {
                PlayRandomSound(calmVoices);
                canDoCalmVoice = false;
                canDoCalmVoiceTimer.Start();
            }
        }

        public void PlayAlertVoice()
        {
            PlayRandomSound(alertVoices);
        }

        public void PlayDangerVoice()
        {
            PlayRandomSound(dangerVoices);
        }

        public void PlayAttackVoice()
        {
            PlayRandomSound(attackVoices);
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