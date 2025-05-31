using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NX_NZTurnTileScript", menuName = "Scriptable Objects/WCF/NX_NZTurnTileScript")]
public class NX_NZTurnTileScript : TileScript
{
    protected override void OnEnable ()
    {
        base.OnEnable ();

        negativeX = "path";
        negativeZ = "path";
        positiveX = "wall";
        positiveZ = "wall";
        tileName = "NX_NZTurn_corridor";

        weight = 0.25f;
   
    }
    

}
