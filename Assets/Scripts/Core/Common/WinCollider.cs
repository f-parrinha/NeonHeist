using Core.Common.Finders;
using Core.Health.Interfaces;
using UnityEngine;

public class WinCollider : MonoBehaviour
{
    private GameControllerFinder gameControllerFinder;

    private void Start()
    {
        gameControllerFinder = new GameControllerFinder();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameControllerFinder.Find().Win();
        }
    }
}
