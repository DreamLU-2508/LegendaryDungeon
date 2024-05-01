using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

namespace DreamLU
{
    public class EndGameRoom : MonoBehaviour
    {
        private InstancedRoom _room;
        private Grid grid;
        private Tilemap groundTilemap;
        private Tilemap decoration1Tilemap;
        private Tilemap decoration2Tilemap;
        private Tilemap frontTilemap;
        private Tilemap collisionTilemap;
        private Tilemap minimapTilemap;

        private IEnemySpawnProvider _enemySpawnProvider;
        private IGameStateProvider _gameStateProvider;
        private IDropItemHandle _dropItemHandle;
        
        public InstancedRoom Room => _room;
        public Grid Grid => grid;
        
        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        public void SetData(InstancedRoom room)
        {
            _room = room;
        }

        public void Initialise(GameObject gameObject)
        {
            PopulateTilemapMemberVariables(gameObject);
            BlockOffUnusedDoorWays();
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
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            
        }

        private void OnDestroy()
        {
            
        }
    }
}