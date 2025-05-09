using UnityEngine;

namespace Core.Props
{
    public class SelfDesctructable : MonoBehaviour
    {
        [SerializeField] private float timeToDie = 1f;

        private void Start()
        {
            Destroy(gameObject, timeToDie);
        }
    }
}