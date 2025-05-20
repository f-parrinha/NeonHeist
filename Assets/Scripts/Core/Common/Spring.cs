using UnityEngine;

namespace Core.Utilities
{
    public class Spring
    {
        /** Constants */
        const int MAX_ACTIONS = 10;
        const int MAX_T = 10000;
        const int MASS = 1;

        /** Variables */
        private int actionsCounter;
        private readonly SpringAction[] actions;

        /** Properties */
        public float AnimationSmoothness { get; private set; }
        public float b { get; private set; }    // Damping constant
        public float k { get; private set; }    // Restitution constant
        public float c { get; private set; }    // Restitution by mass ratio
        public Vector3 Value { get; private set; }

        public Spring(float b, float k, float animationSmoothness)
        {
            this.b = b;
            this.k = k;
            c = k / MASS;
            actions = new SpringAction[MAX_ACTIONS];
            actionsCounter = 0;
            Value = Vector3.zero;
            AnimationSmoothness = animationSmoothness;
        }

        public void Simulate()
        {
            Vector3 actionSum = Vector3.zero;
            for (int i = 0; i < actionsCounter; i++)
            {
                SpringAction action = actions[i];
                actionSum += PlayDampedAction(ref action);
            }

            Value = Vector3.Lerp(Value, actionSum, AnimationSmoothness * Time.deltaTime);
        }

        /// <summary>
        /// Simulates a spring movement using SHM on only on axis
        /// </summary>
        /// <param name="targetPosition"> </param>
        /// <returns></returns>
        public float PlaySHMAction(ref SpringAction action)
        {
            float res = action.targetPoint.x * Mathf.Exp(-b * action.tRef) * Mathf.Cos(Mathf.Sqrt(k) * action.tRef);

            action.tRef += action.tRef < MAX_T ? Time.deltaTime : action.tRef;
            return res;
        }

        /// <summary>
        /// Simulates a spring movement using SHM
        /// </summary>
        /// <param name="targetPosition"> </param>
        /// <returns></returns>
        public Vector3 PlayDampedAction(ref SpringAction action)
        {
            float x = action.targetPoint.x * Mathf.Exp(-b * action.tRef) * Mathf.Cos(Mathf.Sqrt(k) * action.tRef);
            float y = action.targetPoint.y * Mathf.Exp(-b * action.tRef) * Mathf.Cos(Mathf.Sqrt(k) * action.tRef);
            float z = action.targetPoint.z * Mathf.Exp(-b * action.tRef) * Mathf.Cos(Mathf.Sqrt(k) * action.tRef);

            action.tRef += action.tRef < MAX_T ? Time.deltaTime : action.tRef;
            return new(x, y, z);
        }

        /// <summary>
        /// Adds a new spring action to the set of actions to execute
        /// </summary>
        /// <param name="point"> action target point </param>
        public void AddSpringAction(Vector3 point)
        {
            if (actionsCounter == MAX_ACTIONS) { actionsCounter = 0; }

            actions[actionsCounter++] = new SpringAction { targetPoint = point };
        }

        /// <summary>
        /// Class <c>SpringAction</c> represents a single action to be taken care of by the spring
        /// </summary>
        public class SpringAction
        {
            /** Variables */
            private float t;

            /** Properties */
            public ref float tRef { get { return ref t; } }
            public Vector3 targetPoint { get; set; }
        }
    }
}
