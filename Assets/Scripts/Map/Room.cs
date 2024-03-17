using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

namespace DreamLU
{
    public class Room : MonoBehaviour
    {
        private InstancedRoom _room;
        private Grid grid;
        private Tilemap groundTilemap;
        private Tilemap decoration1Tilemap;
        private Tilemap decoration2Tilemap;
        private Tilemap frontTilemap;
        private Tilemap collisionTilemap;
        private Tilemap minimapTilemap;
        private GameObject _vfxTelepos;
        private bool isCreateTelepos;
        
        // [ShowInInspector, ReadOnly]
        private int[,] aStarMovementPenalty = new int[0,0];  // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
        // [ShowInInspector, ReadOnly]
        private int[,] aStarItemObstacles = new int[0,0]; // use to store position of moveable items that are obstacles

        private IEnemySpawnProvider _enemySpawnProvider;
        private IGameStateProvider _gameStateProvider;
        private IDropItemHandle _dropItemHandle;

        public System.Action<Room> OnEnterRoom;
        public System.Action<Room> OnExitRoom;

        public InstancedRoom InstancedRoom => _room;
        public bool IsClear => _room.IsClearEnemy;
        public Grid Grid => grid;

        public int[,] AStarMovementPenalty => aStarMovementPenalty;
        public int[,] AStarItemObstacles => aStarItemObstacles;

        private void Awake()
        {
            _enemySpawnProvider = CoreLifetimeScope.SharedContainer.Resolve<IEnemySpawnProvider>();
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            _dropItemHandle = CoreLifetimeScope.SharedContainer.Resolve<IDropItemHandle>();
            
            OnEnterRoom += _gameStateProvider.OnEnterRoom;
            _enemySpawnProvider.OnKillEnemy += OnKillEnemyInRoom;
        }

        public void SetData(InstancedRoom room, GameObject vfxTelepos)
        {
            _room = room;
            _vfxTelepos = vfxTelepos;
            isCreateTelepos = false;
        }

        public void Initialise(GameObject gameObject)
        {
            PopulateTilemapMemberVariables(gameObject);
            BlockOffUnusedDoorWays();
            AddDoors();
            AddObstaclesAndPreferredPaths();
            CreateItemObstaclesArray();
            UpdateMoveableObstacles();
            
            if (_room.RoomType == RoomType.Entrance)
            {
                OnEnterRoom.Invoke(this);
            }

            if (_room.RoomType == RoomType.ChessRoom)
            {
                var chestPrefab = _dropItemHandle.ChestPrefab;
                if (chestPrefab != null)
                {
                    var chest = Instantiate(chestPrefab, this.transform);
                    chest.transform.position = grid.CellToWorld((Vector3Int)_room.RoomData.positionChest);
                }
            }
        }
        
        /// <summary>
        /// Update the array of moveable obstacles
        /// </summary>
        public void UpdateMoveableObstacles()
        {
            InitializeItemObstaclesArray();

            // foreach (MoveItem moveItem in moveableItemsList)
            // {
            //     Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            //     Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);
            //
            //     // Loop through and add moveable item collider bounds to obstacle array
            //     for (int i = colliderBoundsMin.x; i <= colliderBoundsMax.x; i++)
            //     {
            //         for (int j = colliderBoundsMin.y; j <= colliderBoundsMax.y; j++)
            //         {
            //             aStarItemObstacles[i - _room.TemplateLowerBounds.x, j - _room.TemplateLowerBounds.y] = 0;
            //         }
            //     }
            // }
        }
        
        /// <summary>
        /// Initialize Item Obstacles Array With Default AStar Movement Penalty Values
        /// </summary>
        private void InitializeItemObstaclesArray()
        {
            for (int x = 0; x < (_room.TemplateUpperBounds.x - _room.TemplateLowerBounds.x + 1); x++)
            {
                for (int y = 0; y < (_room.TemplateUpperBounds.y - _room.TemplateLowerBounds.y + 1); y++)
                {
                    // Set default movement penalty for grid sqaures
                    aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
                }
            }
        }

        private void BlockOffUnusedDoorWays()
        {
            // Loop through all doorways
            foreach (Doorway doorway in _room.Doors)
            {
                if (doorway.isConnected)
                    continue;

                // Block unconnected doorways using tiles on tilemaps
                if (collisionTilemap != null)
                {
                    BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
                }

                if (minimapTilemap != null)
                {
                    BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
                }

                if (groundTilemap != null)
                {
                    BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
                }

                if (decoration1Tilemap != null)
                {
                    BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
                }

                if (decoration2Tilemap != null)
                {
                    BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
                }

                if (frontTilemap != null)
                {
                    BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
                }
            }
        }

        private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
        {
            switch (doorway.orientation)
            {
                case DoorOrientation.Up:
                case DoorOrientation.Down:
                    BlockDoorwayHorizontally(tilemap, doorway);
                    break;

                case DoorOrientation.Right:
                case DoorOrientation.Left:
                    BlockDoorwayVertically(tilemap, doorway);
                    break;

                case DoorOrientation.None:
                    break;
            }

        }

        private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
        {
            Vector2Int startPosition = doorway.doorwayStartCopy.position;

            // loop through all tiles to copy
            for (int xPos = 0; xPos < doorway.doorwayStartCopy.width; xPos++)
            {
                for (int yPos = 0; yPos < doorway.doorwayStartCopy.height; yPos++)
                {
                    // Get rotation of tile being copied
                    Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                    // Copy tile
                    tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                    // Set rotation of tile copied
                    tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
                }
            }
        }

