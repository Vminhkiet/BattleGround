using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPooler : MonoBehaviourPun
{
    public static ObjectPooler Instance;

    public GameObject projectilePrefab;
    public int poolSize = 10;

    private Queue<GameObject> cubePool = new Queue<GameObject>();
    private List<GameObject> activeProjectiles = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            cubePool.Enqueue(obj);
        }
    }

    public GameObject GetCube()
    {
        GameObject obj;
        if (cubePool.Count > 0)
        {
            obj = cubePool.Dequeue();
        }
        else
        {
            obj = Instantiate(projectilePrefab);
        }

        obj.SetActive(true);
        activeProjectiles.Add(obj);
        return obj;
    }

    public void ReturnCube(GameObject obj)
    {
        obj.SetActive(false);
        cubePool.Enqueue(obj);
        activeProjectiles.Remove(obj);
    }

    public void SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        photonView.RPC("RPC_SpawnProjectile", RpcTarget.All, position, rotation);
    }

    public void SpawnProjectileWithTarget(Vector3 position, Quaternion rotation, int targetViewID)
    {
        photonView.RPC("RPC_SpawnProjectileWithTarget", RpcTarget.All, position, rotation, targetViewID);
    }

    [PunRPC]
    void RPC_SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetCube();
        obj.GetComponent<HomingCube>().isHoming = false;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
    }

    [PunRPC]
    void RPC_SpawnProjectileWithTarget(Vector3 position, Quaternion rotation, int targetViewID)
    {
        GameObject obj = GetCube();
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        HomingCube hc = obj.GetComponent<HomingCube>();

        if (targetViewID != -1)
        {
            PhotonView targetView = PhotonView.Find(targetViewID);
            if (targetView != null)
            {
                hc.SetTarget(targetView.transform);
            }
        }
        else
        {
            hc.SetTarget(null);
        }
        hc.isHoming = true;
    }

    public void DespawnProjectile(GameObject projectile)
    {
        photonView.RPC("RPC_DespawnProjectileAt", RpcTarget.All, projectile.transform.position);
    }

    [PunRPC]
    void RPC_DespawnProjectileAt(Vector3 position)
    {
        GameObject target = FindClosestActiveProjectile(position);
        if (target != null)
        {
            ReturnCube(target);
        }
    }

    GameObject FindClosestActiveProjectile(Vector3 position)
    {
        float closestDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject obj in activeProjectiles)
        {
            float dist = Vector3.Distance(obj.transform.position, position);
            if (dist < 0.5f) // điều chỉnh threshold nếu cần
            {
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = obj;
                }
            }
        }

        return closest;
    }
}
