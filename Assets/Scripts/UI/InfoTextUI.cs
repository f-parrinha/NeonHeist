using Core.Common.Interfaces.Info;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InfoTextUI : MonoBehaviour, IInfoDisplayable
    {
        private TextMeshProUGUI textGUI;

        private void Start()
        {
            textGUI = GetComponent<TextMeshProUGUI>();

            textGUI.text = string.Empty;
        }

        public void Display(string text)
        {
            textGUI.text = text;
        }
    }
}