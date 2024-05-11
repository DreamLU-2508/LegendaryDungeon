using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public class TierSelector
    {
        public int tier;
        public int levelMin;
        public int levelMax;
    }
    
    [CreateAssetMenu(menuName = "Database/Enemy/EnemyDataManifest")]
    public class EnemeyDataManifest : ScriptableObject
    {
        public List<EnemyData> list;
        public List<BossData> listBoss;
        
        [TableList]
        public List<TierSelector> tierSelectors;

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
        
        public EnemyData GetEnemyByID(EnemyID enemyID)
        {
            return list.Find(x => x.enemyID == enemyID);
        }

        public List<EnemyData> GetListByLevel(int level)
        {
            List<EnemyData> l = new List<EnemyData>();
            foreach (var enemyData in list)
            {
                var foundSelector = tierSelectors.Find((x) => x.tier == enemyData.metaTier);
                if (foundSelector != null && foundSelector.levelMin <= level && foundSelector.levelMax >= level)
                {
                    l.Add(enemyData);
                }
            }

            return l;
        }
    }

}