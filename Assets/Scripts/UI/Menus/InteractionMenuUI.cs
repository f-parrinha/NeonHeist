using Core.Interactions;
using Core.Common.Interfaces;
using Core.UserInput;
using UI.Components;
using UnityEngine;
using System.Collections.Generic;

namespace UI.Menus
{
    public class InteractionMenuUI : MonoBehaviour, IOpenable, IInteractionerHolder
    {
        // Useful cause he is the interactor transform
        private Transform interactor;

        private InteractionButtonUI currentButton;
        private InteractionButtonUI[] buttons;
        private int idx;

        [SerializeField] private InteractionButtonUI buttonPrefab;
        [SerializeField] private float sensitivity = 0.5f;
        public bool IsOpened => gameObject.activeSelf;

        public MultiInteractable Interactioner { get; private set; }

        private void Start()
        {
            interactor = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            AddScrollWheelControl();
            AddInteractControl();
        }

        public void SetInteractioner(MultiInteractable interactioner)
        {
            if (IsOpened) return;

            Interactioner = interactioner;

            SetupButtons(interactioner);
        }

        public void Open()
        {
            if (gameObject.activeSelf) return;

            idx = 0;
            Refresh();
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (!gameObject.activeSelf) return;

            gameObject.SetActive(false);
        }

        public bool Toggle()
        {
            if (IsOpened)
            {
                Close();
                return false;
            }

            Open();
            return true;
        }

        public void Refresh()
        {
            // Make old selected button inactive
            currentButton.Disable();

            // Get new button
            currentButton = buttons[idx];

            // Make new one active
            currentButton.Enable();
        }
       
        private void AddScrollWheelControl()
        {
            float mouseScroll = InputSystem.Instance.MouseScroll * sensitivity;
            if (mouseScroll == 0 || buttons == null || buttons.Length == 0) return;
            
            // increase idx factor and make it wrap around like a circular array
            int max = buttons.Length;
            idx += mouseScroll < 0 ? 1 : -1;
            idx = Mathf.Clamp(idx, 0, buttons.Length - 1);
            Refresh();
        }

        private void AddInteractControl()
        {
            if (IsOpened && InputSystem.Instance.KeyDown(InputKeys.INTERACT))
            {
                Interactioner.Interact(idx, interactor);
            }
        }

        private void SetupButtons(MultiInteractable interactioner)
        {
            if (interactioner == null || IsOpened) return;

            List<Interaction> interactions = interactioner.GetInteractions();

            // Reset all buttons
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }


            // Create new buttons
            if (interactions.Count == 0) return;
           
            int j = 0;
            buttons = new InteractionButtonUI[interactions.Count];
            foreach (Interaction interaction in interactions)
            {
                InteractionButtonUI newButton = Instantiate(buttonPrefab, transform);
                newButton.Initialize(interaction.Name);
                buttons[j++] = newButton;
            }

            currentButton = buttons[0];
        }
    }
}