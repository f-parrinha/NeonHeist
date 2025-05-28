using Core.Common.Finders.UI;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Info;
using Core.Controllers;
using Core.Interactions;
using Core.UserInput;
using Core.Utilities;
using Player.Cameras;
using UnityEngine;

namespace Player.Controller
{
    [RequireComponent(typeof(Player))]
    public class PlayerInteraction : MonoBehaviour
    {
        private PlayerCamera pCamera;
        private InfoTextUIFinder infoTextFinder;
        private UIControllerFinder uiControllerFinder;

        [SerializeField] private float scannerDistance = 2f;

        private void Start()
        {
            var player = GetComponent<Player>();
            pCamera = player.Camera;

            infoTextFinder = new InfoTextUIFinder();
            uiControllerFinder = new UIControllerFinder();
        }

        private void Update()
        {
            var obj = ScanObject();

            DisplayInfo(obj);
            //AddInteractionControl(obj);
        }

        private void AddInteractionControl(GameObject obj)
        {
            if (obj == null) return;

            if (InputSystem.Instance.KeyDown(InputKeys.INTERACT) && obj.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact(transform);
            }
        }


        private GameObject ScanObject()
        {
            var ray = new Ray(pCamera.transform.position, pCamera.transform.forward);

            if (Physics.Raycast(ray, out var hit, scannerDistance, LayerNames.Ignore(LayerNames.PLAYER, LayerNames.IGNORE_RAYCAST)))
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        private void DisplayInfo(GameObject obj)
        {
            UIController uiController = uiControllerFinder.Find();
            IOpenable interactionMenu = uiController.InteractionMenu;
            if (interactionMenu is not IInteractionerHolder interactionerHolder) return;

            if (obj == null || !obj.TryGetComponent<MultiInteractable>(out var interactioner))
            {
                interactionMenu.Close();
                uiController.Cursor.Enable();
                return;
            }

            interactionerHolder.SetInteractioner(interactioner);
            interactionMenu.Open();
            uiController.Cursor.Disable();
        }
    }
}