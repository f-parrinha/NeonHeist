using UnityEngine;

[CreateAssetMenu(fileName = "XCorridorTileScript", menuName = "Scriptable Objects/WCF/XCorridorTileScript")]
public class XCorridorTileScript : TileScript
{
    protected override void OnEnable()
    {
        base.OnEnable();

        negativeX = "path";
        negativeZ = "wall";
        positiveX = "path";
        positiveZ = "wall";
        tileName = "x_straight_corridor";

        weight = 0.5f;
    
    }



}
