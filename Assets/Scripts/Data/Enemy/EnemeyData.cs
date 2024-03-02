using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
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
    public class EnemeyData : ScriptableObject
    {
        public EnemeyID enemeyID;
        public string enemyName;
        public GameObject prefab;

        public WeaponID weaponID;
        
        public EnemyStat stat;
    }

}