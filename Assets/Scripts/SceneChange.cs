using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LevelType
{
    Level02,
    Level03,
}

public class SceneChange : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private LevelType sceneName;
    private float fadeDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(FadeToScene(sceneName.ToString())); //SceneManager.LoadScene("Level02");  //LoadSceneAsync ??

        }
    }
    
    private IEnumerator FadeToScene(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Load scene
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, blend);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }


   
}
