using Core.Common.Interfaces;
using Core.Common.Queue;
using UnityEngine;

namespace Core.Controllers
{
    public class CursorController : MonoBehaviour, IRefreshable
    {
        [SerializeField] private bool startEnabled = false;

        public bool IsEnabled { get; private set; }


        private void Awake()
        {
            SetEnabled(startEnabled);
        }

        public void SetEnabled(bool enabled) 
        {
            IsEnabled = enabled;
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