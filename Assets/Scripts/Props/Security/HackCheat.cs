using Core.Hacking;
using Core.Interactions;
using Props.Pickables;
using UnityEngine;

namespace Props.Security
{
    public class HackCheat : Pickable
    {
        public override void Initialize()
        {
            if (IsInitialized) return;

            SetInteractions(new Interaction("Pick Up", UponPickUp));
            IsInitialized = true;
        }

        private void UponPickUp(Transform interactor)
        {
            if (interactor.TryGetComponent<HackCheatBearer>(out var cheatBearer))
            {
                cheatBearer.CanCheat = true;
            }

            Destroy(gameObject);
        }
    }
}