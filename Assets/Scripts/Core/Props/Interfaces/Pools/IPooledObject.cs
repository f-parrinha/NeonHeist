using UnityEngine;

namespace Core.Props.Interfaces.Pools
{
    public interface IPooledObject
    {
        float ParentResetTimer { get; }
        Transform Transform { get; }
        IPool Pool { get; }

        void Setup(IPool pool);
        void SetParent(Transform parent);
        void SetActive(bool active);
        void Restart();
    }
}