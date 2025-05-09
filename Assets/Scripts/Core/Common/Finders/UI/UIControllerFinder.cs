using Core.Controllers;
using UnityEngine;

namespace Core.Common.Finders.UI
{
    public class UIControllerFinder
    {
        public const string NAME = "UIController";

        private UIController uiController;

        public UIController Find()
        {
            if (uiController != null) return uiController;

            return uiController = GameObject.Find(NAME).GetComponent<UIController>();
        }
        
    }
}