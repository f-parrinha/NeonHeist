using Core.Controllers;
using Core.Props.Interfaces.Pools;
using UnityEngine;

namespace Core.Common.Finders.Pools
{
    public class BulletImpactControllerFinder
    {
        public const string NAME = "BulletImpactController";

        private BulletImpactController controller;
        public BulletImpactController Find()
        {
            if (controller != null) return controller;

            return controller = GameObject.Find(NAME).GetComponent<BulletImpactController>();
        }
    }
}