using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public struct RoomDataType
    {
        public RoomType roomType;
        public List<RoomData> roomData;
    }

    [CreateAssetMenu(menuName = "Database/Map/RoomManifest")]
    public class RoomManifest : ScriptableObject
    {
        public List<RoomDataType> roomDataTypes;
    }

}