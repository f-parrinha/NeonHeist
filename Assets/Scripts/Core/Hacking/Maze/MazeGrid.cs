using System.Collections.Generic;
using UnityEngine;

namespace Core.Hacking.Maze
{
    public class MazeGrid
    {

        private bool[,] grid;
        public float MazeLength { get; private set; }
        public float TileLength { get; private set; }
        public int GridSize { get; private set; }

        public MazeGrid(int gridSize, float mazeWorldSize)
        {
            GridSize = gridSize % 2 == 0 ? gridSize + 1 : gridSize;
            TileLength = mazeWorldSize / gridSize;
        }

        /// <summary>
        /// Recursive Depth-First Search to carve the grid, creating a valid maze
        /// <para> By default, sets all positions to be a wall, and tries to find valid positions to carve the wall, creating a maze </para>
        /// </summary>
        /// <param name="start"> return maze's start position </param>
        /// <param name="goal"> return maze's last position </param>
        public void CreateMaze(out Vector2Int start, out Vector2Int goal)
        {
            grid = new bool[GridSize, GridSize];

            // Set all to walls
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    grid[x, y] = true;
                }
            }

            // State
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

            goal = Vector2Int.zero;
            start = new Vector2Int(Random.Range(0, grid.GetLength(0)), Random.Range(0, grid.GetLength(1)));
            stack.Push(start);
            grid[start.x, start.y] = false;


            // Start algorithm
            while (stack.Count > 0)
            {
                List<Vector2Int> unvisited = new List<Vector2Int>();
                Vector2Int current = stack.Pop();
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int next = current + direction * 2;

                    if (IsOutOfBounds(next) || !grid[next.x, next.y]) continue;

                    unvisited.Add(next);
                }

                // For the "if the current cell..." section...
                if (unvisited.Count > 0)
                {
                    stack.Push(current);
                    Vector2Int chosen = unvisited[Random.Range(0, unvisited.Count)];

                    // Remove the wall between current and chosen cell
                    Vector2Int wall = current + (chosen - current) / 2;
                    grid[wall.x, wall.y] = false;

                    // Marks current as visited
                    grid[chosen.x, chosen.y] = false;
                    stack.Push(chosen);

                    // Updates goal position
                    goal = current;
                }
            }
        }

        public Vector2Int WorldToGrid(Vector2 worldPos)
        {
            int x = (int)Mathf.Round(worldPos.x / TileLength);
            int y = -(int)Mathf.Round(worldPos.y / TileLength);
            return new Vector2Int(x, y);
        }

        public Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new(TileLength * gridPos.x, -TileLength * gridPos.y);
        }

        public bool IsOutOfBounds(Vector2Int gridPos)
        {
            return gridPos.x >= GridSize || gridPos.x < 0 ||
                gridPos.y < 0 || gridPos.y >= GridSize;
        }

        /// <summary>
        /// Is the position out of bounds or in a wall?
        /// </summary>
        /// <param name="gridPos"> grid position to check </param>
        /// <returns> whether the position is invalid or not </returns>
        public bool IsPositionInvalid(Vector2Int gridPos)
        {
            return IsOutOfBounds(gridPos) || grid[gridPos.x, gridPos.y];
        }

        public bool IsWall(Vector2Int gridPos)
        {
            return !IsOutOfBounds(gridPos) && grid[gridPos.x, gridPos.y];
        }
    }
}