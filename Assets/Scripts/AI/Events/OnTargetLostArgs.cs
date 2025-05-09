using UnityEngine;

namespace AI.Events
{
    public class OnTargetLostArgs
    {
        public Collider Target { get; set; }
        public Vector3 TargetPosition { get; set; }
    }
}