using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;
public class Tile : MonoBehaviour
{
    //model and its name
    //private string tileName;

    [SerializeField] private List<Transform> entryPoint;
    [SerializeField] private List<Transform> exitPoint;
    [SerializeField] private BoxCollider boxCollider;

    //ju
    //[SerializeField] private List<GameObject>exitTiles;
    [SerializeField] private List<TileType> exitTileTypes;
   
    public List<TileType> getTiles()
    {

        return exitTileTypes;
    }

    public int getTilesCount()
    {
        return exitTileTypes.Count;
    }

    public TileType getTile(int index)
    {

        return exitTileTypes[index];
    }

    public List<Transform> getEntryPoints()
    {
        return entryPoint;
    }
    public List<Transform> getExitPoints()
    {
        return exitPoint;
    }

    public BoxCollider getBoxCollider()
    {
        return boxCollider;
    }

    public TileType randomTileType()
    {
        return exitTileTypes[Random.Range(0, exitTileTypes.Count)];
    }


}
