using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;


namespace DreamLU
{
    public class DungeonBuilder: MonoBehaviour, IDungeonBuilder
    {
        private Dictionary<string, InstancedRoom> dungeonBuilderRoomDictionary = new Dictionary<string, InstancedRoom>();
        [SerializeField] private RoomManifest roomTemplateList;
        [SerializeField] private RoomNodeListType roomNodeTypeList;
        [SerializeField] private RoomNodeGraphManifest roomNodeGraphManifest;
        [SerializeField] private Transform container;
        [SerializeField] private GameObject vfxTelepos;
        [SerializeField] private RoomData endGameMap;

        private bool dungeonBuildSuccessful;
        // private ThemeMapType theme = ThemeMapType.Default;
        [ShowInInspector]
        public List<RoomDataType> roomDatas = new List<RoomDataType>();
        public List<ThemeMapType> themeExcludes = new List<ThemeMapType>();
        private int maxBuild = 10000;
        private int countBuild;
        private List<Room> rooms = new List<Room>();
        private EndGameRoom endGameRoomData;
        [ShowInInspector]
        public int CountBuild => countBuild;

        [ShowInInspector]
        public Dictionary<string, InstancedRoom> DungeonBuilderRoomDictionary => dungeonBuilderRoomDictionary;
        
        private IGameStateProvider _gameStateProvider;

        private void Awake()
        {
            // roomDatas = roomTemplateList.GetThemeMapByType(ThemeMapType.Default);
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            _gameStateProvider.OnEndGame += OnEndGame;
        }

        [Button]
        public bool GenerateDungeon(LevelData levelData)
        {
            if (levelData.isChangeTheme)
            {
                var theme = roomTemplateList.GetRandomThemeMapType(themeExcludes);
                if (theme == ThemeMapType.None)
                {
                    roomDatas = roomTemplateList.GetThemeMapByType(ThemeMapType.Default);
                }
                else
                {
                    themeExcludes.Add(theme);
                    roomDatas = roomTemplateList.GetThemeMapByType(theme);
                }
                Debug.Log(theme);
            }
            
            rooms.Clear();
            dungeonBuildSuccessful = false;
            int count = 0;
            while (!dungeonBuildSuccessful && count < maxBuild)
            {
                countBuild = count;
                count++;
                RoomNodeGraph nodeGraph = null;
                if (levelData.isLevelBoss)
                {
                    nodeGraph = roomNodeGraphManifest.listRoomBoss[Random.Range(0, roomNodeGraphManifest.listRoomBoss.Count)];
                }
                else
                {
                    nodeGraph = roomNodeGraphManifest.listRoomNormal[Random.Range(0, roomNodeGraphManifest.listRoomNormal.Count)];
                }
                
                if (nodeGraph != null)
                {
                    ClearDungeon();

                    dungeonBuildSuccessful = AttemptToBuildRandomDungeon(nodeGraph);

                    if (dungeonBuildSuccessful)
                    {
                        // Instantiate Room Gameobjects
                        InstantiateRoomGameobjects();
                    }
                }
            }

            return dungeonBuildSuccessful;
        }

        public bool GenerateEndGameRoom()
        {
            rooms.Clear();
            dungeonBuildSuccessful = false;
            InstantiateEndGameRoom();

            return dungeonBuildSuccessful;
        }

