using Core.Common.Interfaces;
using Core.Props.Interfaces.Pools;
using Core.Utilities;
using System.Collections.Generic;
using UnityEngine;


namespace Props
{
    /// <summary>
    /// Works as a pooling system for props in the scene
    /// </summary>
    public class PropPool : MonoBehaviour, IInitializable, IPool
    {
        [SerializeField] private GameObject objectToPool;
        [SerializeField] private int size = 100;

        private LinkedList<IPooledObject> pool;
        private LinkedListNode<IPooledObject> current;

        public bool IsInitialized { get; private set; }
        public Transform Transform => transform;

        public void Initialize()
        {
            if (IsInitialized) return;
            if (!objectToPool.TryGetComponent<IPooledObject>(out _))
            {
                Log.Warning(this, "Initialize", "The given object is not IPooledObject");
                return;
            }

            pool = new LinkedList<IPooledObject>();

            // Load pool
            if (size > 0) 
            { 
                for (int i = 0; i < size; i++)
                {
                    GameObject obj = Instantiate(objectToPool, transform);
                    obj.TryGetComponent<IPooledObject>(out var pooledObj);
                    pooledObj.Setup(this);
                    pooledObj.SetActive(false);
                    pool.AddLast(pooledObj);
                }

                current = pool.First;
            }

            IsInitialized = true;
        }

        private void Awake()
        {
            Initialize();
        }

        public IPooledObject Next()
        {
            if (pool.Count == 0)
            {
                return null;
            }

            var res = current;

            current = current == pool.Last ? pool.First : current.Next;

            IPooledObject obj = res.Value;
            obj.SetActive(true);
            obj.Restart();
            return obj;
        }

        public IPooledObject NextAt(Vector3 position, Quaternion rotation)
        {
            IPooledObject res = Next();
            if (res == null)
            {
                return null;
            }

            res.Transform.SetPositionAndRotation(position, rotation);
            return res;
        }
    }
}
