using Core.Common.Finders.UI;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Info;
using Core.UserInput;
using Core.Utilities;
using Player.Cameras;
using System.Security.Cryptography;
using UnityEngine;

namespace Player.Controller
{
    [RequireComponent(typeof(Player))]
    public class PlayerInteraction : MonoBehaviour
    {
        private PlayerCamera pCamera;
        private InfoTextUIFinder infoTextFinder;

        [SerializeField] private float scannerDistance = 2f;

        private void Start()
        {
            var player = GetComponent<Player>();
            pCamera = player.Camera;

            infoTextFinder = new InfoTextUIFinder();
        }

        private void Update()
        {
            var obj = ScanObject();

            DisplayInfo(obj);
            AddInteractionControl(obj);
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

            if (Physics.Raycast(ray, out var hit, scannerDistance, LayerUtils.Ignore(LayerUtils.PLAYER, LayerUtils.IGNORE_RAYCAST)))
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        private void DisplayInfo(GameObject obj)
        {
            IInfoDisplayable displayable = infoTextFinder.Find();


            if (obj == null || !obj.TryGetComponent<IInfoHolder>(out var info))
            {
                displayable.Display(string.Empty);
                return;
            }

            displayable.Display(info.GetInfo());
        }
    }
}