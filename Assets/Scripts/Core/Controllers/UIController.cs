using Core.Common.Interfaces;
using Core.Utilities;
using UnityEngine;

namespace Core.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject deathMenuObject;
        [SerializeField] private GameObject winMenuObject;

        private IOpenable deathMenu;
        private IOpenable winMenu;

        public IOpenable DeathMenu => deathMenu == null ? deathMenu = deathMenuObject.GetComponent<IOpenable>() : deathMenu;
        public IOpenable WinMenu => winMenu == null ? winMenu = winMenuObject.GetComponent<IOpenable>() : winMenu;


        private void Start()
        {
            if (!IsMenu(deathMenuObject))
            {
                Log.Error(this, "Start", "DeathMenu menu is not a plausible menu, as it is not IOpenable");
            }
            if (!IsMenu(winMenuObject))
            {
                Log.Error(this, "Start", "WinMenu menu is not a plausible menu, as it is not IOpenable");
            }

            deathMenu = deathMenuObject.GetComponent<IOpenable>();
            winMenu = winMenuObject.GetComponent<IOpenable>();
        } 

        public static bool IsMenu(GameObject obj)
        {
            return obj != null && obj.TryGetComponent<IOpenable>(out _);
        }
    }
}