        private bool AttemptToBuildRandomDungeon(RoomNodeGraph roomNodeGraph)
        {
            Queue<RoomNode> openRoomNodeQueue = new Queue<RoomNode>();
            RoomNode entranceNode = roomNodeGraph.GetRoomNodeByType(RoomType.Entrance);

            if (entranceNode != null)
            {
                openRoomNodeQueue.Enqueue(entranceNode);
                openRoomNodeQueueShow = openRoomNodeQueue;
            }
            else
            {
                Debug.Log("No Entrance Node");
                return false;  // Dungeon Not Built
            }

            bool noRoomOverlaps = true;
            noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

            if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [ShowInInspector, ReadOnly] private Queue<RoomNode> openRoomNodeQueueShow;
        [ShowInInspector, ReadOnly] private List<RoomNode> nodes = new List<RoomNode>();
        
        private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraph roomNodeGraph, Queue<RoomNode> openRoomNodeQueue, bool noRoomOverlaps)
        {
            nodes.Clear();
            foreach (var roomNode in roomNodeGraph.roomNodes)
            {
                nodes.Add(roomNode);
            }
            
            while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true && nodes.Count > 0)
            {
                RoomNode roomNode = openRoomNodeQueue.Dequeue();
                // Debug.LogError(roomNode.roomNodeType.type);
                foreach (RoomNode childRoomNode in roomNode.GetChildRoomNode())
                {
                    // Debug.LogError("child -- " + childRoomNode.roomNodeType.RoomName);
                    openRoomNodeQueue.Enqueue(childRoomNode);
                    openRoomNodeQueueShow = openRoomNodeQueue;
                }

                if (roomNode.roomNodeType.type == RoomType.Entrance)
                {
                    RoomData roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType.type);
                    InstancedRoom room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                    room.IsPositoned = true;
                    dungeonBuilderRoomDictionary.Add(room.ID, room);

                    var node = nodes.Find(x => x.id == room.ID);
                    if (node != null)
                    {
                        nodes.Remove(node);
                    }
                }
                else
                {
                    InstancedRoom parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];
                    noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
                }

            }

            return noRoomOverlaps;

        }

        private bool CanPlaceRoomWithNoOverlaps(RoomNode roomNode, InstancedRoom parentRoom)
        {
            bool roomOverlaps = true;
            while (roomOverlaps && nodes.Count > 0)
            {
                List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.Doors).ToList();

                if (unconnectedAvailableParentDoorways.Count == 0)
                {
                    return false; // room overlaps
                }

                Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

                RoomData roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);
                InstancedRoom room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

                if (PlaceTheRoom(parentRoom, doorwayParent, room))
                {
                    roomOverlaps = false;

                    // Mark room as positioned
                    room.IsPositoned = true;

                    // Add room to dictionary
                    dungeonBuilderRoomDictionary.Add(room.ID, room);
                    var node = nodes.Find(x => x.id == room.ID);
                    if (node != null)
                    {
                        nodes.Remove(node);
                    }
                }
                else
                {
                    roomOverlaps = true;
                }

            }

            return true;  // no room overlaps

        }

        private RoomData GetRandomTemplateForRoomConsistentWithParent(RoomNode roomNode, Doorway doorwayParent)
        {
            RoomData roomtemplate = null;

            // If room node is a corridor then select random correct Corridor room template based on
            // parent doorway orientation
            if (roomNode.roomNodeType.type == RoomType.Corridor || roomNode.roomNodeType.type == RoomType.CorridorNS || roomNode.roomNodeType.type == RoomType.CorridorEW)
            {
                switch (doorwayParent.orientation)
                {
                    case DoorOrientation.Up:
                    case DoorOrientation.Down:
                        roomtemplate = GetRandomRoomTemplate(RoomType.CorridorNS);
                        break;


                    case DoorOrientation.Right:
                    case DoorOrientation.Left:
                        roomtemplate = GetRandomRoomTemplate(RoomType.CorridorEW);
                        break;


                    case DoorOrientation.None:
                        break;

                    default:
                        break;
                }
            }
            // Else select random room template
            else
            {
                roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType.type);
            }


            return roomtemplate;
        }

        private bool PlaceTheRoom(InstancedRoom parentRoom, Doorway doorwayParent, InstancedRoom room)
        {
            Doorway doorway = GetOppositeDoorway(doorwayParent, room.Doors);

            // Return if no doorway in room opposite to parent doorway
            if (doorway == null)
            {
                // Just mark the parent doorway as unavailable so we don't try and connect it again
                doorwayParent.isUnavailable = true;

                return false;
            }

            // Calculate 'world' grid parent doorway position
            Vector2Int parentDoorwayPosition = parentRoom.LowerBounds + doorwayParent.position - parentRoom.TemplateLowerBounds;

            Vector2Int adjustment = Vector2Int.zero;

            // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)

            switch (doorway.orientation)
            {
                case DoorOrientation.Up:
                    adjustment = new Vector2Int(0, -1);
                    break;

                case DoorOrientation.Left:
                    adjustment = new Vector2Int(1, 0);
                    break;

                case DoorOrientation.Down:
                    adjustment = new Vector2Int(0, 1);
                    break;

                case DoorOrientation.Right:
                    adjustment = new Vector2Int(-1, 0);
                    break;

                case DoorOrientation.None:
                    break;

                default:
                    break;
            }

            // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
            room.LowerBounds = parentDoorwayPosition + adjustment + room.TemplateLowerBounds - doorway.position;
            room.UpperBounds = room.LowerBounds + room.TemplateUpperBounds - room.TemplateLowerBounds;

            InstancedRoom overlappingRoom = CheckForRoomOverlap(room);
            // overlappingRoom == null
            if (overlappingRoom == null)
            {
                // mark doorways as connected & unavailable
                doorwayParent.isConnected = true;
                doorwayParent.isUnavailable = true;

                doorway.isConnected = true;
                doorway.isUnavailable = true;

                // return true to show rooms have been connected with no overlap
                return true;
            }
            else
            {
                // Just mark the parent doorway as unavailable so we don't try and connect it again
                doorwayParent.isUnavailable = true;

                return false;
            }

        }

        private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
        {

            foreach (Doorway doorwayToCheck in doorwayList)
            {
                if (parentDoorway.orientation == DoorOrientation.Left && doorwayToCheck.orientation == DoorOrientation.Right)
                {
                    return doorwayToCheck;
                }
                else if (parentDoorway.orientation == DoorOrientation.Right && doorwayToCheck.orientation == DoorOrientation.Left)
                {
                    return doorwayToCheck;
                }
                else if (parentDoorway.orientation == DoorOrientation.Down && doorwayToCheck.orientation == DoorOrientation.Up)
                {
                    return doorwayToCheck;
                }
                else if (parentDoorway.orientation == DoorOrientation.Up && doorwayToCheck.orientation == DoorOrientation.Down)
                {
                    return doorwayToCheck;
                }
            }

            return null;
        }

        private InstancedRoom CheckForRoomOverlap(InstancedRoom roomToTest)
        {
            // Iterate through all rooms
            foreach (KeyValuePair<string, InstancedRoom> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                InstancedRoom room = keyvaluepair.Value;

                // skip if same room as room to test or room hasn't been positioned
                if (room.ID == roomToTest.ID || !room.IsPositoned)
                    continue;

                // If room overlaps
                if (IsOverLappingRoom(roomToTest, room))
                {
                    return room;
                }
            }


            // Return
            return null;

        }

        private bool IsOverLappingRoom(InstancedRoom room1, InstancedRoom room2)
        {
            //return false;

            bool isOverlappingX = IsOverLappingInterval(room1.LowerBounds.x, room1.UpperBounds.x, room2.LowerBounds.x, room2.UpperBounds.x);

            bool isOverlappingY = IsOverLappingInterval(room1.LowerBounds.y, room1.UpperBounds.y, room2.LowerBounds.y, room2.UpperBounds.y);

            if (isOverlappingX && isOverlappingY)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
        {
            if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private RoomData GetRandomRoomTemplate(RoomType roomNodeType)
        {
            List<RoomData> matchingRoomTemplateList = new List<RoomData>();

            foreach (RoomDataType roomTemplate in roomDatas)
            {
                if (roomTemplate.roomType == roomNodeType)
                {
                    matchingRoomTemplateList = roomTemplate.roomDatas;
                    break;
                }
            }

            if (matchingRoomTemplateList.Count == 0)
                return null;

            return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];

        }

        private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
        {
            // Loop through doorway list
            foreach (Doorway doorway in roomDoorwayList)
            {
                if (!doorway.isConnected && !doorway.isUnavailable)
                    yield return doorway;
            }
        }

        private InstancedRoom CreateRoomFromRoomTemplate(RoomData roomTemplate, RoomNode roomNode)
        {
            // Initialise room from template
            InstancedRoom room = new InstancedRoom(roomTemplate, roomNode);
            return room;

        }

        private void InstantiateRoomGameobjects()
        {
            // Iterate through all dungeon rooms.
            foreach (KeyValuePair<string, InstancedRoom> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                InstancedRoom instancedRoom = keyvaluepair.Value;

                Vector3 roomPosition = new Vector3(instancedRoom.LowerBounds.x - instancedRoom.TemplateLowerBounds.x, instancedRoom.LowerBounds.y - instancedRoom.TemplateLowerBounds.y, 0f);

                GameObject roomGameobject = Instantiate(instancedRoom.Prefab, roomPosition, Quaternion.identity, container);
                Room room = roomGameobject.GetComponent<Room>();
                if(room != null)
                {
                    room.SetData(instancedRoom, vfxTelepos);
                    room.Initialise(roomGameobject);
                    rooms.Add(room);
                }
            }
        }
        
        private void InstantiateEndGameRoom()
        {
            InstancedRoom instancedRoom = new InstancedRoom(endGameMap);

            Vector3 roomPosition = new Vector3(0, 0, 0);

            GameObject roomGameobject = Instantiate(instancedRoom.Prefab, roomPosition, Quaternion.identity, container);
            EndGameRoom room = roomGameobject.GetComponent<EndGameRoom>();
            if(room != null)
            {
                room.SetData(instancedRoom);
                room.Initialise(roomGameobject);
                endGameRoomData = room;
            }
        }

        public void ClearDungeon()
        {
            endGameRoomData = null;

            for (int i = container.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(container.GetChild(i).gameObject);
            }

            dungeonBuilderRoomDictionary.Clear();
        }

        public Vector3 GetPositionRoomEntrance()
        {
            if(rooms.Count > 0 && dungeonBuildSuccessful)
            {
                foreach(var room  in rooms)
                {
                    if(room.InstancedRoom.RoomType == RoomType.Entrance)
                    {
                        Vector3Int vector3Int = new Vector3Int((room.InstancedRoom.LowerBounds.x + room.InstancedRoom.UpperBounds.x) / 2, (room.InstancedRoom.LowerBounds.y + room.InstancedRoom.UpperBounds.y) / 2, 0);
                        return room.Grid.CellToWorld(vector3Int);
                    }
                }
            }

            return Vector3.zero;
        }
        
        public Vector3 GetPositionSpawnEndGameRoom()
        {
            if (endGameRoomData != null)
            {
                Vector3Int vector3Int = new Vector3Int(endGameMap.positionSpawnEndGame.x, endGameMap.positionSpawnEndGame.y, 0);
                return endGameRoomData.Grid.CellToWorld(vector3Int);
            }
            return Vector3.zero;
        }

        private void OnEndGame()
        {
            themeExcludes.Clear();
        }

        private void OnDestroy()
        {
            _gameStateProvider.OnEndGame -= OnEndGame;
        }
    }
}