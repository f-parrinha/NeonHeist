using UnityEngine;

namespace Core.Props.Interfaces.Pools
{
    public interface IPool
    {
        Transform Transform { get; }

        IPooledObject Next();
        IPooledObject NextAt(Vector3 position, Quaternion rotation);
    }
}