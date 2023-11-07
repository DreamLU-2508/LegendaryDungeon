using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "Enemey_", menuName = "Database/Enemy/Enemy Data")]
    public class EnemeyData : ScriptableObject
    {
        public EnemeyID enemeyID;
        public string enemyName;
        public GameObject prefab;

        public int enemyMaxHeath;
        public float enemySpeed;
        public bool isBoss;
        public bool isElite;

        public WeaponID weaponID;
        public int collisionDamage;
    }

}