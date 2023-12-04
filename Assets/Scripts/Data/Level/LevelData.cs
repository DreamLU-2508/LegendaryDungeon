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

        [Header("Boss setup")]
        public bool isLevelBoss;
        [ShowIf("isLevelBoss")] public int bossDamageBonus;
        [ShowIf("isLevelBoss")] public int bossHPBonus;
    }
}