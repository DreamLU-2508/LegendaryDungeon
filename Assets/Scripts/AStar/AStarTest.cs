using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

namespace DreamLU.AStar
{
    public class AStarTest : MonoBehaviour
    {
        private Room _room;
        private Grid grid;
        private Tilemap frontTilemap;
        private Tilemap pathTilemap;
        private Vector3Int startGridPosition;
        private Vector3Int endGridPosition;
        private TileBase startPathTile;
        private TileBase finishPathTile;

        private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
        private Stack<Vector3> pathStack;

        private IGameStateProvider _gameStateProvider;
        
        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        public void OnRoomChanged(Room room)
        {
            pathStack = null;
            _room = room;
            frontTilemap = room.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
            grid = room.transform.GetComponentInChildren<Grid>();
            startGridPosition = noValue;
            endGridPosition = noValue;

            SetUpPathTilemap();
        }

        private void Start()
        {
            startPathTile = GameResources.Instance.preferredEnemyPathTile;
            finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
        }

        /// <summary>
        /// Use a clone of the front tilemap for the path tilemap.  If not created then create one, else use the exisitng one.
        /// </summary>
        private void SetUpPathTilemap()
        {
            Transform tilemapCloneTransform = _room.transform.Find("Grid/Tilemap4_Front(Clone)");

            // If the front tilemap hasn't been cloned then clone it
            if (tilemapCloneTransform == null)
            {
                pathTilemap = Instantiate(frontTilemap, grid.transform);
                pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
                // pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
                pathTilemap.gameObject.tag = "Untagged";
            }
            // else use it
            else
            {
                pathTilemap = _room.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
                pathTilemap.ClearAllTiles();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (_room == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

            if (Input.GetKeyDown(KeyCode.I))
            {
                ClearPath();
                SetStartPosition();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                ClearPath();
                SetEndPosition();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                DisplayPath();
            }
        }


        /// <summary>
        /// Set the start position and the start tile on the front tilemap
        /// </summary>
        private void SetStartPosition()
        {
            if (startGridPosition == noValue)
            {
                startGridPosition = grid.WorldToCell(_gameStateProvider.GetMouseWorldPosition());

                if (!IsPositionWithinBounds(startGridPosition))
                {
                    startGridPosition = noValue;
                    return;
                }

                pathTilemap.SetTile(startGridPosition, startPathTile);
            }
            else
            {
                pathTilemap.SetTile(startGridPosition, null);
                startGridPosition = noValue;
            }
        }


        /// <summary>
        /// Set the end position and the end tile on the front tilemap
        /// </summary>
        private void SetEndPosition()
        {
            if (endGridPosition == noValue)
            {
                endGridPosition = grid.WorldToCell(_gameStateProvider.GetMouseWorldPosition());

                if (!IsPositionWithinBounds(endGridPosition))
                {
                    endGridPosition = noValue;
                    return;
                }

                pathTilemap.SetTile(endGridPosition, finishPathTile);
            }
            else
            {
                pathTilemap.SetTile(endGridPosition, null);
                endGridPosition = noValue;
            }
        }


        /// <summary>
        /// Check if the position is within the lower and upper bounds of the room
        /// </summary>
        private bool IsPositionWithinBounds(Vector3Int position)
        {
            // If  position is beyond grid then return false
            if (position.x < _room.InstancedRoom.TemplateLowerBounds.x || position.x > _room.InstancedRoom.TemplateUpperBounds.x
                                                              || position.y < _room.InstancedRoom.TemplateLowerBounds.y || position.y > _room.InstancedRoom.TemplateUpperBounds.y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Clear the path and reset the start and finish positions
        /// </summary>
        private void ClearPath()
        {
            // Clear Path
            if (pathStack == null) return;

            foreach (Vector3 worldPosition in pathStack)
            {
                pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
            }

            pathStack = null;

            //Clear Start and Finish Squares
            endGridPosition = noValue;
            startGridPosition = noValue;
        }

        /// <summary>
        /// Build and display the AStar path between the start and finish positions
        /// </summary>
        private void DisplayPath()
        {
            if (startGridPosition == noValue || endGridPosition == noValue) return;
            
            pathStack = AStarUtilities.BuildPath(_room, startGridPosition, endGridPosition);
            
            if (pathStack == null) return;
            
            foreach (Vector3 worldPosition in pathStack)
            {
                pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
            }
        }
    }

}