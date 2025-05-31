using UnityEngine;

public class BangSwarm : SwarmBot
{
    public float explosionRadius = 3f;
    public GameObject explosionEffect;

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Destructible"))
            {
                Destroy(hit.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
