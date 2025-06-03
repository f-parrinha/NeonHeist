using Core.UserInput;
using UnityEngine;

namespace Core.Controllers
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private UIController UIController;
        [SerializeField] private PauseController pauseController;
        [SerializeField] private CursorController cursorController;

        private void Start()
        {
            InputSystem.Instance.ClearActive();
        }

        public void Win()
        {
            UIController.WinMenu.Open();
            pauseController.Pause(this, true);
            cursorController.SetEnabled(true);
        }

        public void Lose()
        {
            UIController.DeathMenu.Open();
            pauseController.Pause(this, true);
            cursorController.SetEnabled(true);
        }

       
    }
}