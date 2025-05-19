using AI.Agents;
using AI.Enums;
using UnityEngine;

namespace AI.Common
{
    public class ScannedTarget
    {
        public ScannedTarget(float distance, SimulationAgent agent)
        {
            Distance = distance;
            Agent = agent;
        }

        public SimulationAgent Agent{ get; private set; }
        public Vector3 Position => Agent.transform.position;
        public float Distance { get; private set; }
        public float DetectionLevel { get; private set; }

        public void Detect(float detectionFactor, float maxViewDistance = 10)
        {
            float distanceFactor = Distance / maxViewDistance;
            DetectionLevel += detectionFactor / (distanceFactor * distanceFactor);
            DetectionLevel = Mathf.Clamp(DetectionLevel, 0, 100);
        }

        public void Undetect(float falloff)
        {
            DetectionLevel -= falloff;
            DetectionLevel = Mathf.Clamp(DetectionLevel, 0, 100);
        }

        public void Refresh(float distance)
        {
            Distance = distance;
        }

        public override bool Equals(object obj)
        {
            return obj is ScannedTarget other && Agent.ID == other.Agent.ID;
        }

        public override int GetHashCode()
        {
            return Agent.GetHashCode();
        }
    }
}