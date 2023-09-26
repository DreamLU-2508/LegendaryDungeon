using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class DungeonBuilder : MonoBehaviour, IDungeonBuilder
    {
        [SerializeField] private RoomNodeGraphManifest _roomNodeGraphManifest;
        [SerializeField] private RoomManifest _roomManifest;
        [SerializeField] private RoomNodeGraph roomNodeGraphDefault;

        private void Awake()
        {
            
        }

        public bool GenerateMap()
        {
            // Get random room node type
            RoomNodeGraph roomNodeGraph = roomNodeGraphDefault;
            if (_roomNodeGraphManifest.list != null && _roomNodeGraphManifest.list.Count > 0)
            {
                int randomIndex = Random.Range(0, _roomNodeGraphManifest.list.Count);
                roomNodeGraph = _roomNodeGraphManifest.list[randomIndex];
            }

            if(roomNodeGraph != null)
            {
                //ClearMap();
            }

            return false;
        }
    }
}
