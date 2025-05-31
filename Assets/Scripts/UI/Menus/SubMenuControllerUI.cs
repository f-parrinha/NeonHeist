using Core.Utilities;
using UnityEngine;

namespace UI.Menus
{
    public class SubMenuControllerUI : MonoBehaviour
    {
        [SerializeField] private SubMenuUI[] subMenus;

        public int SubMenusCount => subMenus.Length;

        public void CloseAll()
        {
            foreach (var subMenu in subMenus)
            {
                subMenu.Close();
            }
        }

        public void Open(int idx)
        {
            if (idx < 0 ||  idx > subMenus.Length - 1)
            {
                Log.Warning(this, "Open", "Tried to open a SubMenu with an invalid index");
                return;
            }

            CloseAll();
            subMenus[idx].Open();
        }
    }
}