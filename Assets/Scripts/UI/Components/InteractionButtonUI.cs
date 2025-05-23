using Core.Common.Interfaces;
using Core.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    [RequireComponent(typeof(Image))]
    public class InteractionButtonUI : MonoBehaviour, IInitializable
    {
        private Image image;

        [SerializeField] private float inactiveAlpha = 0.2f;
        [SerializeField] private float activeAlpha = 1f;
        [SerializeField] private TextMeshProUGUI textMesh;

        public bool IsInitialized { get; private set; }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            image = GetComponent<Image>();
            image.color = Color.white;
            Disable();

            IsInitialized = true;
        }

        public void Initialize(string name)
        {
            if (IsInitialized) return;

            textMesh.text = name;

            Initialize();
        }

        public void Enable()
        {
            SetAlpha(activeAlpha);
        }

        public void Disable()
        {
            SetAlpha(inactiveAlpha);
        }

        private void SetAlpha(float alpha)
        {
            if (image == null) return;

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}
     