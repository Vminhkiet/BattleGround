using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCube : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private float lifeTimer;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void OnEnable()
    {
        lifeTimer = 5f;
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
        {
            ReturnToPool();
            return;
        }

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void ReturnToPool()
    {
        ObjectPooler.Instance.ReturnCube(gameObject);
    }
}
