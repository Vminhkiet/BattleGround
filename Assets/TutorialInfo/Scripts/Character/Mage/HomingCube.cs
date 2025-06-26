using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCube : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public float homingDelay = 0.2f;

    private Transform target;
    private float lifeTimer;
    private float delayTimer;
    public bool isHoming = true;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void OnEnable()
    {
        lifeTimer = lifetime;
        delayTimer = homingDelay;
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
        {
            ReturnToPool();
            return;
        }

        if (!isHoming)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            return;
        }

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 7f);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //Ke dich mat mau
            ReturnToPool();
        } else if (other.CompareTag("DecorationObject"))
        {
            ReturnToPool();
        }    
    }

    void ReturnToPool()
    {
        ObjectPooler.Instance.ReturnCube(gameObject);
    }
}
