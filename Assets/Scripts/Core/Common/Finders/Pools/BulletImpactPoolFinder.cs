using Core.Controllers;
using Core.Props.Interfaces.Pools;
using UnityEngine;

namespace Core.Common.Finders.Pools
{
    public class BulletImpactPoolFinder
    {
        public const string NAME = "BulletImpact Pool";

        private IPool pool;
        public IPool Find()
        {
            if (pool != null) return pool;

            return pool = GameObject.Find(NAME).GetComponent<IPool>();
        }
    }
}