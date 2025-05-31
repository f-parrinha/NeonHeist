using UnityEngine;

public class VoltSwarm : SwarmBot
{
    public float hackRange = 2f;

    void Update()
    {
       
        Collider[] hits = Physics.OverlapSphere(transform.position, hackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Hackable"))
            {
                print("hack something");
            }
        }
    }
}
