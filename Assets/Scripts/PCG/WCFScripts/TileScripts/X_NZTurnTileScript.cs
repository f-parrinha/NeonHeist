using System;
using UnityEngine;

[CreateAssetMenu(fileName = "X_NZTurnTileScript", menuName = "Scriptable Objects/WCF/X_NZTurnTileScript")]
public class X_NZTurnTileScript : TileScript
{
   
    protected override void OnEnable ()
    {
        base.OnEnable ();

        negativeX = "wall";
        negativeZ = "path";
        positiveX = "path";
        positiveZ = "wall";
        tileName = "X_NZturn_corridor";

        weight = 0.25f;
        
    }
    

}
