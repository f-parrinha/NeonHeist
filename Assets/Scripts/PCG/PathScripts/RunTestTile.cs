using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;


[Serializable]
public class TileTypePrefabList
{
    public TileType type;
    public List<GameObject> prefabs;
}

public class RunTestTile : MonoBehaviour
{

    [SerializeField] private Transform _startPoint;
    [SerializeField] private List<TileTypePrefabList> _tileTypePrefabs;
    [SerializeField] private NavMeshSurface playerNavMesh;

    private Dictionary<TileType, List<GameObject>> tilePrefabsByType;
    //List<NavMeshAgent> agents;

    void Start()
    {
        tilePrefabsByType = _tileTypePrefabs.ToDictionary(t => t.type, t => t.prefabs);
        //agents = new List<NavMeshAgent>();
        Generate();
      
        playerNavMesh.BuildNavMesh();

        //EnableAgents(agents);

    }

    private void Generate()
    {

        Tile currentTile = GenerateFirstTile();
        
        GenerateTiles(currentTile);
      
    }

    private Tile GenerateFirstTile()
    {
        List<GameObject> possibleIntroPrefabs = tilePrefabsByType[TileType.Straight];
        GameObject startTileObj = possibleIntroPrefabs[Random.Range(0, possibleIntroPrefabs.Count)];

        Tile startTile = startTileObj.GetComponent<Tile>();

        List<Transform> entryPointList = startTile.getEntryPoints();

        Transform entryPoint = entryPointList[0]; //smmpre começa com o straight

        Vector3 tileOffset = startTile.transform.position - entryPoint.position;
        Vector3 pos = _startPoint.position + (_startPoint.rotation * tileOffset);

        GameObject tile = Instantiate(startTileObj, pos, _startPoint.rotation);

        return tile.GetComponent<Tile>();

    }

    private void GenerateTiles(Tile firstTile)
    {
        
       
        Tile currentTile = firstTile;

        for (int rep = 0; rep < 15; rep++) // how many iteration
        {
            //Get Random Type
            TileType randomType = currentTile.randomTileType();
            Debug.Log("Type ->" + randomType);

            List<GameObject> possiblePrefabs = tilePrefabsByType[randomType];
            GameObject nextTileObj = possiblePrefabs[Random.Range(0, possiblePrefabs.Count)];
            Tile nextTileScript = nextTileObj.GetComponent<Tile>();
           
            //Get exit points
            Transform exitPoint; 
            List<Transform> exitPointList = currentTile.getExitPoints();

            if (exitPointList.Count == 1)
                exitPoint = exitPointList[0];
            else
            {
                int randomExit = Random.Range(0, exitPointList.Count);
                exitPoint = exitPointList[randomExit];
            }

            //Get entry points
            List<int> entryIndexToFix = new List<int>();
            Transform entryPoint; 

            List<Transform> entryPointList = nextTileScript.getEntryPoints();

            if (entryPointList.Count == 1)
                entryPoint = entryPointList[0];
            else
            {
                int randomEntry = Random.Range(0, entryPointList.Count);
                entryPoint = entryPointList[randomEntry];

                for(int i = 0; i < entryPointList.Count; i++)
                {
                    if(i != randomEntry)
                        entryIndexToFix.Add(i);
                }
            }

           
            Quaternion targetRotation = exitPoint.rotation;


            Debug.Log("rotation target " + targetRotation);

            Vector3 tileOffset = -Vector3.Scale(entryPoint.localPosition, nextTileObj.transform.localScale);
            Vector3 targetPosition = exitPoint.position +  (targetRotation * tileOffset);
            Debug.Log("position target " + targetPosition);


            //check overlap
            Vector3 boxCenter = targetPosition + targetRotation * Vector3.Scale(nextTileScript.getBoxCollider().center, nextTileObj.transform.localScale);
            Vector3 boxHalfExtents = Vector3.Scale(nextTileScript.getBoxCollider().size, nextTileObj.transform.localScale) * 0.5f;// * 0.99f;
            DebugDrawBox(boxCenter, boxHalfExtents, targetRotation, Color.red, 1000f);
            if (checkOverlap(boxCenter, boxHalfExtents, targetRotation))
                continue;


            GameObject nextTile = Instantiate(nextTileObj, targetPosition, targetRotation);


            if (entryIndexToFix.Count > 0) {
                InstantiateEntryFiller(entryIndexToFix, entryPointList, nextTile);
            }
            
            
           /* if(randomType == TileType.Room)
            {
                var agent = nextTile.GetComponentInChildren<NavMeshAgent>();

                agents.Add(agent);

               
            }*/
            

            nextTile.layer = LayerMask.NameToLayer("PCG");

            currentTile = nextTile.GetComponent<Tile>();

        }

        InstantiateLastWall(currentTile);

       
    }

