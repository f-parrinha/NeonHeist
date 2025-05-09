using Core.Common.Interfaces;
using Core.Common.Queue;
using Core.UserInput.Data;
using System;
using UnityEngine;

namespace Core.UserInput
{
    [CreateAssetMenu(fileName = "InputSystem", menuName = "Scriptable Objects/Core/InputSystem")]
    public class InputSystem : ScriptableObject, IInitializable
    {
        [NonSerialized] private static InputSystem instance;
        [NonSerialized] private BoolQueue isActiveQueue;

        [SerializeField] private InputData data;

        public static InputSystem Instance => instance;

        // State properties
        public bool IsInitialized => instance == this;
        public bool IsActive => isActiveQueue.Evaluate();
        public bool IsPaused => !IsActive;


        // Input properties
        public bool IsMoving => MoveAxis != Vector3.zero;
        public Vector3 MoveAxis
        {
            get {
                float x = Convert.ToSingle(Key(InputKeys.RIGHT)) - Convert.ToSingle(Key(InputKeys.LEFT));
                float z = Convert.ToSingle(Key(InputKeys.FORWARD)) - Convert.ToSingle(Key(InputKeys.BACKWARD));

                return IsPaused ? Vector3.zero : Vector3.ClampMagnitude(new(x, 0, z), 1);
            }
        }
        public float MouseX => IsPaused ? 0 : Input.GetAxis("Mouse X");
        public float MouseY => IsPaused ? 0 : Input.GetAxis("Mouse Y");
        public float MouseScroll => IsPaused ? 0 : Input.GetAxis("Mouse ScrollWheel");
        public Vector2 MouseAxis => IsPaused ? Vector2.zero : new(MouseX, MouseY);


        // Input methods
        public bool Key(string key)
        {
            if (IsPaused) return false;

            var entry = data.Get(key);
            return entry != null && Input.GetKey(data.Get(key).KeyCode);
        }
        public bool KeyUp(string key)
        {
            if (IsPaused) return false;

            var entry = data.Get(key);
            return entry != null && Input.GetKeyUp(data.Get(key).KeyCode);
        }
        public bool KeyDown(string key)
        {
            if (IsPaused) return false;

            var entry = data.Get(key);
            return entry != null && Input.GetKeyDown(data.Get(key).KeyCode);
        }

        public bool MouseDown(int mouseIndex) => !IsPaused && Input.GetMouseButtonDown(mouseIndex);
        public bool MouseUp(int mouseIndex) => !IsPaused && Input.GetMouseButtonUp(mouseIndex);
        public bool Mouse(int mouseIndex) => !IsPaused && Input.GetMouseButton(mouseIndex);



        // State methods
        public void SetActive(object setter, bool value) => isActiveQueue.Set(setter, value);
        public void UnsetActive(object setter) => isActiveQueue.Unset(setter);


        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            isActiveQueue = new BoolQueue();

            isActiveQueue.Set(this, true);
            instance = this;
        }
    }
}