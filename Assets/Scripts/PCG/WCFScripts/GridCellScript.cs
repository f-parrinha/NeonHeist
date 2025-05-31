using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "GridCellScript", menuName = "Scriptable Objects/WCF/GridCellScript")]
public class GridCellScript : ScriptableObject
{
    public Vector3Int position;
    public List<TileScript> possibleTiles;
    public bool isCollapsed;
    public TileScript collapsedTile;

    public GridCellScript(Vector3Int pos, IEnumerable<TileScript> inititialTiles)
    { 
        possibleTiles = new List <TileScript> (inititialTiles);
        isCollapsed = false;
        position = pos;
    }

    public int getEntropy ()
    {
        return possibleTiles.Count;
    }

}
