using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathGen : MonoBehaviour
{

    EntryWallScript entryWall;
    private List<EntryWallScript> entrances = new List<EntryWallScript>();
    [SerializeField] public GameObject passage;

    [SerializeField]private List<TileScript> allTiles;
    private List<TileScript> entryPath = new List<TileScript>();
    private Vector3Int firstPathDir;

    //rest
    public GameObject gridCell;
    [SerializeField] private Transform wallTransform;

    public Vector3Int gridSize = new Vector3Int(5, 1, 5); //or 2int? x e z (y)??
    private GridCellScript[,,] grid;
    private Transform start;
    private Queue<Vector3Int> propagationQueue; // A queue for propagation

 

    private void Awake()
    {
       /* EntryWallScript[] childWallScripts = GetComponentsInChildren<EntryWallScript>();

        foreach (EntryWallScript wallScript in childWallScripts)
        {
            //Dont need this??
            if (wallScript.isEntry())
            {
                entrances.Add(wallScript);
                Debug.Log($"Found entry wall at position: {wallScript.getWallBlockPos()}");
            }
        }*/

    }

    void Start()
    {
        entryWall = GetRandomEntryWall();

        Vector3 wallPos = entryWall.getWallBlockPos();
        float wallDepth = entryWall.transform.localScale.x;

        GameObject startPos = Instantiate(passage, wallPos, entryWall.transform.rotation);
        Destroy(entryWall.gameObject );


        TileScript first = getFirstTile();
        Debug.Log("first tile " + first.name);

        //is this right???
        float x = (first.prefab.transform.localScale.x + wallDepth) /2;
        float z = (first.prefab.transform.localScale.z + wallDepth) / 2;
        
        Vector3 firstPathPos = new Vector3(wallPos.x+ (firstPathDir.x*x), 0, wallPos.z +(firstPathDir.z*z));
        Instantiate(first.prefab, firstPathPos, Quaternion.identity);

        //GenerateGrid(firstPathPos);
      //  InitializeGrid();
       // RunWCFAlgorithm();
        //InstantiateTiles();
      

    }

    private void InitializeGrid()
    {
        grid = new GridCellScript[gridSize.x, gridSize.y, gridSize.z];


        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z] = new GridCellScript(new Vector3Int(x, y, z), allTiles);

                }
            }
        }

    }

    private void RunWCFAlgorithm()
    {
        throw new System.NotImplementedException();
    }

    private void InstantiateTiles()
    {
        throw new System.NotImplementedException();
    }

    //WRONG!
    private void GenerateGrid(Vector3 firstPathPos)
    {
      
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 5; z++) 
            {
                
                Vector3 cellWorldPosition = firstPathPos + new Vector3(x, 0f, z);
                
                GameObject newCell = Instantiate(gridCell, cellWorldPosition,Quaternion.identity);

            }
        }
    }

    private void GeneratePath()
    {
        //startTile = topFloor[xIndex];

        for (int i= 0;  i< 10;i++)
        {
            foreach (TileScript tileScript in allTiles)
            {
                if (tileScript.getFace(-firstPathDir) == "path")
                {
                    Debug.Log("Added Tile - " + tileScript.name);
                    entryPath.Add(tileScript);
                }
            }

            float totalWeight = entryPath.Sum(t => t.weight);

            // Randomly select a tile based on weights
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            TileScript selectedTile = null;

            foreach (TileScript tile in entryPath)
            {
                Debug.Log("entered entryPaths, to select start tile");
                currentWeight += tile.weight;
                if (randomValue <= currentWeight)
                {
                    selectedTile = tile;
                    Debug.Log("inside loop -> selected tile " + selectedTile.name);
                    break;
                }
            }
        }
    }

    private TileScript getFirstTile()
    {
        Vector3 wallX = entryWall.transform.right;
        Debug.Log("wall local x! " + wallX);

        firstPathDir = new Vector3Int(
                Mathf.RoundToInt(wallX.x),
                Mathf.RoundToInt(wallX.y),
                Mathf.RoundToInt(wallX.z)
            );

       
        foreach (TileScript tileScript in allTiles)
        {
            if (tileScript.getFace(-firstPathDir) == "path")
            {
                Debug.Log("Added Tile - " + tileScript.name);
                entryPath.Add(tileScript);
            }
        }

        float totalWeight = entryPath.Sum(t => t.weight);

        // Randomly select a tile based on weights
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        TileScript selectedTile = null;

        foreach (TileScript tile in entryPath)
        {
            Debug.Log("entered entryPaths, to select start tile");
            currentWeight += tile.weight;
            if (randomValue <= currentWeight)
            {
                selectedTile = tile;
                Debug.Log("inside loop -> selected tile " + selectedTile.name);
                break;
            }
        }

        return selectedTile;
    }
    private EntryWallScript GetRandomEntryWall()
    {
        if (entrances.Count == 0)
        {
            Debug.LogWarning("No entry walls found!");
            return null;
        }
        return entrances[Random.Range(0, entrances.Count)];
       
    }

   

    


}
