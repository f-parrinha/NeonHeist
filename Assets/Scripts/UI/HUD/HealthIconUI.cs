using Character;
using Core.Common.Interfaces;
using Core.Health.Events;
using UnityEngine;
using UnityEngine.UI;


namespace UI.HUD
{

    public class HealthIconUI : MonoBehaviour, IInitializable, IRefreshable
    {
        [SerializeField] private CharacterHealth healthHolder;
        [SerializeField] private float animAmplitude = 0.5f;
        [SerializeField] private float animFrequency = 2f;
        [SerializeField] [Range(0f,1f)] private float alertThreashold = 0.2f;

        private Image sprite;
        private bool playerAlertAnim;

        public bool IsInitialized { get; private set; }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            AlertAnimation();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            sprite = GetComponent<Image>();

            healthHolder.AddOnDamageHandler((sender, args) => Refresh());
            healthHolder.AddOnHealHandler((sender, args) => Refresh());
            IsInitialized = true;
        }

        public void Refresh()
        {
            playerAlertAnim = healthHolder.Health <= healthHolder.MaxHealth * alertThreashold;
        }

        private void AlertAnimation()
        {
            if (playerAlertAnim)
            {
                float sinFactor = Mathf.Abs(Mathf.Sin(Time.time * animFrequency));
                sprite.color = Color.red;
                sprite.rectTransform.localScale = Vector3.one * (1 + animAmplitude * sinFactor);
            }
            else
            {
                sprite.color = Color.white;
                sprite.rectTransform.localScale = Vector3.one;
            }
        }
    }
}