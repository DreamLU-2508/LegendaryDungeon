using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class InstancedRoom
    {
        private string id;
        private GameObject prefab;
        private RoomType roomType;
        private Vector2Int lowerBounds;
        private Vector2Int upperBounds;
        private List<string> childRoomIDList;
        private string parentRoomID;
        private List<Doorway> doors;
        private bool isPositoned = false;
        private bool isLit = false;
        private bool isClearEnemy = false;
        private bool isPreviouslyVisited = false;
    }
}
