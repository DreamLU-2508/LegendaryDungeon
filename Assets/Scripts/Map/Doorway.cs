using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public class Doorway
    {
        public Vector2Int position;
        public DoorOrientation orientation;
        public GameObject doorPrefab;

        public DoorwayCopy doorwayStartCopy;

        public bool isConnected = false;
        public bool isUnavailable = false;
    }

    public struct DoorwayCopy
    {
        public Vector2Int position;
        public int width;
        public int height;
    }
}