using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public struct Score
    {
        public int level;
        public float score;
        public float speedMod;
        public float healthMod;
        public float dropChanceMultiplier;
    }
    
    [CreateAssetMenu(menuName = "Database/Level/LevelsDataManifest")]
    public class LevelsDataManifest : ScriptableObject
    {
        public float protoHealthModPerLevel = 0.2f;
        public float protoDamageModPerLevel = 0.2f;
        
        public List<LevelData> levels;

        [Header("Score")]
        public float baseScore = 80;
        public float increasePerLevel = 0.25f;
        public float factor = 1.05f;
        public float multiplesRoom = 1.5f;
        
        [TableList][ShowInInspector]
        public List<Score> scores;
        
        
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
        
        public Score GetLevelScore(int level)
        {
            int idx = scores.FindIndex((x) => x.level == level);
            if (idx != -1)
            {
                return scores[idx];
            }
            else
            {
                if (scores.Count > 0)
                {
                    return scores[^1];
                }
                else
                {
                    throw new System.Exception("No Data Provided");
                }
            }
        }
        
        [Button]
        void BuildScore(int levelCount = 15)
        {
            scores.Clear();
            for (int idx = 1; idx <= levelCount; idx++)
            {
                float lvl = idx;
                float pow = Mathf.Pow(factor, lvl);
                float score = baseScore * pow * (1f + lvl*increasePerLevel);
                scores.Add(new Score() {level = idx, score = score});
            }
        }
    }
}