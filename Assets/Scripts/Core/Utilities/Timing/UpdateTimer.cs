using UnityEngine;

namespace Core.Utilities.Timing
{
    /// <summary>
    /// Class <c> UpdateTimer </c> : Defines the properties of a timer (alarm clock ?) that can be checked on an update function
    /// </summary>
    public class UpdateTimer
    {
        /** Properties */
        public float Time { get; private set; }


        public UpdateTimer()
        {
            Time = 0;
        }

        /// <summary>
        /// Resets the timer clock to 0 if input is not given. Sets to input if given
        /// <param name="start"/>set value</param>
        /// </summary>
        public void ResetTimer(float? start)
        {
            Time = start is null ? 0 : Mathf.Abs(start.Value);
        }


        /// <summary>
        /// Updates timer and checks if it reached its target
        /// </summary>
        /// <param name="target"> target value </param>
        /// <returns> true or false - timer's clock reached the target </returns>
        public bool Play(float target)
        {
            Time += UnityEngine.Time.deltaTime;

            if (Time >= uint.MaxValue) Time = uint.MaxValue;

            return Time >= target;
        }

        /// <summary>
        /// Simply increases the timer counter
        /// </summary>
        public void Play()
        {
            Time += UnityEngine.Time.deltaTime;
        }

        /// <summary>
        /// Checks if the timer's clock reached the target
        /// </summary>
        /// <param name="target"> target value </param>
        /// <returns> true or false - timer's clock reached the target </returns>
        public bool CheckTimer(float target)
        {
            return Time >= target;
        }
    }
}