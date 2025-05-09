using Character;
using Core.Common.Finders;
using Core.Controllers;
using System;

namespace Player
{
    public class PlayerHealth : CharacterHealth
    {
        private GameControllerFinder gameControllerFinder;

        public override void Initialize()
        {
            base.Initialize();

            gameControllerFinder = new GameControllerFinder();

            AddOnDeathHandler((object sender, EventArgs args) => gameControllerFinder.Find().Lose());
        }
    }
}