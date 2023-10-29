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

        public Doorway Clone()
        {
            return new Doorway()
            {
                position = this.position,
                orientation = this.orientation,
                doorPrefab = this.doorPrefab,
                doorwayStartCopy = new DoorwayCopy()
                {
                    position = doorwayStartCopy.position,
                    width = doorwayStartCopy.width,
                    height = doorwayStartCopy.height
                },
                isConnected = this.isConnected,
                isUnavailable = this.isUnavailable
            };
        }
    }

    [System.Serializable]
    public struct DoorwayCopy
    {
        public Vector2Int position;
        public int width;
        public int height;
    }
}