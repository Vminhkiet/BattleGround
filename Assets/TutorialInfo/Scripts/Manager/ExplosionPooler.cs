using System;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPooler : MonoBehaviour
{
    public static ExplosionPooler Instance;

    [Header("Explosion Pool Settings")]
    public GameObject explosionPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(explosionPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(explosionPrefab);
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
