using UnityEngine;

namespace AI.Events
{
    public class OnTargetFoundArgs
    {
        public float Distance { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Collider Target {  get; set; }
    }
}