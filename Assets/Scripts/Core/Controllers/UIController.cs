using Core.Common.Interfaces;
using Core.Utilities;
using UnityEngine;

namespace Core.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject deathMenuObject;
        [SerializeField] private GameObject winMenuObject;
        [SerializeField] private GameObject interactionMenuObject;
        [SerializeField] private GameObject cursorObject;

        private IOpenable deathMenu;
        private IOpenable winMenu;
        private IOpenable interactionMenu;
        private IEnableable cursor;

        public IEnableable Cursor => cursor == null ? cursor = cursorObject.GetComponent<IEnableable>() : cursor;
        public IOpenable DeathMenu => deathMenu == null ? deathMenu = deathMenuObject.GetComponent<IOpenable>() : deathMenu;
        public IOpenable WinMenu => winMenu == null ? winMenu = winMenuObject.GetComponent<IOpenable>() : winMenu;
        public IOpenable InteractionMenu => interactionMenu == null ? interactionMenu = interactionMenuObject.GetComponent<IOpenable>() : interactionMenu;


        private void Start()
        {
            /*if (!IsMenu(deathMenuObject))
            {
                Log.Error(this, "Start", "DeathMenu menu is not a plausible menu, as it is not IOpenable");
                return;
            }
            if (!IsMenu(winMenuObject))
            {
                Log.Error(this, "Start", "WinMenu menu is not a plausible menu, as it is not IOpenable");
                return;
            }*/
            if (!IsMenu(interactionMenuObject))
            {
                Log.Error(this, "Start", "Interaction menu is not a plausible menu, as it is not IOpenable");
                return;
            }
            if (!IsCursor(cursorObject))
            {
                Log.Error(this, "Start", "Cursor is not a plausible cursor, as it is not IEnableable");
                return;
            }

            //deathMenu = deathMenuObject.GetComponent<IOpenable>();
            //winMenu = winMenuObject.GetComponent<IOpenable>();
            interactionMenu = interactionMenuObject.GetComponent<IOpenable>();
            cursor = cursorObject.GetComponent<IEnableable>();
        } 

        public static bool IsMenu(GameObject obj)
        {
            return obj != null && obj.TryGetComponent<IOpenable>(out _);
        }

        public static bool IsCursor(GameObject obj)
        {
            return obj != null && obj.TryGetComponent<IEnableable>(out _);
        }
    }
}