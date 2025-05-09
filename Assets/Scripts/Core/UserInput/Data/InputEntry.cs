using System;
using UnityEngine;

namespace Core.UserInput.Data
{
    [Serializable]
    public class InputEntry
    {
        [SerializeField] private string name;
        [SerializeField] private KeyCode keyCode;
        public string Name => name;
        public KeyCode KeyCode => keyCode;
    }
}