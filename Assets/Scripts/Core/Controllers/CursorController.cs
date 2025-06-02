using Core.Common.Interfaces;
using Core.Common.Queue;
using UnityEngine;

namespace Core.Controllers
{
    public class CursorController : MonoBehaviour, IRefreshable
    {
        private BoolQueue activeQueue;

        [SerializeField] private bool startEnabled = false;

        public bool IsEnabled => activeQueue.Evaluate();


        private void Awake()
        {
            activeQueue = new BoolQueue();

            SetEnabled(this, startEnabled);
        }

        private void Start()
        {
            Refresh();
        }


        public void SetEnabled(object setter, bool enabled) 
        {
            activeQueue.Set(setter, enabled);
            Refresh();
        }

        public void UnsetEnabler(object setter)
        {
            activeQueue.Unset(setter);
            Refresh();
        }

        public void Refresh()
        {
            if (IsEnabled)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}