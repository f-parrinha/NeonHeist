using System;
using UnityEngine;

public abstract class TileScript : ScriptableObject
{
    //model and its name
    public GameObject prefab; //???
    public string tileName;

    //model faces
    public string negativeX;
    public string negativeZ;
    public string positiveX;
    public string positiveZ;

    public float weight; //for random selection

    public string getFace(Vector3Int direction)
    {
        if (direction == Vector3Int.forward) return positiveZ;
        if (direction == Vector3Int.back) return negativeZ;
        if (direction == Vector3Int.right) return positiveX;
        if (direction == Vector3Int.left) return negativeX;

        return "";
    }

    public string getOppositeFace(Vector3Int direction)
    {
        if (direction == Vector3Int.forward) return negativeZ;
        if (direction == Vector3Int.back) return positiveZ;
        if (direction == Vector3Int.right) return negativeX;
        if (direction == Vector3Int.left) return positiveX;

        return "";
    }

    protected virtual void OnEnable() { }


}
