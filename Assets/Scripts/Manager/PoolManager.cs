using System;
using System.Collections;
using System.Collections.Generic;
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
            var poolsInstance = this.GetComponentsInChildren<PoolInstanceID>(true);
            if (poolsInstance != null)
            {
                foreach (var poolInstance in poolsInstance)
                {
                    Destroy(poolInstance.gameObject);
                }
            }
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
        
        public QuickPool CreatePool(GameObject prefab, Transform parentTransform, int preloadCount, int capacity) {
            QuickPool pool = new QuickPool(prefab, parentTransform, preloadCount, capacity);
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
        
        public static QuickPool GetPool(GameObject prefab, Transform parentTransform = null)
        {
            int poolID = prefab.GetInstanceID();
            if (Instance.pools.ContainsKey(poolID))
            {
                return Instance.pools[poolID];
            }
            else
            {
                return Instance.CreatePool(prefab, parentTransform == null ? Instance.CurrentTransform : parentTransform,0,0);
            }
        }
        
        public static void Release(GameObject pooledObject)
        {
            var comp = pooledObject.GetComponent<PoolInstanceID>();
            GetPool(comp.poolID).ReleaseObject(pooledObject);
        }

        public static void RegisterParent(string nameParent, out Transform transformParent)
        {
            transformParent = null;

            var gameObject = new GameObject(nameParent);
            gameObject.transform.SetParent(Instance.currentTransform);
            transformParent = gameObject.transform;
        }

        [Button]
        public void TestRegisterParent(string name, bool isDestroy)
        {
            if (isDestroy)
            {
                var gO = GameObject.Find(name);
                if (gO != null)
                {
                    Destroy(gO);
                    Debug.LogError("Destroy");
                }
                else
                {
                    Debug.LogError("Not Found");
                }
            }
            else
            {
                RegisterParent(name, out var transformParent);
            }
        }
    }
}