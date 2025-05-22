using Core.Common.Finders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class SceneController : MonoBehaviour
    {
        PauseControllerFinder pauseControllerFinder;

        private void Awake()
        {
            pauseControllerFinder = new PauseControllerFinder();
        }

        public void ChangeLevel(int idx)
        {
            SceneManager.LoadScene(idx);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Restart()
        {
            pauseControllerFinder.Find().ResetPause();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}