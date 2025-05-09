using Core.Common.Interfaces;
using Core.Common.Interfaces.Info;
using System.Collections.Generic;
using UnityEngine;


namespace World.Props
{
    public class Door : MonoBehaviour, IInteractable, IInfoHolder
    {
        private const string OPEN = "Open";
        private const string CLOSE = "Close";

        private readonly List<string> AnimatorTriggers = new() { OPEN, CLOSE };

        [SerializeField] private Animator hingeAnimator;

        public bool IsOpened { get; private set; }
        private bool IsAnyAnimationPlaying => hingeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f || hingeAnimator.IsInTransition(0);

        public string GetInfo()
        {
            return IsOpened ? "Close" : "Open";
        }

        public void Interact(Transform interactor)
        {
            if (IsAnyAnimationPlaying) return;

            AnimatorTriggers.ForEach((trigger) => hingeAnimator.ResetTrigger(trigger));

            IsOpened = !IsOpened;

            hingeAnimator.SetTrigger(IsOpened ? OPEN : CLOSE);
        }
    }
}
