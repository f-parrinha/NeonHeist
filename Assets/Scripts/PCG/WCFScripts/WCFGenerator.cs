using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class WCFGenerator : MonoBehaviour
{

    public Vector3Int gridSize = new Vector3Int(5,1,5); //or 2int? x e z (y)??
    public List<TileScript> allTiles;
    public Transform start;
    private GridCellScript[,,] grid;

    private Queue<Vector3Int> propagationQueue; // A queue for propagation

    void Start()
    {
        Generate();
        
    }

    private void Generate()
    {
        InitializeGrid();
        RunWCFAlgorithm();
        InstantiateTiles();
    }
    private void InitializeGrid()
    {
        grid = new GridCellScript[gridSize.x, gridSize.y, gridSize.z];
     

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z] = new GridCellScript(new Vector3Int(x, y, z), allTiles);

                }

            }
        }
    }


    private void RunWCFAlgorithm()
    {
        propagationQueue = new Queue<Vector3Int>();

        int iterations = 0;

        int maxIterations = gridSize.x * gridSize.y * gridSize.z *10; //to failsafe if stuck in loop

        while (iterations < maxIterations)
        {
            Vector3Int? cellToCollapsePos = GetLowestEntropyCell();

            if (!cellToCollapsePos.HasValue)
            {
                // All cells collapsed or no uncollapsed cells left
                Debug.Log("WFC: All cells collapsed or no more uncollapsed cells.");
                break;
            }

            GridCellScript currentCell = grid[cellToCollapsePos.Value.x, cellToCollapsePos.Value.y, cellToCollapsePos.Value.z];

            if (currentCell.getEntropy() == 0)
            {
                Debug.LogError($"contradiction at runWFCALg");
                //Debug.LogError($"WFC: Contradiction at {currentCell.position}. No possibilities left.");
                // You might need to implement backtracking here for robust WFC
                break;
            }

            if (currentCell.isCollapsed) return;

            float totalWeight = currentCell.possibleTiles.Sum(t => t.weight);

            // Randomly select a tile based on weights
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            TileScript selectedTile = null;

            foreach (TileScript tile in currentCell.possibleTiles)
            {
                Debug.Log("entered WCF, to select tile");
                currentWeight += tile.weight;
                if (randomValue <= currentWeight)
                {
                    selectedTile = tile;
                    Debug.Log("inside loop -> selected tile " + selectedTile.name);
                    break;
                }

                
            }
            Debug.Log("after loop -> selected tile " + selectedTile.name);
            currentCell.collapsedTile = selectedTile;
            currentCell.possibleTiles.Clear();
            currentCell.possibleTiles.Add(selectedTile); //ou tirar fica sem nda empty como já foi collapsed
 
            currentCell.isCollapsed = true;
            

            Debug.Log("Cell : "+currentCell.position+" collapsed: " +currentCell.collapsedTile.name);
            propagationQueue.Enqueue(currentCell.position);

            while (propagationQueue.Count > 0) 
            {
                Vector3Int currentPropagatePos = propagationQueue.Dequeue();
                Debug.Log("dequeuing propag at wcf at pos " + currentPropagatePos);
                PropagateChanges(currentPropagatePos);
                
            }
        }

    }


    private Vector3Int? GetLowestEntropyCell()
    {
        Vector3Int? lowestEntropyPos = null;
        int minEntropy = int.MaxValue;
        List<Vector3Int> candidates = new List<Vector3Int>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for(int y=0; y < gridSize.y;y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    GridCellScript cell = grid[x, y, z];
                    if (!cell.isCollapsed)
                    {
                        Debug.Log("entered lowest entropy for loop && not collapsed");

                        if (cell.getEntropy() < minEntropy && cell.getEntropy()>0)
                        {
                
                            minEntropy = cell.getEntropy();
                            Debug.Log("min entropy: " + minEntropy);
                            candidates.Clear();
                            candidates.Add(cell.position);
                           

                        }
                        else if (cell.getEntropy() == minEntropy)
                        {
                            candidates.Add(cell.position);
                        }
                    }
                }
            }
            
        }

        if (candidates.Count > 0)
        {
            // Pick a random one if multiple cells have the same lowest entropy
            foreach (Vector3Int candidate in candidates)
            {
                Debug.Log("candidates: " + candidate);
            }

            lowestEntropyPos = candidates[Random.Range(0, candidates.Count)];
        }
        Debug.Log("chosen (random) candidate: " + lowestEntropyPos);
        return lowestEntropyPos;
    }

    private bool IsValidPosition(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y && pos.z >= 0 && pos.z < gridSize.z;
    }
    //this is not right
    private void PropagateChanges(Vector3Int position)
    {

        Debug.Log("START PROPAGATE pos: " + position);
        foreach(Vector3Int t in propagationQueue)
        {
            Debug.Log("element in prop queue: " + t);
        }
      
        GridCellScript collapsedCell = grid[position.x, position.y, position.z];
        TileScript collapsedTile = collapsedCell.collapsedTile;

        Vector3Int[] directions = new Vector3Int[] // Corresponding Unity directions
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.right,
            Vector3Int.left
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = position + dir;

            Debug.Log(" start loop - neighbor pos: " + neighborPos);

            if (IsValidPosition(neighborPos))
            {
                GridCellScript neighborCell = grid[neighborPos.x, neighborPos.y, neighborPos.z];

                Debug.Log("valid pos: " + neighborCell.position);

                if (!neighborCell.isCollapsed) 
                {

                    int initialPossibleCount = neighborCell.possibleTiles.Count;
                    Debug.Log("not collapsed cell, initial posible count " + initialPossibleCount);
                    List<TileScript> newPossibleTiles = new List<TileScript>();

                    //string neighborRequiredFace = collapsedCell.collapsedTile.getOppositeFace(dir); //problematic??
                    if(collapsedCell.collapsedTile!=null)
                    {
                        string requiredFace = collapsedCell.collapsedTile.getFace(dir);
                        Debug.Log("collapsed tile the needed face - >" + requiredFace);

                        //problem qnd tenta propagar pra os vizinhos dos vizinhos q ainda não foram collapsed, da null
                        foreach (TileScript tile in neighborCell.possibleTiles)
                        {
                            string neighborFace = tile.getFace(-dir);

                            if (requiredFace == neighborFace) // Note the -directionToNeighbor here!
                            {
                                newPossibleTiles.Add(tile);
                                Debug.Log("Added tile " + tile.name);
                            }

                        }

                    }
                    else
                    { //WRONG

                        Debug.Log("not collapsed cell else");
                        //problem qnd tenta propagar pra os vizinhos dos vizinhos q ainda não foram collapsed, da null

                        foreach (TileScript tile in collapsedCell.possibleTiles)
                        {
                            string requiredFace = tile.getFace(dir);

                            foreach (TileScript neighborTiles in  neighborCell.possibleTiles)
                            {
                                Debug.Log("neighbor Possible tiles " + neighborTiles.name);
                                string neighborFace = neighborTiles.getFace(-dir);
                                if (requiredFace == neighborFace && !newPossibleTiles.Contains(neighborTiles)) // Change to hashset!!!
                                {
                                    newPossibleTiles.Add(neighborTiles);
                                    Debug.Log("not collapsed Added tile " + neighborTiles.name);
                                }
                            }
                             

                        }
                    }

                   
                    if (newPossibleTiles.Count < initialPossibleCount)
                    {
                        neighborCell.possibleTiles = newPossibleTiles;

                        if (neighborCell.getEntropy() == 0)
                        {
                            Debug.LogError("Contradition, no available tiles during propagation at {neighborPos} "); //backtract????
                           // return; //REVIEWW
                        }

                        if(!neighborCell.isCollapsed)
                        {
                            propagationQueue.Enqueue(neighborPos);
                            Debug.Log("enqueueing " + neighborPos);
                        }
                    }

                 
                }
            }
        }

    }


    private void InstantiateTiles()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.z; j++)
            {
                TileScript tile = grid[i, 0,j].collapsedTile;

                if (tile != null)
                {
                   
                    Debug.Log(tile.name + " -> pos: " + grid[i, 0, j]);
                    Instantiate(tile.prefab, new Vector3Int(i, 0, j), Quaternion.identity);
                }
                else
                {

                    Debug.Log("NoTile");

                }
             
            }
        }


      
    }

    
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
