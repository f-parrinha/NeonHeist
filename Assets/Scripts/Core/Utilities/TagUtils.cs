using System.Linq;
using UnityEngine;

namespace Core.Utilities
{
    public class TagUtils
    {
        public static bool IsAgent(Collider collider) => collider.CompareTag("Player") || collider.CompareTag("Agent");
        public static bool MultiCompare(string mainTag, params string[] tags)
        {
            if (tags == null)
            {
                return false;
            }

            return tags.Contains(mainTag);
        }
    }
}