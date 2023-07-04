using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    
    [SerializeField] [Range(0,50)] int poolSize = 10;
    [SerializeField] [Range(0.1f, 30f)]float spawnInterval = 1f;

    GameObject[] pool;
    
    void Awake() {
        PopulatePool();    
    }
    void Start()
    {
        StartCoroutine(spawnEnemies());
    }

    void PopulatePool()
    {
        pool = new GameObject[poolSize];

        for(int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(enemyPrefab, transform);
            pool[i].SetActive(false);
        }
    }

    void EnableObjectInPool()
    {
        foreach(GameObject enemy in pool)
        {
            if(!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return;
            }
        }
    }

    IEnumerator spawnEnemies()
    {
        while(true)
        {
            EnableObjectInPool();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
