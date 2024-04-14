using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public enum ThemeMapType
    {
        None,
        Default,
        Map1,
        Map2,
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

        public ThemeMapType GetRandomThemeMapType(List<ThemeMapType> themeMapTypes)
        {
            if (roomDataTypes.Count > 0)
            {
                ChancefTable<ThemeMapType> chancefTable = new ChancefTable<ThemeMapType>();
                URandom random = URandom.CreateSeeded();

                foreach (var themeMap in roomDataTypes)
                {
                    if (themeMapTypes.Contains(themeMap.type))
                    {
                        continue;
                    }

                    chancefTable.AddRange(1, themeMap.type);
                }

                if (chancefTable.CanRoll)
                {
                    return chancefTable.RollWithinMaxRange(random);
                }
            }

            return ThemeMapType.None;
        }

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