using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Hacking.Maze
{
    /// <summary>
    /// Class <c> AStar </c> represents the A* algorithim for a grid, using manhattan distance for heuristics
    /// </summary>
    public class MazeAStar
    {
        public static List<Vector2Int> Run(MazeGrid maze, Vector2Int start, Vector2Int goal)
        {
            // Setup state
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            var openSet = new PriorityQueue();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var gScore = new Dictionary<Vector2Int, float>();
            var fScore = new Dictionary<Vector2Int, float>();

            gScore[start] = 0;
            fScore[start] = GetHeuristic(start, goal);
            openSet.Enqueue(start, fScore[start]);

            // Do the algorithm
            while (openSet.Count > 0)
            {
                Vector2Int current = openSet.Dequeue();
                if (current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }

                // Evaluate neighbors
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;

                    // Consider walls
                    if (maze.IsPositionInvalid(neighbor)) continue;

                    float tentativeGScore = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + GetHeuristic(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                        else
                        {
                            openSet.UpdatePriority(neighbor, fScore[neighbor]);
                        }
                    }
                }
            }

            // Failed... :((
            return new List<Vector2Int>();
        }


        /// <summary>
        /// Generates Manhatthan heuristic
        /// </summary>
        /// <param name="from"> start position </param>
        /// <param name="to"> goal positon </param>
        /// <returns> estimated cost </returns>
        public static float GetHeuristic(Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }

        private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var totalPath = new List<Vector2Int> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current); // Prepend to the beginning
            }

            return totalPath;
        }


        /// <summary>
        /// Inner class <c> PriorityQueue </c> represents the queue used in the "openSet" for the graph investigation
        /// </summary>
        public class PriorityQueue
        {
            private List<(Vector2Int item, float priority)> heap = new();

            public int Count => heap.Count;

            public void Enqueue(Vector2Int item, float priority)
            {
                heap.Add((item, priority));
                HeapifyUp(heap.Count - 1);
            }

            public Vector2Int Dequeue()
            {
                if (heap.Count == 0)
                    throw new InvalidOperationException("PriorityQueue is empty");

                var item = heap[0].item;
                heap[0] = heap[^1];
                heap.RemoveAt(heap.Count - 1);
                HeapifyDown(0);
                return item;
            }

            public bool Contains(Vector2Int item)
            {
                return heap.Any(e => EqualityComparer<Vector2Int>.Default.Equals(e.item, item));
            }

            public void UpdatePriority(Vector2Int item, float newPriority)
            {
                for (int i = 0; i < heap.Count; i++)
                {
                    if (EqualityComparer<Vector2Int>.Default.Equals(heap[i].item, item))
                    {
                        heap[i] = (item, newPriority);
                        HeapifyUp(i);
                        HeapifyDown(i);
                        return;
                    }
                }
            }

            private void HeapifyUp(int index)
            {
                while (index > 0)
                {
                    int parent = (index - 1) / 2;
                    if (heap[index].priority >= heap[parent].priority)
                        break;
                    (heap[index], heap[parent]) = (heap[parent], heap[index]);
                    index = parent;
                }
            }

            private void HeapifyDown(int index)
            {
                int lastIndex = heap.Count - 1;
                while (true)
                {
                    int left = index * 2 + 1, right = index * 2 + 2, smallest = index;
                    if (left <= lastIndex && heap[left].priority < heap[smallest].priority)
                        smallest = left;
                    if (right <= lastIndex && heap[right].priority < heap[smallest].priority)
                        smallest = right;
                    if (smallest == index) break;
                    (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                    index = smallest;
                }
            }
        }
    }
}