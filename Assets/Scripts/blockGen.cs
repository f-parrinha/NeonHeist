using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public GameObject blockGameObj;
    public GameObject objToSpawn;

    private List<Vector3> blockPos = new List<Vector3>();


    private int worldSizeX = 10;
    private int worldSizeZ = 10;

    private float gridOffset = 1.5f;

    private void Start()
    {
        for(int x =0; x < worldSizeX; x++)
        {
            for(int z =0; z < worldSizeZ; z++)
            {
                Vector3 pos = new Vector3(x* gridOffset, 0, z* gridOffset);

                GameObject block = Instantiate(blockGameObj, pos, Quaternion.identity);

                block.transform.SetParent(this.transform);
            
                blockPos.Add(block.transform.position); //add the positions of blocks to the list
            }
        }

        SpawnObj();
    }

    private Vector3 ObjectSpawnLocation () //to find the location to spawn
    {
        int rndomIndex = Random.Range(0, blockPos.Count);

        Vector3 newPos = new Vector3(
            blockPos[rndomIndex].x,
            blockPos[rndomIndex].y + 0.5f ,//so the object isnt "inside" de location
            blockPos[rndomIndex].z);

        blockPos.RemoveAt(rndomIndex); //removing to avoid other object to spwn in the same pos
        return newPos;
    }

    private void SpawnObj()
    {
        for(int c =0; c<20; c++)
        {
            GameObject toPlaceObj = Instantiate(objToSpawn, 
                ObjectSpawnLocation(), 
                Quaternion.identity);
        }
    }
}