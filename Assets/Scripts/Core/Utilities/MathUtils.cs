using UnityEngine;

namespace Core.Utilities
{
    public class MathUtils
    {
        public const short MILIMETER_RESIZER = 1000;
        public const short CENTIMETER_RESIZER = 100;


        /// <summary>
        /// Frame independent lerping. Use this insteand of Mathf.Lerp
        /// </summary>
        /// <returns> Frame independent linear interpolation </returns>
        public static float Lerp(float a, float b, float smoothness, float dt)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Exp(-smoothness * dt));
        }

        /// <summary>
        /// Frame independent lerping for Vecto3. Use this insteand of Vector3.Lerp
        /// </summary>
        /// <returns> Frame independent linear interpolation </returns>
        public static Vector3 VectorLerp(Vector3 a, Vector3 b, float smoothness, float dt)
        {
            return Vector3.Lerp(a, b, 1 - Mathf.Exp(-smoothness * dt));
        }

        /// <summary>
        /// Converts polar points to cartesian points
        /// </summary>
        /// <param name="polarPoint"> point in polar coordinates </param>
        /// <returns> point in cartesian coordinates </returns>
        public static Vector2 Vec2PolarToCartisian(Vector2 polarPoint)
        {
            float x = Mathf.Cos(polarPoint.y) * polarPoint.x;
            float y = Mathf.Sin(polarPoint.y) * polarPoint.x;

            return new(x, y);
        }

        /// <summary>
        /// Checks if the first number is equals to the second, with an error (margin)
        /// </summary>
        /// <param name="number1"> number to be compared </param>
        /// <param name="number2"> number compared to </param>
        /// <param name="margin"> error / margin </param>
        /// <returns> true or false </returns>
        public static bool EqualsWithMargin(float number1, float number2, float margin)
        {
            return number1 > number2 - margin && number1 < number2 + margin;
        }


        /// <summary>
        /// Class  <c> Rand </c> : Gives the possibility mof non repetitive random values
        /// </summary>
        public class Rand
        {
            /** Variables */
            private int counter;

            public Rand()
            {
                counter = 0;
            }

            /// <summary>
            /// Generates a random number that is different from the previous generated number
            /// </summary>
            /// <param name="min"> minimum range </param>
            /// <param name="max"> maximum range </param>
            /// <returns></returns>
            public int NonRepetitive(int min, int max)
            {
                /** Exception handling */
                if (max == 0) { return -1; }         // Checks if the array is empty, return -1 if error
                if (min == max - 1) { return 0; }    // Checks if there is only one element, if so return 0 (first element)

                /** Get non repetitive number if there are at least two elements in the array */
                int lastCounter = counter;

                while (lastCounter == counter)
                {
                    counter = Random.Range(min, max);
                }

                return counter;
            }
        }
    }
}