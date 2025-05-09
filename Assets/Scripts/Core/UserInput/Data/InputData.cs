using Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UserInput.Data
{
    [CreateAssetMenu(fileName = "InputData", menuName = "Scriptable Objects/Core/InputData")]
    public class InputData : ScriptableObject, IInitializable
    {
        [NonSerialized] private Dictionary<string, InputEntry> data;
        [NonSerialized] private bool isInitialized;

        [SerializeField] private List<InputEntry> entries;

        public bool IsInitialized => isInitialized;

        private void OnEnable()
        {
            Initialize();
        }


        public void Initialize()
        {
            if (IsInitialized) return;

            LoadInputEntries();

            isInitialized = true;
        }

        public InputEntry Get(string key)
        {
            if (!isInitialized || !data.ContainsKey(key))
            {
                Utilities.Log.Warning(this, "Get", $"Undefined key ({key})");
                return null;
            }

            return data[key];
        }

        private void LoadInputEntries()
        {
            const string methodName = "LoadInputEntries";

            data = new Dictionary<string, InputEntry>();

            foreach (var entry in entries)
            {
                if (entry.Name == string.Empty)
                {
                    Utilities.Log.Warning(this, methodName, "Empty key name found. Skipping");
                }
                if (data.ContainsKey(entry.Name))
                {
                    Utilities.Log.Warning(this, methodName, $"Duplicate key was found ({entry.Name}). Skipping");
                    continue;
                }

                data.Add(entry.Name, entry);
            }
        }
    }
}