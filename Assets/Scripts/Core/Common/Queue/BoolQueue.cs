using Core.Queue.Interfaces;
using System.Collections.Generic;

namespace Core.Common.Queue
{
    public class BoolQueue : IQueue<bool>
    {
        private Dictionary<object, bool> queue;

        public BoolQueue()
        {
            queue = new Dictionary<object, bool>();
        }

        public bool Evaluate() {
            foreach (var (setter, value) in queue)
            {
                if (value == false) return false;
            }
            
            return true;
        }

        public void Set(object setter, bool value)
        {
            if (queue.ContainsKey(setter))
            {
                queue[setter] = value;
                return;
            }

            queue.Add(setter, value);
        }

        public void Unset(object setter)
        {
            queue.Remove(setter);
        }
        public bool Contains(object setter) => queue.ContainsKey(setter);

        public void Clear()
        {
            queue.Clear();
        }
    }
}