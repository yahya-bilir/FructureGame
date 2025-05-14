using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pool
{
    public class PoolSystem : SingletonMonoBehaviour<PoolSystem>
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        protected override void Awake()
        {
            base.Awake();
            SpawnPool();
        }

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        void SpawnPool()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    obj.AddComponent<PoolableObject>().PoolTag = pool.tag;
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnGameObject(string poolTag)
        {
            if (!poolDictionary.ContainsKey(poolTag))
            {
                Debug.LogWarning("Pool with tag " + poolTag + " doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[poolTag].Dequeue();

            objectToSpawn.SetActive(true);

            //poolDictionary[poolTag].Enqueue(objectToSpawn);
            
            return objectToSpawn;
        }
    
        public void ReturnToPool(string poolTag, GameObject obj) => poolDictionary[poolTag].Enqueue(obj);
    }
}