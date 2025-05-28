using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //model and its name
    //private string tileName;

    [SerializeField] private List<Transform> entryPoint;
    [SerializeField] private List<Transform> exitPoint;
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private List<GameObject>exitTiles;
   
    public List<GameObject> getTiles()
    {
        return exitTiles;
    }

    public int getTilesCount()
    {
        return exitTiles.Count;
    }

    public GameObject getTile(int index)
    {
        return exitTiles[index];
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


}
