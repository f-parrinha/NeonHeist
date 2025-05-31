using UnityEngine;

public class BuilderSwarm : SwarmBot
{
    public float interactionRange = 2f;

    void Update()
    {

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Carryable"))
            {
                // Exemplo: mover objeto
                hit.transform.position = Vector3.MoveTowards(hit.transform.position, transform.position + Vector3.up * 0.5f, 2f * Time.deltaTime);
            }
        }
    }
}
