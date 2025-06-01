using Core.Common.Finders;
using Core.Common.Queue;
using Core.UserInput;
using UnityEngine;

namespace Core.Controllers
{
    public class PauseController : MonoBehaviour
    {
        private BoolQueue activeQueue;
        private BoolQueue inputActiveQueue;

        [SerializeField] private CursorController cursorController;

        public bool IsInputActive => inputActiveQueue.Evaluate();
        public bool IsActive => activeQueue.Evaluate();
        public bool IsPaused => !IsActive;

        private void Awake()
        {
            activeQueue = new BoolQueue();
            inputActiveQueue = new BoolQueue();
        }

        private void Start()
        {
            ResetPause();
        }

        public void SetInputActive(object setter, bool value)
        {
            inputActiveQueue.Set(setter, value);
            Evaluate();
        }
        public void UnsetInputActivator(object setter)
        {
            inputActiveQueue.Unset(setter);
            Evaluate();
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
            cursorController.SetEnabled(this, IsActive);
            InputSystem.Instance.SetActive(this, IsActive && IsInputActive);
            Time.timeScale = IsPaused ? 0 : 1;
        }
    }
}

