using Core.Common.Finders;
using Core.Common.Interfaces;
using Core.Controllers;
using UnityEngine;

namespace UI.Menus
{
    public class MenuUI : MonoBehaviour, IRefreshable, IOpenable, IInitializable
    {
        private PauseControllerFinder pauseControllerFinder;

        public bool IsOpened {  get; private set; }

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
            IsOpened = !IsOpened;
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
            IsOpened = true;
            Refresh();
        }

        public void Close()
        {
            IsOpened = false;
            Refresh();
        }
    }
}