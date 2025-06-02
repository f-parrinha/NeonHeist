using Core.Common.Interfaces;
using Core.Controllers;
using Core.Hacking.Events;
using Core.Hacking.Interfaces;
using Core.Hacking.Maze;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Terminal
{
    /// <summary>
    /// Class <c> TerminalUI </c> represents the hacking terminal. It is in itslef a hacking mechanism/system
    /// </summary>
    public class TerminalUI : MonoBehaviour, IHackSystem, IInitializable, IRefreshable
    {
        private const int STADARD_MAZE_SIZE = 11;

        [Header("General Settings")]        
        [SerializeField] private RectTransform mazeArea;
        [SerializeField] private TextMeshProUGUI moveCounter;
        [SerializeField] private PauseController pauseController;
        [Header("Maze Prefabs")]
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject goalPrefab;
        [SerializeField] private GameObject pathPrefab;

        private MazeGrid mazeGrid;
        private RectTransform goal;
        private RectTransform player;
        private Vector2Int goalPos;

        private event EventHandler<UponHackArgs> uponHack;

        public bool IsOpened => gameObject.activeSelf;
        public bool IsInitialized { get; private set; }
        public int MovesLeft { get; private set; }
        public bool CheatMode { get; set; }
        public int HackDifficulty { get; set; } = STADARD_MAZE_SIZE;


        /* --------- UNITY METHODS --------- */

        private void Start()
        {
            Close();
        }

        private void Update()
        {
            AddPlayerControl();
        }


        /* --------- PUBLIC METHODS --------- */

        public void Initialize()
        {
            if (IsInitialized) return;

            // Fix maze area - if needed
            var mazeWorldSize = mazeArea.sizeDelta.x;
            mazeArea.sizeDelta = new Vector2(mazeWorldSize, mazeWorldSize);

            // Setup state
            mazeGrid = new MazeGrid(HackDifficulty, mazeWorldSize);

            IsInitialized = true;
        }

        public void Close()
        {
            if (!IsOpened) return;

            gameObject.SetActive(false);
            pauseController.UnsetPauser(this);
        }

        public void Open()
        {
            if (IsOpened) return;

            Refresh();
            pauseController.Pause(this, true);
            gameObject.SetActive(true);
        }

        public bool Toggle()
        {
            gameObject.SetActive(!IsOpened);
            return IsOpened;
        }

        public void Refresh()
        {
            Initialize();

            mazeGrid.CreateMaze(out Vector2Int startPos, out goalPos);
            List<Vector2Int> bestPath = MazeAStar.Run(mazeGrid, startPos, goalPos);

            ClearDraw();
            DrawPath(bestPath);
            DrawWalls();
            DrawGoal(goalPos);
            DrawPlayer(startPos);
            DrawMoveCounter(MovesLeft = bestPath.Count - 1);
        }

        /* --------- PLAYER CONTROL METHODS --------- */

        private void AddPlayerControl()
        {
            if (!IsOpened || player == null) return;

            Vector2Int moveVec = GetMoveVector();

            // Return immediately if not moving...
            if (moveVec == Vector2Int.zero) return;

            // Check if position is valid before moving
            Vector2Int currentGridPos = mazeGrid.WorldToGrid(player.anchoredPosition);
            Vector2Int nextGridPos = currentGridPos + moveVec;
            if (mazeGrid.IsPositionInvalid(nextGridPos)) return;

            // Define lose condition
            if (MovesLeft-- == 0)
            {
                RaiseUponHack(false);
                Close();
                return;
            }

            // Define win condition
            if (nextGridPos == goalPos)
            {
                RaiseUponHack(true);
                Close();
                return;
            }


            // Finally move
            DrawPlayer(nextGridPos);
            DrawMoveCounter(MovesLeft);
        }

        /// <summary>
        /// Gets the player's movement vector within the Terminal. Does not accept diagonal movement. Returns in Grid coordinates
        /// </summary>
        /// <returns> Player's movement vector in Grid coordinates </returns>
        private Vector2Int GetMoveVector()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                return Vector2Int.down;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                return Vector2Int.right;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                return Vector2Int.up;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return Vector2Int.left;
            }

            return Vector2Int.zero;
        }



        /* --------- DRAW METHODS --------- */

        private void ClearDraw()
        {
            // Clear grid
            for (int i = 0; i < mazeArea.childCount; i++)
            {
                Destroy(mazeArea.GetChild(i).gameObject);
            }

            player = null;
            goal = null;
        }

        /// <summary>
        /// Refreshes current grid and its walls
        /// </summary>
        private void DrawWalls()
        {
            // Paint walls
            for (int y = 0; y < mazeGrid.GridSize; y++)
            {
                for (int x = 0; x < mazeGrid.GridSize; x++)
                {
                    if (mazeGrid.IsWall(new Vector2Int(x, y)))
                    {
                        RectTransform wall = Instantiate(wallPrefab, mazeArea).GetComponent<RectTransform>();
                        wall.anchoredPosition = mazeGrid.GridToWorld(new Vector2Int(x, y));
                        wall.sizeDelta = new Vector2(mazeGrid.TileLength, mazeGrid.TileLength);
                    }
                }
            }
        }

        private void DrawPlayer(Vector2Int playerPos)
        {
            if (player == null)
            {
                player = Instantiate(playerPrefab, mazeArea).GetComponent<RectTransform>();
            }

            player.sizeDelta = Vector2.one * mazeGrid.TileLength;
            player.anchoredPosition = mazeGrid.GridToWorld(playerPos);
        }

        private void DrawGoal(Vector2Int goalPos)
        {
            if (goal == null)
            {
                goal = Instantiate(goalPrefab, mazeArea).GetComponent<RectTransform>();
            }

            goal.sizeDelta = Vector2.one * mazeGrid.TileLength;
            goal.anchoredPosition = mazeGrid.GridToWorld(goalPos);
        }

        private void DrawPath(List<Vector2Int> path)
        {
            if (!CheatMode) return;

            foreach (Vector2Int el in path)
            {
                RectTransform pathTile = Instantiate(pathPrefab, mazeArea).GetComponent<RectTransform>();
                pathTile.sizeDelta = Vector2.one * mazeGrid.TileLength;
                pathTile.anchoredPosition = mazeGrid.GridToWorld(el);
            }
        }

        private void DrawMoveCounter(int counter)
        {
            moveCounter.text = counter.ToString();
        }


        /* --------- EVENT METHODS --------- */
        public void AddUponHackHandler(EventHandler<UponHackArgs> handler) => uponHack += handler;
        public void RemoveUponHackHandler(EventHandler<UponHackArgs> handler) => uponHack -= handler;
        protected void RaiseUponHack(bool success) => uponHack?.Invoke(this, new UponHackArgs { Successful = success });
    }
}
