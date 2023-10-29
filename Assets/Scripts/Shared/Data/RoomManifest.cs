using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public enum ThemeMapType
    {
        None,
        Default
    }

    [System.Serializable]
    public struct ThemeMap
    {
        public ThemeMapType type;
        public List<RoomDataType> roomDataTypes;
    }

    [System.Serializable]
    public struct RoomDataType
    {
        public RoomType roomType;
        public List<RoomData> roomDatas;
    }

    [CreateAssetMenu(menuName = "Database/Map/RoomManifest")]
    public class RoomManifest : ScriptableObject
    {
        public List<ThemeMap> roomDataTypes;

        public List<RoomDataType> GetThemeMapByType(ThemeMapType type)
        {
            int themeMapIndex = roomDataTypes.FindIndex(x => x.type == type);
            if(themeMapIndex != -1)
            {
                return roomDataTypes[themeMapIndex].roomDataTypes;
            }

            return null;
        }
    }

}