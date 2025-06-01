using AI.Enums;
using System;
using UnityEngine;

namespace AI.Agents
{
    public class SimulationAgent : MonoBehaviour
    {
        [SerializeField] private Faction faction;

        public string ID { get; private set; }
        public Faction Faction => faction;

        private void Awake()
        {
            ID = Guid.NewGuid().ToString();
        }
    }
}