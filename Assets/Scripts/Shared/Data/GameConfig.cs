using UnityEngine;

namespace DreamLU
{
    public enum PlayTarget
    {
        Main = 0,
        Demo = 1,
    }

    [CreateAssetMenu(menuName = "Database/GameConfig", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public PlayTarget target;
        public bool enableCheat;

        public float fireRateCoolDown = 0.2f;
        public float distanceFollowCharacter = 10f;
        public float maxSpawnEnemyInRoom = 10;

        [Header("Level Config")] 
        public int maxLevel;
    }
}
