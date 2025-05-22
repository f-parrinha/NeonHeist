using Core.Common.Finders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Controllers
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private UIController UIController;

        public void Win()
        {
            UIController.WinMenu.Open();
        }

        public void Lose()
        {
            UIController.DeathMenu.Open();
        }
    }
}