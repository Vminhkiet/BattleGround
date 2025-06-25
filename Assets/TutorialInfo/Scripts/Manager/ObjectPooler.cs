using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ObjectPooler:MonoBehaviour
{
    public static ObjectPooler Instance;

    public GameObject projectilePrefab;
    public int poolSize = 10;

    private Queue<GameObject> cubePool = new Queue<GameObject>();

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
        if (cubePool.Count > 0)
        {
            GameObject obj = cubePool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(projectilePrefab);
            return obj;
        }
    }

    public void ReturnCube(GameObject obj)
    {
        obj.SetActive(false);
        cubePool.Enqueue(obj);
    }
}
