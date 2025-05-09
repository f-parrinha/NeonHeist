using UnityEngine;

namespace Core.UserInput
{
    /// <summary>
    /// Class <c> InputSystemStarter </c> handles the initialization, or rather, garantees the InputSystem's initialization
    ///    , so other objects in the scene can interact with it. It is REQUIRED on every scene to use the InputSystem
    /// </summary>
    public class InputSystemStarter : MonoBehaviour
    {
        [SerializeField] private InputSystem input;

        private void Awake()
        {
            input.Initialize();
        }
    }
}