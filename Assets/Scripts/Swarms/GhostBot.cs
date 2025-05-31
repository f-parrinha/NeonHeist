using UnityEngine;

public class GhostSwarm : SwarmBot
{
    public GameObject smokeEffect;

    public void EmitSmoke()
    {
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
    }
}
