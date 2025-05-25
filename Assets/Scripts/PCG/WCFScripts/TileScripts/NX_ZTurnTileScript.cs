using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NX_ZTurnTileScript", menuName = "Scriptable Objects/WCF/NX_ZTurnTileScript")]
public class NX_ZTurnTileScript : TileScript
{
   
    protected override void OnEnable ()
    {
        base.OnEnable ();

        negativeX = "path";
        negativeZ = "wall";
        positiveX = "wall";
        positiveZ = "path";
        tileName = "NX_Zturn_corridor";

        weight = 0.25f;
        
    }
    

}
