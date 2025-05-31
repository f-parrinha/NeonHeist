using Core.Props.Interfaces.Pools;
using Core.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Controllers
{
    [Serializable]
    public class BulletImpactPool
    {
        public enum Type
        {
            Metal,
            Flesh,
            Ground,
        }

        private IPool pool;

        [SerializeField] private Type type;
        [SerializeField] private GameObject poolObject;
        public Type MaterialType => type;
        public IPool Pool => pool == null ? pool = poolObject.GetComponent<IPool>() : pool;

        public bool Validate()
        {
            return poolObject.TryGetComponent<IPool>(out pool);
        }

    }

    public class BulletImpactController : MonoBehaviour
    {
        [SerializeField] BulletImpactPool[] poolEntries;

        private Dictionary<BulletImpactPool.Type, IPool> pools;
        private Dictionary<string, BulletImpactPool.Type> typesByString;

        private void Awake()
        {
            if (!ValidatePools())
            {
                Log.Warning(this, "Start", "The given gameoObjects are not IPool");
                return;
            }

            SetupTypesByString();
            SetupPools();
        }

        public IPool GetPool(BulletImpactPool.Type type)
        {
            return pools[type];
        }

        public IPool GetPool(string type)
        {
            return pools[GetType(type)];
        }

        public BulletImpactPool.Type GetType(string type)
        {
            if (type == null || type == string.Empty) return BulletImpactPool.Type.Ground;
            if (typesByString.ContainsKey(type))
            {
                return typesByString[type];
            }
            else
            {
                return BulletImpactPool.Type.Ground;
            }
        }

        public bool ValidatePools()
        {
            foreach (var pool in poolEntries)
            {
                if (pool.Validate()) continue;
                return false;
            }

            return true;
        }

        private void SetupPools()
        {
            pools = new Dictionary<BulletImpactPool.Type, IPool>();

            foreach (var poolEntry in poolEntries)
            {
                if (pools.ContainsKey(poolEntry.MaterialType))
                {
                    Log.Warning(this, "Start", "Repeated type in pools entries");
                    continue;
                }

                pools.Add(poolEntry.MaterialType, poolEntry.Pool);
            }
        }

        private void SetupTypesByString()
        {
            typesByString = new Dictionary<string, BulletImpactPool.Type>();

            foreach (BulletImpactPool.Type type in Enum.GetValues(typeof(BulletImpactPool.Type))) 
            {
                typesByString.Add(type.ToString(), type);
            }
        }
    }
}