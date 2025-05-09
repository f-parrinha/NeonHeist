using Core.Props.Interfaces.Pools;
using Core.Utilities.Timing;
using UnityEngine;

namespace Props
{
    public class BulletImpact : MonoBehaviour, IPooledObject
    {
        private IPool pool;
        private TickTask resetParentTask;

        [SerializeField] private ParticleSystem particle;
        [SerializeField] private float parentResetTimer;

        public float ParentResetTimer => parentResetTimer;
        public Transform Transform => transform;
        public IPool Pool => pool;

        public void Setup(IPool pool)
        {
            this.pool = pool;
            resetParentTask = new TickTask(TimeUtils.FracToMilli(parentResetTimer), () => transform.SetParent(pool.Transform));
        }

        public void SetParent(Transform parent)
        {
            //transform.SetParent(parent);
            resetParentTask.Restart();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Restart()
        {
            particle.Play();
        }
    }
}