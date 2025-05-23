using Core.Common.Interfaces;
using UnityEngine;

namespace Core.Interactions
{
    public class Interaction : INamed
    {
        public string Name { get; private set; }
        public InteractionFunc Func { get; private set; }

        public Interaction(string name, InteractionFunc func)
        {
            Name = name;
            Func = func;
        }

        public void Interact(Transform interactor)
        {
            Func?.Invoke(interactor);
        }
    }
}