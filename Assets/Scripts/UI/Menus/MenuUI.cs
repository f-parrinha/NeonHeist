using Core.Common.Finders;
using Core.Common.Interfaces;
using UnityEngine;

namespace UI.Menus
{
    public class MenuUI : MonoBehaviour, IRefreshable, IOpenable, IInitializable
    {
        private PauseControllerFinder pauseControllerFinder;

        public bool IsOpened => gameObject.activeSelf;

        public bool IsInitialized { get; private set; }

        private void Start()
        {
            Initialize();   
        }


        public void Initialize()
        {
            if (IsInitialized) return;

            pauseControllerFinder = new PauseControllerFinder();

            IsInitialized = true;
        }

        public bool Toggle()
        {   
            gameObject.SetActive(!gameObject.activeSelf);
            Refresh();

            return IsOpened;
        }

        public void Refresh()
        {
            Initialize();

            gameObject.SetActive(IsOpened);
            pauseControllerFinder.Find().Pause(this, IsOpened);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Refresh();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            Refresh();
        }
    }
}