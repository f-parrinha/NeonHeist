using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Utilities
{
    public class UIUtils
    {
        public static List<Transform> GetUIElementsOnCursor()
        {
            PointerEventData eventData = new(EventSystem.current) { position = Input.mousePosition };
            List<Transform> results = new();

            if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
            {
                return results;
            }

            List<RaycastResult> rayResults = new();
            EventSystem.current.RaycastAll(eventData, rayResults);

            rayResults.ForEach((RaycastResult result) => results.Add(result.gameObject.transform));
            return results;
        }
    }
}