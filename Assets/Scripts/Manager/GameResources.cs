using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class GameResources : MonoBehaviour
    {
        private static GameResources instance;

        public static GameResources Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = Resources.Load<GameResources>("Prefabs/GameResources");
                }
                return instance;
            }
        }

        [Header("Map")]
        public RoomNodeListType roomNodeListType;
    }

}