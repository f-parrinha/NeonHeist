using Core.Common.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Interactions
{
    public abstract class MultiInteractable : MonoBehaviour, IInteractable
    {
        private Interaction[] interactions;

        public int InteractionsCount => interactions == null ? 0 : interactions.Length;
 
        private void Start()
        {
            interactions = new Interaction[0];
        }
        public List<Interaction> GetInteractions()
        {
            return new List<Interaction>(interactions);
        }

        public void Interact(Transform interactor)
        {
            Interact(0, interactor);
        }

        public void Interact(int idx, Transform interactor)
        {
            if (interactions == null || idx < 0 || idx > interactions.Length)
            {
                Log.Warning(this, "Act", "Either no actions are defined, or the wrong index was given");
                return;
            }

            interactions[idx].Interact(interactor);
        }

        public void SetInteractions(params Interaction[] interactions)
        {
            this.interactions = interactions;
        }
    }
}