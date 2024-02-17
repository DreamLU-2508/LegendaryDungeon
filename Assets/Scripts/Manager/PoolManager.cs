using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DreamLU
{
    [System.Serializable] public class GameObjectEvent : UnityEvent<GameObject> { }
    
    [System.Serializable]
    public class PoolSource {
        public string name;
        public GameObject prefab;
        public int capacity = 0;
        public int preloadCount = 0;
    }
    
    [System.Serializable]
    public class DynamicPoolSource : PoolSource
    {
        public GameObjectEvent OnCreate;
        public GameObjectEvent OnRetrieve;
        public GameObjectEvent OnRelease;
        public GameObjectEvent OnDestroy;        
    }
    
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;
        
        //List<QuickPool> poolList;
        Dictionary<int, QuickPool> pools;
        Dictionary<string, QuickPool> namedPools;
        
        public Dictionary<int, QuickPool> Pools
        {
            get => pools;
        }
        
        Transform currentTransform;

        public Transform CurrentTransform => currentTransform;
        
        public List<PoolSource> predefinedPools;
        public List<DynamicPoolSource> dynamicPools;
        
        [Button]
        public void DestroyAllPools()
        {
            pools.Clear();
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            currentTransform = this.transform;
            pools = new Dictionary<int, QuickPool>();
            namedPools = new Dictionary<string, QuickPool>();            

            // Initialize Predefined Pools
            for (int i = 0; i < predefinedPools.Count; i++)
            {
                PoolSource ps = predefinedPools[i];
                QuickPool pool = new QuickPool(ps.prefab, currentTransform, ps.preloadCount, ps.capacity);
                pools.Add(pool.ID, pool);
                if (ps.name != null && ps.name.Trim().Length > 0)
                {
                    namedPools.Add(ps.name, pool);
                }
            }

            // Initlize Dynamic Pools
            for (int i = 0; i < dynamicPools.Count; i++)
            {
                DynamicPoolSource dps = dynamicPools[i];
                QuickPool pool = new QuickPool(dps.prefab, currentTransform, dps.preloadCount, dps.capacity);
                pool.DynamicPoolSource = dps;

                pools.Add(pool.ID, pool);
                namedPools.Add(dps.name, pool);
            }                        
        }
        
        public QuickPool CreatePool(GameObject prefab, int preloadCount, int capacity) {
            QuickPool pool = new QuickPool(prefab, currentTransform, preloadCount, capacity);
            pools.Add(pool.ID, pool);
            return pool;
        }
        
        public static QuickPool GetPool(int poolID)
        {
            return Instance.pools.ContainsKey(poolID) ? Instance.pools[poolID] : null;
        }
        
        public static QuickPool GetNamedPool(string name)
        {
            return Instance.namedPools.ContainsKey(name) ? Instance.namedPools[name] : null;
        }
        
        public static QuickPool GetPool(GameObject prefab)
        {
            int poolID = prefab.GetInstanceID();
            if (Instance.pools.ContainsKey(poolID))
            {
                return Instance.pools[poolID];
            }
            else
            {
                return Instance.CreatePool(prefab,0,0);
            }
        }
        
        public static void Release(GameObject pooledObject)
        {
            var comp = pooledObject.GetComponent<PoolInstanceID>();
            GetPool(comp.poolID).ReleaseObject(pooledObject);
        }
    }
}