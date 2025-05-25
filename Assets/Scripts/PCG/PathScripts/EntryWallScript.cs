using System.Collections.Generic;
using UnityEngine;

public class EntryWallScript : MonoBehaviour
{
    [SerializeField] private Transform wall;

    private bool entry = true;
   // public string entryWallPath = "path";
  
    public bool isEntry ()
    {
        return entry;
    }

    public Vector3 getWallBlockPos ()
    {
        return wall.position;
    }  

}
