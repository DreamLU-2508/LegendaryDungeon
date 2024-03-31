using System.Collections.Generic;
using DreamLU;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Skill/Skill Data")]
    public class SkillData : ScriptableObject
    {
        public SkillBase prefab;
        [TableList] public float damageScaling;
        [TableList] public float cooldown;
        [TableList] public float wanderingAfterComplete;
        [TableList] public Vector2 castRange;
    }
}