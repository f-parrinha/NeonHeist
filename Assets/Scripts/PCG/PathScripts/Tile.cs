using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //model and its name
    //private string tileName;

    [SerializeField] private Transform entryPoint;
    [SerializeField] private Transform exitPoint;
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

    public Transform getEntryPoint()
    {
        return entryPoint;
    }
    public Transform getExitPoint()
    {
        return exitPoint;
    }

    public BoxCollider getBoxCollider()
    {
        return boxCollider;
    }


}
