using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Level/LevelsDataManifest")]
    public class LevelsDataManifest : ScriptableObject
    {
        public float protoHealthModPerLevel = 0.2f;
        public float protoDamageModPerLevel = 0.2f;
        
        public List<LevelData> levels;

        public LevelData GetLevelData(int level)
        {
            var lvlData = levels.Find(x => x.level == level);

            // For prototyping purpose, if level is not yet defined, just use the last level
            if (lvlData == null)
            {
                return levels.Last();
            }

            return lvlData;
        }
    }
}