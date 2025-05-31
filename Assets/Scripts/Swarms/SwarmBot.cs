using UnityEngine;
using UnityEngine.AI;

public class SwarmBot : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
        Debug.Log("Target destinanion" + destination);
        if (agent != null)
            agent.SetDestination(destination);
    }
}
