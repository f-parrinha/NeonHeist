using Core.Character.Events;
using Core.Utilities.Timing;
using System;
using UnityEngine;

namespace Core.Character
{
    public class CharacterStats : MonoBehaviour
    {
        private TickTimer speedModifierTimer;
        private bool isSpeedModifierOn;

        private event EventHandler<OnSpeedModifiedArgs> OnSpeedModified;

        public float SpeedModifier { get; private set; } = 1;

        private void Start()
        {
            SpeedModifier = 1f;
        }


        public void SetSpeedModifier(float modifier, float timeout)
        {
            if (isSpeedModifierOn) return;

            isSpeedModifierOn = true;
            speedModifierTimer?.Stop();
            speedModifierTimer = new TickTimer(TimeUtils.FracToMilli(timeout), () =>
            {
                isSpeedModifierOn = false;
                SpeedModifier = 1;
            });

            speedModifierTimer.Start();
            SpeedModifier = modifier;
            RaiseOnSpeedModified(modifier);
        }

        public void AddOnSpeedModifiedHandler(EventHandler<OnSpeedModifiedArgs> handler) => OnSpeedModified += handler;
        protected void RaiseOnSpeedModified(float modifier) => OnSpeedModified?.Invoke(this, new OnSpeedModifiedArgs { Modifier = modifier });
    }
}