    private void EnableAgents(List <NavMeshAgent> agents)
    {
        foreach( NavMeshAgent agent in agents)
        {

        if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.transform.position = hit.position;
            agent.enabled = true; // only enable it when you're sure it's on a valid surface
        }
        else
        {
            Debug.LogWarning("Agent too far from NavMesh!");
        }

        }
        // Snap it to NavMesh
    }

    private bool checkOverlap(Vector3 boxCenter, Vector3 boxHalfExtents, Quaternion targetRotation)
    {
       

        Collider[] hitColliders = Physics.OverlapBox(
                boxCenter,
                boxHalfExtents,
                targetRotation,
                LayerMask.GetMask("PCG")
            );

        if (hitColliders.Length > 0)
        {
            Debug.Log("Stopped!");
            return true;
            //nao instancia, continua o loop sem instanciar os que fazem colide
        }

        return false;
    }

    private void InstantiateEntryFiller(List<int> entryIndexToFix, List<Transform> entryPointList, GameObject nextTile )
    {
        for (int i = 0; i < entryIndexToFix.Count; i++)
        {
            int index = entryIndexToFix[i];
            List<GameObject> possibleWallsPrefabs = tilePrefabsByType[TileType.Wall];
            GameObject wallTileObj = possibleWallsPrefabs[Random.Range(0, possibleWallsPrefabs.Count)];

            GameObject wall = Instantiate(wallTileObj, nextTile.transform);
            wall.transform.localPosition = entryPointList[index].localPosition + new Vector3(0, 0.45f, 1.8f); //??
            wall.transform.localRotation = entryPointList[index].localRotation;



        }
    }

    private void InstantiateLastWall(Tile currentTile)
    {
        for (int i = 0; i < currentTile.getExitPoints().Count; i++)
        {
            GameObject lastTile = currentTile.gameObject;
            List<GameObject> possibleEndPrefabs = tilePrefabsByType[TileType.End];
            GameObject wallTileObj = possibleEndPrefabs[Random.Range(0, possibleEndPrefabs.Count)];

            GameObject lastWall = Instantiate(wallTileObj, lastTile.transform);
            lastWall.transform.localPosition = currentTile.getExitPoints()[i].transform.localPosition + (currentTile.getExitPoints()[i].transform.localRotation * new Vector3(0, 0.45f, -1.8f)); //?? * rotation?
            lastWall.transform.localRotation = currentTile.getExitPoints()[i].transform.localRotation;
            Debug.Log("instantiated last wall");

        }
    }

    void DebugDrawBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, Color color, float duration = 0f)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(center, orientation, halfExtents * 2f);
        Vector3[] verts = new Vector3[8]
        {
        cubeTransform.MultiplyPoint3x4(new Vector3(-0.5f, -0.5f, -0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(0.5f, -0.5f, -0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(0.5f, -0.5f, 0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(-0.5f, -0.5f, 0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, -0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(0.5f, 0.5f, -0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(0.5f, 0.5f, 0.5f)),
        cubeTransform.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, 0.5f)),
        };

        Debug.DrawLine(verts[0], verts[1], color, duration);
        Debug.DrawLine(verts[1], verts[2], color, duration);
        Debug.DrawLine(verts[2], verts[3], color, duration);
        Debug.DrawLine(verts[3], verts[0], color, duration);

        Debug.DrawLine(verts[4], verts[5], color, duration);
        Debug.DrawLine(verts[5], verts[6], color, duration);
        Debug.DrawLine(verts[6], verts[7], color, duration);
        Debug.DrawLine(verts[7], verts[4], color, duration);

        Debug.DrawLine(verts[0], verts[4], color, duration);
        Debug.DrawLine(verts[1], verts[5], color, duration);
        Debug.DrawLine(verts[2], verts[6], color, duration);
        Debug.DrawLine(verts[3], verts[7], color, duration);
    }


}
