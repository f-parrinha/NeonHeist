using TMPro;
using UnityEngine;

namespace UI.Components
{

    public class TextDisplayerUI : MonoBehaviour
    {
        private TextMeshProUGUI text;

        public TextMeshProUGUI Text => text == null ? text = GetComponent<TextMeshProUGUI>() : text;

        protected virtual void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            Text.text = text;
        }
    }
}
