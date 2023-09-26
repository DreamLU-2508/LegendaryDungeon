using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Enemy/EnemyDataManifest")]
    public class EnemeyDataManifest : ScriptableObject
    {
        public List<EnemeyData> list;
    }

}