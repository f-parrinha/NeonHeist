using Core.Hacking;
using Core.Hacking.Interfaces;
using Core.Interactions;
using Core.Utilities;
using UnityEngine;

namespace Props.Common
{
    public class SecurityCameraTerminal : MultiInteractable, IHackeable
    {
        [SerializeField] private SecurityCamera hackableCamera;
        [SerializeField] private GameObject hackSystemObject;       // Terminal or smth
        [SerializeField] private int hackDifficulty = 15;

        private IHackSystem hackSystem;
        public IHackSystem HackSystem => hackSystem;

        public bool IsHacked { get; private set; }


        private void Start()
        {
            if (!hackSystemObject.TryGetComponent<IHackSystem>(out hackSystem))
            {
                Log.Warning(this, "Start", "Hacking system object is not IHackSystem");
                return;
            }

            SetInteractions(new Interaction("Hack", StartHack));
            hackSystem.AddUponHackHandler((sender, args) => UponHack(args.Successful));
        }

        public void StartHack(Transform interactor)
        {
            if (IsHacked) return;
            if (interactor.TryGetComponent<HackCheatBearer>(out var cheatBearer))
            {
                hackSystem.CheatMode = cheatBearer.CanCheat;
            }

            hackSystem.HackDifficulty = hackDifficulty;
            hackSystem.Open();
        }


        public void UponHack(bool success)
        {
            if (IsHacked) return;

            if (success)
            {
                hackableCamera.Disable();
                IsHacked = true;
            }
            else
            {
                hackableCamera.Alarm();
            }
        }
    }
}