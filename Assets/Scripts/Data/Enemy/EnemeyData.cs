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

        public float enemySpeed;
        public AmmoData ammoData;
    }

}