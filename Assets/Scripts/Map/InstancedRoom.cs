using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class InstancedRoom
    {
        private string roomDataID;
        private string roomNodeID;
        private GameObject prefab;
        private RoomType roomType;
        private Vector2Int lowerBounds;
        private Vector2Int upperBounds;
        private Vector2Int templateLowerBounds;
        private Vector2Int templateUpperBounds;
        private Vector2Int[] spawnPositionArray;
        private List<string> childRoomIDList;
        private string parentRoomID;
        private List<Doorway> doors;
        private bool isPositoned = false;
        private bool isLit = false;
        private bool isClearEnemy = false;
        private bool isPreviouslyVisited = false;
        private RoomData roomData;
        private int enemyAmount;
        private bool isHasTelepos;
        private Vector2Int positionTelepos;

        [ShowInInspector]
        public string ID => roomNodeID;

        [ShowInInspector]
        public Vector2Int LowerBounds
        {
            get { return lowerBounds; }
            set { lowerBounds = value; }
        }

        [ShowInInspector]
        public Vector2Int UpperBounds
        {
            get { return upperBounds; }
            set { upperBounds = value; }
        }

        [ShowInInspector]
        public Vector2Int TemplateLowerBounds
        {
            get { return templateLowerBounds; }
            set { templateLowerBounds = value; }
        }

        [ShowInInspector]
        public Vector2Int TemplateUpperBounds
        {
            get { return templateUpperBounds; }
            set { templateUpperBounds = value; }
        }

        public bool IsPreviouslyVisited
        {
            get { return isPreviouslyVisited; }
            set { isPreviouslyVisited = value; }
        }

        public bool IsClearEnemy
        {
            get { return isClearEnemy; }
            set { isClearEnemy = value; }
        }

        public GameObject Prefab => prefab;

        [ShowInInspector]
        public List<Doorway> Doors => doors;

        [ShowInInspector]
        public bool IsPositoned
        {
            get { return isPositoned; }
            set { isPositoned = value; }
        }

        [ShowInInspector]
        public RoomType RoomType => roomType;

        [ShowInInspector]
        public string ParentRoomID => parentRoomID;

        public RoomData RoomData { get { return roomData; } }

        public int EnemyAmount { get { return enemyAmount; }
            set { enemyAmount = value; }
        }
        public Vector2Int[] SpawnPositionArray => spawnPositionArray;
        public bool IsHasTele => isHasTelepos;
        public Vector2Int PositionTele => positionTelepos;

        public InstancedRoom(RoomData roomData, RoomNode roomNode)
        {
            this.roomDataID = roomData.guid;
            this.roomNodeID = roomNode.id;
            this.prefab = roomData.prefab;
            this.roomType = roomData.roomType;
            this.lowerBounds = roomData.lowerBounds;
            this.upperBounds = roomData.upperBounds;
            this.templateLowerBounds = roomData.lowerBounds;
            this.templateUpperBounds = roomData.upperBounds;
            this.spawnPositionArray = roomData.spawnPositionArray;
            this.childRoomIDList = HelperUtilities.Clone<string>(roomNode.childRoomNodeIDList);
            this.doors = roomData.CloneDoorwayList();
            this.roomData = roomData;
            this.enemyAmount = roomData.enemyAmount;

            if (roomNode.parentRoomNodeIDList.Count == 0)
            {
                this.parentRoomID = "";
                this.isPreviouslyVisited = true;
            }
            else
            {
                this.parentRoomID = roomNode.parentRoomNodeIDList[0];
            }

            if(this.roomType == RoomType.Entrance || this.roomType == RoomType.ChessRoom || this.roomType == RoomType.EndRoom)
            {
                this.isClearEnemy = true;
            }
            
            if(this.roomType == RoomType.BossRoom || this.roomType == RoomType.EndRoom)
            {
                this.isHasTelepos = true;
                this.positionTelepos = roomData.positionTele;
            }
        }
    }
}
