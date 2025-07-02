using Photon.Pun;
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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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

    public void SpawnExplosion(Vector3 position)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("RPC_SpawnExplosion", RpcTarget.All, position);
    }

    [PunRPC]
    void RPC_SpawnExplosion(Vector3 position)
    {
        GameObject explosion = Get();
        explosion.transform.position = position;

        ParticleSystem[] ps = explosion.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in ps)
        {
            p.Clear();
            p.Play();
        }
    }
}
