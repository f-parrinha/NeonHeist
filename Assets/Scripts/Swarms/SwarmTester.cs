using UnityEngine;

public class SwarmTester : MonoBehaviour
{
    public SwarmManager swarmManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            swarmManager.SpawnAndSendBot(SwarmManager.SwarmType.Volt);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            swarmManager.SpawnAndSendBot(SwarmManager.SwarmType.Bang);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            swarmManager.SpawnAndSendBot(SwarmManager.SwarmType.Ghost);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            swarmManager.SpawnAndSendBot(SwarmManager.SwarmType.Builder);
    }
}
