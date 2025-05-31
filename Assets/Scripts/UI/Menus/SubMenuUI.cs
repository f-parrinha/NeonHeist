using Core.Common.Interfaces;
using UnityEngine;

namespace UI.Menus
{
    public class SubMenuUI : MonoBehaviour, IOpenable, IRefreshable
    {
        public bool IsOpened { get; private set; }

        private void Start()
        {
            Close();
        }

        public void Close()
        {
            IsOpened = false;
            Refresh();
        }

        public void Open()
        {
            IsOpened = true;
            Refresh();
        }

        public bool Toggle()
        {
            IsOpened = !IsOpened;
            Refresh();
            return IsOpened;
        }

        public void Refresh()
        {
            gameObject.SetActive(IsOpened);
        }
    }
}