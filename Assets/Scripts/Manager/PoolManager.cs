using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;

        [SerializeField] private List<Pool> poolArray = null;

        private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();
        private Transform poolTransform;

        public static PoolManager Instance => _instance;

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }

            poolTransform = GetComponent<Transform>();
        }

        private void Start()
        {
            if(poolArray != null && poolArray.Count > 0)
            {
                foreach(Pool pool in poolArray)
                {
                    CreatePool(pool);
                }
            }
        }

        private void CreatePool(Pool pool)
        {
            int poolKey = pool.prefab.GetInstanceID();

            string prefabName = pool.prefab.name;

            GameObject parentPrefabGameObject = new GameObject(prefabName + "Anchor");

            parentPrefabGameObject.transform.SetParent(poolTransform);

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary.Add(poolKey, new Queue<Component>());

                for(int i = 0; i < pool.poolSize; i++)
                {
                    GameObject newObject = Instantiate(pool.prefab, parentPrefabGameObject.transform) as GameObject;
                    newObject.SetActive(false);
                    poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(pool.componentType)));
                }
            }
        }

        public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            int poolKey = prefab.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                Component componentToReuse = GetComponentFormPool(poolKey);

                ResetObject(prefab, componentToReuse, position, rotation);

                return componentToReuse;
            }
            else
            {
                Debug.LogError("No object pool for " + prefab);
                return null;
            }
        }

        private Component GetComponentFormPool(int poolKey)
        {
            Component componentToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(componentToReuse);

            if(componentToReuse.gameObject.activeSelf)
            {
                componentToReuse.gameObject.SetActive(false);
            }

            return componentToReuse;
        }

        public void ResetObject(GameObject prefab, Component componentToReuse, Vector3 position, Quaternion rotation)
        {
            componentToReuse.transform.position = position;
            componentToReuse.transform.rotation = rotation;
            componentToReuse.transform.localScale = prefab.transform.localScale;
        }

        public static Type FindType(string qualifiedTypeName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }

    }
}