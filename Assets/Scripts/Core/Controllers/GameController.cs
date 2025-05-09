using Core.Common.Finders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Controllers
{
    public class GameController : MonoBehaviour
    {
        PauseControllerFinder pauseControllerFinder;

        [SerializeField] private UIController UIController;

        private void Start()
        {
            pauseControllerFinder = new PauseControllerFinder();
        }

        public void Win()
        {
            UIController.WinMenu.Open();
        }

        public void Lose()
        {
            UIController.DeathMenu.Open();
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