        private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
        {
            Vector2Int startPosition = doorway.doorwayStartCopy.position;

            // loop through all tiles to copy
            for (int yPos = 0; yPos < doorway.doorwayStartCopy.height; yPos++)
            {

                for (int xPos = 0; xPos < doorway.doorwayStartCopy.width; xPos++)
                {
                    // Get rotation of tile being copied
                    Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                    // Copy tile
                    tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                    // Set rotation of tile copied
                    tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
                }

            }
        }

        private void PopulateTilemapMemberVariables(GameObject roomGameobject)
        {
            // Get the grid component.
            grid = roomGameobject.GetComponentInChildren<Grid>();

            // Get tilemaps in children.
            Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap.gameObject.tag == "groundTilemap")
                {
                    groundTilemap = tilemap;
                }
                else if (tilemap.gameObject.tag == "decoration1Tilemap")
                {
                    decoration1Tilemap = tilemap;
                }
                else if (tilemap.gameObject.tag == "decoration2Tilemap")
                {
                    decoration2Tilemap = tilemap;
                }
                else if (tilemap.gameObject.tag == "frontTilemap")
                {
                    frontTilemap = tilemap;
                }
                else if (tilemap.gameObject.tag == "collisionTilemap")
                {
                    collisionTilemap = tilemap;
                }
                else if (tilemap.gameObject.tag == "minimapTilemap")
                {
                    minimapTilemap = tilemap;
                }

            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();
            if (character != null && !_room.IsPreviouslyVisited)
            {
                _room.IsPreviouslyVisited = true;
                OnEnterRoom?.Invoke(this);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //var character = collision.GetComponent<Character>();
            //if (character != null && _room.IsPreviouslyVisited)
            //{
            //    _room.IsPreviouslyVisited = false;
            //}
            OnExitRoom?.Invoke(this);
        }

        private void OnKillEnemyInRoom(int count)
        {
            if (this != _enemySpawnProvider.CurrentRoom) return;

            if (_room.EnemyAmount > 0)
            {
                _room.EnemyAmount = _room.EnemyAmount - count;

                if(_room.EnemyAmount <= 0)
                {
                    _room.IsClearEnemy = true;
                }
            }
        }

        private void AddDoors()
        {
            if (HelperUtilities.IsCorridor(_room.RoomType)) return;

            foreach(var door in _room.Doors)
            {
                if(door.doorPrefab != null && door.isConnected)
                {
                    float tileDistance = 1;

                    GameObject doorGameObject = Instantiate(door.doorPrefab, gameObject.transform);

                    if(door.orientation == DoorOrientation.Up)
                    {
                        doorGameObject.transform.localPosition = new Vector3(door.position.x +tileDistance / 2f, door.position.y + tileDistance, 0);
                    }
                    else if (door.orientation == DoorOrientation.Down)
                    {
                        doorGameObject.transform.localPosition = new Vector3(door.position.x + tileDistance / 2f, door.position.y, 0);
                    }
                    else if (door.orientation == DoorOrientation.Right)
                    {
                        doorGameObject.transform.localPosition = new Vector3(door.position.x + tileDistance, door.position.y + tileDistance*1.25f, 0);
                    }
                    else if (door.orientation == DoorOrientation.Left)
                    {
                        doorGameObject.transform.localPosition = new Vector3(door.position.x, door.position.y + tileDistance * 1.25f, 0);
                    }

                    Door scriptDoor = doorGameObject.GetComponent<Door>();
                    scriptDoor.Setup(this);
                }
            }
        }

        private void OnDestroy()
        {
            OnEnterRoom -= _gameStateProvider.OnEnterRoom;
            _enemySpawnProvider.OnKillEnemy -= OnKillEnemyInRoom;
        }

        private void Update()
        {
            if (isCreateTelepos) return;

            if (_room.IsHasTele && IsClear)
            {
                var port = Instantiate(_vfxTelepos, this.transform);
                var pos = grid.CellToWorld(new Vector3Int(_room.PositionTele.x, _room.PositionTele.y, 0));
                port.transform.position = pos;

                isCreateTelepos = true;
            }
        }
        
        /// <summary>
        /// Update obstacles used by AStar pathfinmding.
        /// </summary>
        private void AddObstaclesAndPreferredPaths()
        {
            // this array will be populated with wall obstacles 
            aStarMovementPenalty = new int[_room.TemplateUpperBounds.x - _room.TemplateLowerBounds.x + 1, _room.TemplateUpperBounds.y - _room.TemplateLowerBounds.y + 1];


            // Loop thorugh all grid squares
            for (int x = 0; x < (_room.TemplateUpperBounds.x - _room.TemplateLowerBounds.x + 1); x++)
            {
                for (int y = 0; y < (_room.TemplateUpperBounds.y - _room.TemplateLowerBounds.y + 1); y++)
                {
                    // Set default movement penalty for grid sqaures
                    aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                    // Add obstacles for collision tiles the enemy can't walk on
                    TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + _room.TemplateLowerBounds.x, y + _room.TemplateLowerBounds.y, 0));

                    foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                    {
                        if (tile == collisionTile)
                        {
                            aStarMovementPenalty[x, y] = 0;
                            break;
                        }
                    }

                    // Add preferred path for enemies (1 is the preferred path value, default value for
                    // a grid location is specified in the Settings).
                    if (tile == GameResources.Instance.preferredEnemyPathTile)
                    {
                        aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                    }
                }
            }
        }
        
        private void CreateItemObstaclesArray()
        {
            // this array will be populated during gameplay with any moveable obstacles
            aStarItemObstacles = new int[_room.TemplateUpperBounds.x - _room.TemplateLowerBounds.x + 1, _room.TemplateUpperBounds.y - _room.TemplateLowerBounds.y + 1];
        }
    }
}