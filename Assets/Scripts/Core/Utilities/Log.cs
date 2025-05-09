using UnityEngine;

namespace Core.Utilities
{
    public class Log
    {
        public static void Info(object obj, string methodName, string message)
        {
            var name = obj is MonoBehaviour monoObj ? monoObj.name : obj.GetType().Name;

            Debug.Log($"{name} -> {methodName}(): {message}");
        }

        public static void Warning(object obj, string methodName, string message)
        {
            var name = obj is MonoBehaviour monoObj ? monoObj.name : obj.GetType().Name;
            
            Debug.LogWarning($"{name} -> {methodName}(): {message}");
        }

        public static void Error(object obj, string methodName, string message)
        {
            var name = obj is MonoBehaviour monoObj ? monoObj.name : obj.GetType().Name;
            
            Debug.LogError($"{name} -> {methodName}(): {message}");
        }

    }
}