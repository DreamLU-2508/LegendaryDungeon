using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Level/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Map setup")]
        public int level;
        public bool isChangeTheme;

        [Header("Boss setup")]
        public bool isLevelBoss;
        [ShowIf("isLevelBoss")] public float bossDamageBonus;
        [ShowIf("isLevelBoss")] public float bossHPBonus;

        /// <summary>
        /// The global drop chance modifier for this level.
        /// </summary>
        public float goldDropChanceMultiplier = 1.0f;
        
        [ReadOnly] public float score;
        [ReadOnly] public float scoreRoomSmall;
        [ReadOnly] public float scoreRoomMedium;
        [ReadOnly] public float scoreRoomLarge;
    }
}