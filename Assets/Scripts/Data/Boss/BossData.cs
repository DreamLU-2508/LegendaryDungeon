using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public struct SkillExecuteChance
    {
        public SkillData data;
        public float chance;
    }

    [CreateAssetMenu(fileName = "Boss_", menuName = "Database/Enemy/Boss Data")]
    public class BossData : ScriptableObject
    {
        public EnemyID enemyID;
        public EnemyStat stat;
        public GameObject bossPrefab;

        [Header("Skill")] 
        [TableList] public List<SkillExecuteChance> skills;
        public float cooldownActiveSkill;

        public List<SkillData> GetSkillDataList()
        {
            return skills.Select(skill => skill.data).ToList();
        }

        public List<float> GetSkillChanceList()
        {
            return skills.Select(skill => skill.chance).ToList();
        }
    }
}
