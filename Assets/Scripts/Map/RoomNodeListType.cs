using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DreamLU
{
    public enum RoomType
    {
        None,
        SmallRoom,
        MediumRoom,
        LargeRoom,
        BossRoom,
        Entrance,
        Corridor,
        CorridorEW,
        CorridorNS,
        ChessRoom,
        UpgradeRoom,
        ExchangeRoom,
        SpecialExchangeRoom
    }

    [System.Serializable]
    public struct RoomNodeType
    {
        public bool disPlayInRoomNodeGraph;

        public RoomType type;
        public string RoomName;
    }


    [CreateAssetMenu(fileName = "RoomNodeListType", menuName = "Database/Map/Room Node List Type")]
    public class RoomNodeListType : ScriptableObject
    {
        public List<RoomNodeType> list;

        public RoomNodeType GetRoomNodeType(RoomType type, RoomNodeType defaultRoom)
        {
            if(list != null && list.FindIndex(x => x.type == type) is var index && index != -1)
            {
                return list[index];
            }

            return defaultRoom;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            if(list != null && list.Count > 0)
            {
                foreach(RoomNodeType node in list)
                {
                    HelperUtilities.ValidateCheckEmtyString(this, node.type.ToString(), node.RoomName);
                }
            }
        }
#endif
        #endregion
    }
}
