using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Enemy/EnemyDataManifest")]
    public class EnemeyDataManifest : ScriptableObject
    {
        public List<EnemeyData> list;
        public List<BossData> listBoss;

        public BossData GetBossData(List<BossData> bossDatasExclude)
        {
            if (listBoss.Count > 0)
            {
                ChancefTable<BossData> chancefTable = new ChancefTable<BossData>();
                URandom random = URandom.CreateSeeded();

                foreach (var bossData in listBoss)
                {
                    if (bossDatasExclude.Contains(bossData))
                    {
                        continue;
                    }

                    chancefTable.AddRange(1, bossData);
                }

                if (chancefTable.CanRoll)
                {
                    return chancefTable.RollWithinMaxRange(random);
                }
            }

            return null;
        }
    }

}