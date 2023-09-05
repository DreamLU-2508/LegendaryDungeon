using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Database/Map/Room Node Graph")]
    public class RoomNodeGraph : ScriptableObject
    {
        public RoomNodeListType roomNodeTypeList;
        public List<RoomNode> roomNodes = new List<RoomNode>();
        public Dictionary<string, RoomNode> roomNodesDictionary = new Dictionary<string, RoomNode>();

        private void Awake()
        {
            LoadData();
        }

        private void LoadData()
        {
            if(roomNodes != null && roomNodes.Count > 0)
            {
                roomNodesDictionary.Clear();
                foreach (var roomNode in roomNodes)
                {
                    if (!roomNodesDictionary.ContainsKey(roomNode.id))
                    {
                        roomNodesDictionary.Add(roomNode.id, roomNode);
                    }
                }
            }
        }

#if UNITY_EDITOR

        public RoomNode roomNodeStartDrawLine = null;
        public Vector2 positionEndLine = Vector2.zero;


        public void OnValidate()
        {
            LoadData();
        }
#endif
    }

}