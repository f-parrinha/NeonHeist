using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    public enum SwarmType { Volt, Bang, Ghost, Builder }

    public GameObject voltPrefab;
    public GameObject bangPrefab;
    public GameObject ghostPrefab;
    public GameObject builderPrefab;

    public Transform targetPoint;

    public void SpawnAndSendBot(SwarmType type)
    {
        GameObject prefab = GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogWarning("Prefab null para tipo: " + type);
            return;
        }

        Vector3 spawnPos = transform.position;
        Debug.Log("Spawnando bot tipo: " + type + " na posição: " + spawnPos);

        GameObject bot = Instantiate(prefab, spawnPos, Quaternion.identity);

        SwarmBot swarmBot = bot.GetComponent<SwarmBot>();
        if (swarmBot != null)
        {
            Debug.Log("Chamando MoveTo no bot do tipo: " + type);
            swarmBot.MoveTo(targetPoint.position);
        }
        else
        {
            Debug.LogWarning("SwarmBot não encontrado no prefab do tipo: " + type);
        }
    }


    private GameObject GetPrefab(SwarmType type)
    {
        return type switch
        {
            SwarmType.Volt => voltPrefab,
            SwarmType.Bang => bangPrefab,
            SwarmType.Ghost => ghostPrefab,
            SwarmType.Builder => builderPrefab,
            _ => null
        };
    }
}
/*using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    [Header("Swarm Prefabs")]
    [SerializeField] private GameObject voltPrefab;
    [SerializeField] private GameObject bangPrefab;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject builderPrefab;

    public enum SwarmType { Volt, Bang, Ghost, Builder }

    [Header("Spawn Settings")]
    [SerializeField] private Transform defaultSpawnPoint; // Se não for passado um, usa este

    // Método público para instanciar swarm
    public void SpawnBot(SwarmType type, Transform target = null, Vector3? spawnPos = null)
    {
        GameObject botPrefab = GetPrefabByType(type);

        if (botPrefab != null)
        {
            Vector3 positionToSpawn = spawnPos ?? defaultSpawnPoint.position;
            GameObject botInstance = Instantiate(botPrefab, positionToSpawn, Quaternion.identity);

            // Define o alvo se tiver script base
            SwarmBot swarmScript = botInstance.GetComponent<SwarmBot>();
            if (swarmScript != null && target != null)
            {
                swarmScript.SetTarget(target);
            }
        }
        else
        {
            Debug.LogWarning($"Prefab para o tipo {type} não está atribuído.");
        }
    }

    // Retorna o prefab correto com base no tipo
    private GameObject GetPrefabByType(SwarmType type)
    {
        switch (type)
        {
            case SwarmType.Volt: return voltPrefab;
            case SwarmType.Bang: return bangPrefab;
            case SwarmType.Ghost: return ghostPrefab;
            case SwarmType.Builder: return builderPrefab;
            default: return null;
        }
    }
}
*/