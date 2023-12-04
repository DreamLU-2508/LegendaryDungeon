using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Map/RoomNodeGraphManifest")]
    public class RoomNodeGraphManifest : ScriptableObject
    {
        public List<RoomNodeGraph> listRoomNormal;

        public List<RoomNodeGraph> listRoomBoss;
    }

}