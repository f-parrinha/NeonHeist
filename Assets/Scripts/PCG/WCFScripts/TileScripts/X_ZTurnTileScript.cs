using System;
using UnityEngine;

[CreateAssetMenu(fileName = "X_ZTurnTileScript", menuName = "Scriptable Objects/WCF/X_ZTurnTileScript")]
public class X_ZTurnTileScript : TileScript
{
   
    protected override void OnEnable ()
    {
        base.OnEnable ();

        negativeX = "wall";
        negativeZ = "wall";
        positiveX = "path";
        positiveZ = "path";
        tileName = "X_Zturn_corridor";

        weight = 0.25f;
        
    }
    

}
