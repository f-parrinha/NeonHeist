using Core.Common.Finders;
using Core.Common.Queue;
using Core.UserInput;
using UnityEngine;

namespace Core.Controllers
{
    public class PauseController : MonoBehaviour
    {
        private BoolQueue activeQueue;

        [SerializeField] private CursorController cursorController;

        public bool IsActive => activeQueue.Evaluate();
        public bool IsPaused => !IsActive;

        private void Start()
        {
            activeQueue = new BoolQueue();
            ResetPause();
        }

        public void Pause(object setter, bool value)
        {
            activeQueue.Set(setter, !value);
            Evaluate(); 
        }

        public void UnsetPauser(object setter)
        {
            activeQueue.Unset(setter);
            Evaluate();
        }

        public void ResetPause()
        {
            activeQueue.Clear();
            Evaluate();
        }


        public void Evaluate()
        {
            cursorController.SetEnabled(IsPaused);
            InputSystem.Instance.SetActive(this, !IsPaused);
            Time.timeScale = IsPaused ? 0 : 1;
        }
    }
}

