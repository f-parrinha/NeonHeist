using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;
public class Tile : MonoBehaviour
{
    //model and its name
    //private string tileName;

    [SerializeField] private List<Transform> entryPoints;
    [SerializeField] private List<Transform> exitPoints;
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

    public List<Transform> getEntryPoints()
    {
        return entryPoints;
    }
    public List<Transform> getExitPoints()
    {
        return exitPoints;
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
