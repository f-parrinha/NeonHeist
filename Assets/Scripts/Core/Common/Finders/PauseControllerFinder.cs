using Core.Common.Interfaces.Info;
using Core.Controllers;
using UnityEngine;

namespace Core.Common.Finders
{
    public class PauseControllerFinder
    {

        public const string NAME = "PauseController";

        private PauseController pauseController;

        public PauseController Find()
        {
            if (pauseController != null) return pauseController;

            return pauseController = GameObject.Find(NAME).GetComponent<PauseController>();
        }
    }
}