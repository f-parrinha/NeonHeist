using System;
using UnityEngine;

namespace Core.Utilities.Timing
{
    public class TimeUtils
    {
        public const int INVALID_TIME = -1;
        public const int MIN_TIME = 0;
        public const int MAX_HOUR = 24;
        public const int MAX_MIN_SEC = 60;


        public static double NewTimestamp(int year, int month, int day, int hour, int min, int sec, DateTimeKind kind = DateTimeKind.Utc)
        {
            var newDate = new DateTime(year, month, day, hour, min, sec, kind);
            return ((DateTimeOffset)newDate).ToUnixTimeSeconds();
        }

        public static DateTime TimestampToDateTime(double timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)timestamp).UtcDateTime;
        }

        public static float TimeUnitsToFrac(int hour, int minutes, int seconds)
        {
            bool isHourInvalid = hour < MIN_TIME || hour > MAX_HOUR;
            bool isMinutesInvalid = minutes < MIN_TIME || minutes > MAX_MIN_SEC;
            bool isSecondsInvalid = seconds < MIN_TIME || seconds > MAX_MIN_SEC;
            if (isHourInvalid || isMinutesInvalid || isSecondsInvalid) return INVALID_TIME;

            float fracMinutes = (float) minutes / MAX_MIN_SEC;
            float fracSeconds = (float) seconds / (MAX_MIN_SEC * MAX_MIN_SEC);
            return hour + fracMinutes + fracSeconds;
        }

        public static void FracToTimeUnits(float fracTime, out int hour, out int minutes, out int seconds)
        {
            if (fracTime < MIN_TIME || fracTime > MAX_HOUR)
            {
                hour = INVALID_TIME; minutes = INVALID_TIME; seconds = INVALID_TIME;
                return;
            }

            hour = (int) fracTime;

            float minutesFrac = (fracTime - hour) * 60f;
            minutes = (int) minutesFrac;

            seconds = (int) ((minutesFrac - minutes) * 60f);
        }

        /// <summary>
        /// Converts fractional time to milliseconds : (FLOAT) -> INT
        /// <para>
        ///     PRE : It is assumed time is in seconds
        /// </para>
        /// </summary>
        /// <param name="fracTime"> fractional time in FLOAT </param>
        /// <returns> time in milliseconds </returns>
        public static int FracToMilli(float fracTime)
        {
            return (int)(fracTime * 1000);
        }

        /// <summary>
        /// Converts time to milliseconds : (INT) -> INT
        /// <para>
        ///     PRE : It is assumed time is in seconds
        /// </para>
        /// </summary>
        /// <param name="time"> time in seconds </param>
        /// <returns> time in milliseconds </returns>
        public static int ToMilli(int time)
        {
            return time * 1000;
        }


        /// <summary>
        /// Converts time to milliseconds : (INT) -> INT
        /// <para>
        ///     PRE : It is assumed time is in milli seconds
        /// </para>
        /// </summary>
        /// <param name="time"> time in milli seconds </param>
        /// <returns> time in milliseconds </returns>
        public static int ToSeconds(int time)
        {
            return time / 1000;
        }
    }
}