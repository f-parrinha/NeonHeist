using UnityEngine;

[CreateAssetMenu(fileName = "ZCorridorTileScript", menuName = "Scriptable Objects/WCF/ZCorridorTileScript")]
public class ZCorridorTileScript : TileScript
{
    
    protected override void OnEnable()
    {
        base.OnEnable();

        negativeX = "wall";
        negativeZ = "path";
        positiveX = "wall";
        positiveZ = "path";
        tileName = "z_straight_corridor";

        weight = 0.5f;
       
    }



}
