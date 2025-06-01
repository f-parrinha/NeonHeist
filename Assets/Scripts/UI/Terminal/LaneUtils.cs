using Core.Utilities;
using System;
using UnityEngine;

namespace UI.Terminal
{
    public sealed class LaneUtils
    {
        private static readonly int LANE_COUNT = Enum.GetValues(typeof(Lane)).Length;

        public static bool AreLanesOpposite(int lane1, int lane2)
        {
            return Mathf.Abs(lane1 - lane2) == 2;
        }

        public static bool AreLanesOpposite(Lane lane1, Lane lane2)
        {
            int lane1Int = LaneToInt(lane1);
            int lane2Int = LaneToInt(lane2);
            return AreLanesOpposite(lane1Int, lane2Int);
        }

        public static int LaneToInt(Lane lane)
        {
            return (int) lane;
        }

        public static Lane IntToLane(int value)
        {
            int bounded = value % LANE_COUNT;

            switch (bounded)
            {
                case 0:
                    return Lane.Top;
                case 1:
                    return Lane.Left;
                case 2:
                    return Lane.Bottom;
                case 3:
                    return Lane.Right;
                default:
                    Log.Warning(new LaneUtils(), "IntToLane", "Unknown lane was found. Returning defaul (Lane.Top)");
                    return Lane.Top;
            }
        }
    }
}