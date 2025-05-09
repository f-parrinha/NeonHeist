using Character;
using Core.Common.Interfaces;
using Core.Health.Events;
using UI.Components;
using UnityEngine;

namespace UI
{
    public class HealthTextUI : TextDisplayerUI, IInitializable
    {
        [SerializeField] private CharacterHealth healthHolder;

        public bool IsInitialized { get; private set; }

        protected override void Start()
        {
            base.Start();

            Initialize();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            SetText(healthHolder.Health.ToString("0"));
            healthHolder.AddOnDamageHandler((object sender, OnHealthChangeArgs args) => SetText(args.NewHealth.ToString("0")));
            healthHolder.AddOnHealHandler((object sender, OnHealthChangeArgs args) => SetText(args.NewHealth.ToString("0")));
            IsInitialized = true;
        }
    }
}