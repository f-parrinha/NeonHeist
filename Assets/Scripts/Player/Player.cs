using Character;
using Core.Common.Queue;
using Player.Cameras;
using Player.Controller;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Class <c> Player </c> is the base class for the player gameObject. Contains generic methods and access to its main components
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerPhysics))]
    [RequireComponent(typeof(PlayerStances))]
    [RequireComponent(typeof(PlayerInteraction))]
    [RequireComponent(typeof(PlayerLeaning))]
    [RequireComponent(typeof(PlayerGunController))]
    [RequireComponent(typeof(CharacterHealth))]
    public class Player : MonoBehaviour
    {
        public const string TAG = "Player";

        // Main components
        private PlayerMovement pMovement;
        private PlayerPhysics pPhysics;
        private PlayerCamera pCamera;
        private PlayerStances pStances;
        private PlayerInteraction pInteraction;
        private PlayerLeaning pLeaning;
        private PlayerGunController pGunController;
        private CharacterAmmo pCharacterAmmo;
        private CharacterHealth pCharacterHealth;
        private CharacterVoices pCharacterVoices;

        private Vector3Queue rotQueue;
        private Vector3 startRotation;

        public PlayerCamera Camera { get => pCamera = pCamera != null ? pCamera : GetComponentInChildren<PlayerCamera>(); }
        public PlayerPhysics Physics { get => pPhysics = pPhysics != null ? pPhysics : GetComponent<PlayerPhysics>(); }
        public PlayerMovement Movement { get => pMovement = pMovement != null ? pMovement : GetComponent<PlayerMovement>(); }
        public PlayerStances Stances { get => pStances = pStances != null ? pStances : GetComponent<PlayerStances>(); }
        public PlayerInteraction Interaction { get => pInteraction = pInteraction != null ? pInteraction : GetComponent<PlayerInteraction>(); }
        public PlayerLeaning Leaning { get => pLeaning = pLeaning != null ? pLeaning : GetComponent<PlayerLeaning>(); }
        public PlayerGunController GunController { get => pGunController = pGunController != null ? pGunController : GetComponent<PlayerGunController>(); }
        public CharacterAmmo Ammo { get => pCharacterAmmo = pCharacterAmmo != null ? pCharacterAmmo : GetComponent<CharacterAmmo>(); }
        public CharacterHealth Health { get => pCharacterHealth = pCharacterHealth != null ? pCharacterHealth : GetComponent<CharacterHealth>(); }
        public CharacterVoices Voices { get => pCharacterVoices = pCharacterVoices != null ? pCharacterVoices : GetComponent<CharacterVoices>(); }


        private void OnValidate()
        {
            ValidateHasCameraChild();
        }

        private void Start()
        {
            rotQueue = new Vector3Queue();
            startRotation = transform.rotation.eulerAngles;

            Health.AddOnDamageHandler((sender, args) => Voices.PlayDamageVoice());
        }

        private void LateUpdate()
        {
            transform.localRotation = Quaternion.Euler(startRotation + rotQueue.Evaluate());
        }


        public void Rotate(object rotator, Vector3 value)
        {
            rotQueue.Set(rotator, value);
        }

        private void ValidateHasCameraChild()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<PlayerCamera>() != null) return;
            }

            var camera = new GameObject("Camera").AddComponent<PlayerCamera>();
            camera.transform.SetParent(transform);
        }
    }
}