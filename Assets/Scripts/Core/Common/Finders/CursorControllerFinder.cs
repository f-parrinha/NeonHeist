using Core.Controllers;
using Core.Utilities.DesignPatterns;
using UnityEngine;

namespace Core.Common.Finders
{
    public class CursorControllerFinder : GenericSingleton<CursorControllerFinder>
    {
        public const string NAME = "CursorController";

        private CursorController controller;

        public CursorController Find()
        {
            if (controller != null) return controller;

            return controller = GameObject.Find(NAME).GetComponent<CursorController>();
        }
    }
}