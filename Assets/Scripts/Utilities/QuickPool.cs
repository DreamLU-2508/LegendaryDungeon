using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class QuickPool : MonoBehaviour
    {
        public int ID { get; private set; }

        public int Cached { get => cache.Count; }

        /// <summary>
        /// Specify Zero or Negative means No limit.
        /// </summary>
        public int Capacity = 0;

        // How many objects should be preloaded?
        public int PreloadCount = 0;

        GameObject sourcePrefab;
        Stack<GameObject> cache;
        Transform parentTransform;

        private DynamicPoolSource dynamicPoolSource = null;

        public DynamicPoolSource DynamicPoolSource
        {
            get => dynamicPoolSource;
            set
            {
                dynamicPoolSource = value;
            }            
        }

        public QuickPool(GameObject prefab, Transform rootTransform, int preloadCount = 0, int capacity = 0)
        {
            this.sourcePrefab = prefab;
            this.ID = prefab.GetInstanceID();
            this.parentTransform = rootTransform;
            this.PreloadCount = preloadCount;
            this.Capacity = capacity;

            this.cache = new Stack<GameObject>();

            // Preload

            for (int i = 0; i < preloadCount; i++)
            {
//                Debug.Log("Creating preload item: " + i + " " + sourcePrefab.name + " " + PreloadCount + " "  + Capacity);
                var obj = CreateObject(Vector3.zero, Quaternion.identity, this.parentTransform);
                ReleaseObject(obj);
            }
        }

        

        public void ClearCache()
        {
            while (cache.Count > 0) {
                var obj = cache.Pop();                
                dynamicPoolSource?.OnDestroy?.Invoke(obj);
                GameObject.Destroy(obj); 
            };
        }

        #region Pool Methods

        public T RetrieveObject<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T:Component{
            GameObject obj = RetrieveObject(position, rotation, parent);
            return obj.GetComponent<T>();
        }

        /// <summary>
        /// This method always release a nonnull object
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="parent">Parent.</param>
        public GameObject RetrieveObject(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            // Find an item available in cache
            if (cache.Count > 0)
            {
                var clone = cache.Pop();
                if (clone != null)
                {
                    if (parent != null)
                    {
                        clone.transform.SetParent(parent);
                    }
                    clone.transform.localPosition = position;
                    clone.transform.localRotation = rotation;
                    clone.SetActive(true);
                    dynamicPoolSource?.OnRetrieve?.Invoke(clone);
                    return clone;
                }
                else
                {
                    Debug.LogWarning("Warn: pool object is destroyed manually: " + sourcePrefab.name);
                }
            }

            // Full? Create new item
            var newObj = CreateObject(position, rotation, parent);
            newObj.SetActive(true);
            dynamicPoolSource?.OnCreate?.Invoke(newObj);
            return newObj;
        }

        GameObject CreateObject(Vector3 position, Quaternion rotation, Transform parent)
        {
            var clone = Object.Instantiate(sourcePrefab, position, rotation, parent);
            PoolInstanceID pid = clone.AddComponent<PoolInstanceID>();
            pid.poolID = this.ID;
            return clone;
        }

        public void ReleaseObject(GameObject poolObject)
        {
            if (cache.Count < Capacity || Capacity <= 0)
            {
                dynamicPoolSource?.OnRelease?.Invoke(poolObject);

                // If not over capacity, put it back to cache
                cache.Push(poolObject);
                poolObject.SetActive(false);
                poolObject.transform.SetParent(this.parentTransform);
            }
            else
            {
                dynamicPoolSource?.OnDestroy?.Invoke(poolObject);
                // Otherwise, destroy it
                Object.Destroy(poolObject);
            }
        }

        #endregion
    }
}
