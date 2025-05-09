using Core.Common.Interfaces.Info;
using Core.Controllers;
using UnityEngine;

namespace Core.Common.Finders
{
    public class GameControllerFinder 
    {
        public const string NAME = "GameController";

        private GameController gameController;

        public GameController Find()
        {
            if (gameController != null) return gameController;

            return gameController = GameObject.Find(NAME).GetComponent<GameController>();
        }
    }
}