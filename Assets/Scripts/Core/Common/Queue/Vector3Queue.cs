using Core.Queue.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Common.Queue
{
    public class Vector3Queue : IQueue<Vector3>
    {
        private Dictionary<object, Vector3> queue;

        public Vector3Queue()
        {
            queue = new Dictionary<object, Vector3>();
        }


        public Vector3 Evaluate()
        {
            Vector3 res = Vector3.zero;

            foreach (var entry in queue)
            {
                res += entry.Value;
            }

            queue.Clear();
            return res;
        }

        public void Set(object adder, Vector3 value)
        {
            if (queue.ContainsKey(adder)) return;

            queue.Add(adder, value);
        }

        public void Unset(object adder)
        {
            queue.Remove(adder);
        }
        public bool Contains(object setter)
        {
            return queue.ContainsKey(setter);
        }
    }
}