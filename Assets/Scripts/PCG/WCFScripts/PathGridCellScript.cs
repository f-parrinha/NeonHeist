using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "GridCellScript", menuName = "Scriptable Objects/PathGridCellScript")]
public class PathGridCellScript : ScriptableObject
{
    public Vector3Int position;
    public List<TileScript> possibleTiles;
    public bool isCollapsed;
    public TileScript collapsedTile;
    public bool empty ;

    public PathGridCellScript(Vector3Int pos, IEnumerable<TileScript> inititialTiles)
    { 
        possibleTiles = new List <TileScript> (inititialTiles);
        isCollapsed = false;
        position = pos;
        empty = false;
    }

    public int getEntropy ()
    {
        return possibleTiles.Count;
    }

}
