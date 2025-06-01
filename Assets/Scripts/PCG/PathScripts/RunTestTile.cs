using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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

    private Dictionary<TileType, List<GameObject>> tilePrefabsByType;
    
    void Start()
    {
        tilePrefabsByType = _tileTypePrefabs.ToDictionary(t => t.type, t => t.prefabs);

        Generate();

    }

    private void Generate()
    {

        List<GameObject> possibleIntroPrefabs = tilePrefabsByType[TileType.Straight];
        GameObject startTileObj = possibleIntroPrefabs[Random.Range(0, possibleIntroPrefabs.Count)];

        Tile startTile = startTileObj.GetComponent<Tile>();

        List<Transform> entryPointList = startTile.getEntryPoints();
      
        Transform entryPoint = entryPointList[0]; //smmpre começa com o straight
        
        Vector3 tileOffset = startTile.transform.position - entryPoint.position;
        Vector3 pos = _startPoint.position + (_startPoint.rotation * tileOffset);

        GameObject tile = Instantiate(startTileObj, pos, _startPoint.rotation); 
        
        GenerateTiles(tile);
      
    }

    private void GenerateTiles(GameObject firstTile)
    {
        
        Tile currentTile = firstTile.GetComponent<Tile>();

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

            //generate location and rotation
            Quaternion targetRotation = exitPoint.rotation; 
            Vector3 tileOffset = -Vector3.Scale(entryPoint.localPosition, nextTileObj.transform.localScale);
            Vector3 targetPosition = exitPoint.position +  (targetRotation * tileOffset);

            //check overlap
            Vector3 boxCenter = targetPosition + targetRotation * Vector3.Scale(nextTileScript.getBoxCollider().center, nextTileObj.transform.localScale);
            Vector3 boxHalfExtents = Vector3.Scale(nextTileScript.getBoxCollider().size, nextTileObj.transform.localScale) * 0.5f * 0.99f;

            if (checkOverlap(boxCenter, boxHalfExtents, targetRotation))
                continue;


            GameObject nextTile = Instantiate(nextTileObj, targetPosition, targetRotation);

            InstantiateEntryFiller(entryIndexToFix, entryPointList, nextTile);
            /*for (int i = 0; i < entryIndexToFix.Count ; i++)
            {
                int index = entryIndexToFix[i];
                List<GameObject> possibleWallsPrefabs = tilePrefabsByType[TileType.Wall];
                GameObject wallTileObj = possibleWallsPrefabs[Random.Range(0, possibleWallsPrefabs.Count)];

                GameObject wall = Instantiate(wallTileObj, nextTile.transform);
                wall.transform.localPosition = entryPointList[index].localPosition + new Vector3(0,0.45f,1.8f); //??
                wall.transform.localRotation = entryPointList[index].localRotation;

              
                
            }
            */

            nextTile.layer = LayerMask.NameToLayer("PCG");

            currentTile = nextTile.GetComponent<Tile>();
        }

        InstantiateLastWall(currentTile);

        /*for (int i = 0; i < currentTile.getExitPoints().Count; i++)
        {
            GameObject lastTile = currentTile.gameObject;
            List<GameObject> possibleEndPrefabs = tilePrefabsByType[TileType.End];
            GameObject wallTileObj = possibleEndPrefabs[Random.Range(0, possibleEndPrefabs.Count)];

            GameObject lastWall = Instantiate(wallTileObj, lastTile.transform);
            lastWall.transform.localPosition = currentTile.getExitPoints()[i].transform.localPosition + (currentTile.getExitPoints()[i].transform.localRotation * new Vector3(0, 0.45f,-1.8f) ) ; //?? * rotation?
            lastWall.transform.localRotation = currentTile.getExitPoints()[i].transform.localRotation; 
            Debug.Log("instantiated last wall");

        }*/

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

   
}
