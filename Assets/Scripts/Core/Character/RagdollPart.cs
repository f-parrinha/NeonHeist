using Core.Common.Interfaces;
using UnityEngine;

namespace Core.Character
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class RagdollPart : MonoBehaviour, IEnableable
    {
        private Rigidbody rb;
        private Collider col;

        public bool IsEnabled { get; private set; }

        private void Awake() 
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            Enable();
            Disable();
        }

        public void Enable()
        {
            if (IsEnabled) return;

            rb.isKinematic = false;
            col.isTrigger = false;
            IsEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled) return;

            rb.isKinematic = true;
            col.isTrigger = true;
            IsEnabled = false;
        }
    }
}