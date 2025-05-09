using Core.Utilities.DesignPatterns;
using UnityEngine;

namespace Core.Common.Finders.UI
{
    public class CanvasFinder : GenericSingleton<CanvasFinder>
    {
        public const string FIXED_NAME = "FixedCanvas";
        public const string RESIZABLE_NAME = "ResizableCanvas";

        private Canvas fixedCanvas;
        private Canvas resizableCanvas;

        public Canvas Fixed()
        {
            if (fixedCanvas != null) return fixedCanvas;

            return fixedCanvas = GameObject.Find(FIXED_NAME).GetComponent<Canvas>();
        }

        public Canvas Resizable()
        {
            if (resizableCanvas != null) return resizableCanvas;

            return resizableCanvas = GameObject.Find(RESIZABLE_NAME).GetComponent<Canvas>();

        }
    }
}