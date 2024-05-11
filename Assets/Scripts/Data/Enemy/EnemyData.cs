using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DreamLU
{
    public class EnemyStatMod
    {
        public float healthMod;
        public float damageMod;
    }
    
    [System.Serializable]
    public class EnemyStat
    {
        public int maxHealth;
        public int damageBase;
        public float moveSpeed;
        
        public int collisionDamage;
        
        public bool isBoss;
        public bool isElite;

        [Header("Chase Distance")]
        public float chaseDistance;
    }
    
    [CreateAssetMenu(fileName = "Enemey_", menuName = "Database/Enemy/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public EnemyID enemyID;
        public string enemyName;
        public GameObject prefab;

        [FormerlySerializedAs("weaponID")] public ItemID itemID;
        
        public EnemyStat stat;
        
        //Score
        /// <summary>
        /// Modifier for individual enemies. Default is 1.
        /// </summary>
        public float scoreModifier = 1.0f;
        public int metaTier;
        
        [ShowInInspector]
        public float Score => CalcScore(new EnemyStatMod());
        
        public float CalcScore(EnemyStatMod statMod)
        {
            return (stat.maxHealth * (1+statMod.healthMod) + stat.moveSpeed +
                    Mathf.Max(stat.collisionDamage, stat.damageBase) * (1 + statMod.damageMod)) * scoreModifier;
        }
    }

}