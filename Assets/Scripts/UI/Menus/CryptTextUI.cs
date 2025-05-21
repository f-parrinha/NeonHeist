using Core.Utilities.Timing;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CryptTextUI : MonoBehaviour
{
    private readonly string[] CHARS = new string[] 
    { 
        "ア", "イ", "あ", "わ", "れ", "あ", "わ", "れ", "む",
        "く", "ら", "ン", "い", "こ", "ろ", "も", "な", "え", 
        "る", "な", "ぐ", "さ", "ユ", "ク", "ひ", "ツ", "る"
    };

    private TextMeshProUGUI textMesh;
    private TickTask changeCharsTask;

    [SerializeField] private int charsAmount = 30;
    [SerializeField] private int refreshRate = 1000;
    
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        changeCharsTask = new TickTask(refreshRate, UponChangeChars);
        changeCharsTask.Start();
    }

    
    private void UponChangeChars()
    {
        string text = string.Empty;

        for (int i = 0; i < charsAmount; i++) 
        {
            text += CHARS[Random.Range(0, CHARS.Length)];
        }

        textMesh.text = text;
    }
}
