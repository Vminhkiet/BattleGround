using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageScript : MonoBehaviour, ICharacterSkill
{
    public GameObject cubePrefab;
    public float attackRange = 15f;
    public float shootDelay = 0.3f;

    public void NormalAttack(Vector2 inputright)
    {
        StartCoroutine(DelayedShoot());     
    }

    private IEnumerator DelayedShoot()
    {
        yield return new WaitForSeconds(shootDelay);
        FireProjectile();
    }

    private void FireProjectile()
    {
        Transform target = FindClosestEnemy();

        GameObject cube = ObjectPooler.Instance.GetCube();
        cube.transform.position = transform.position;
        cube.transform.rotation = transform.rotation;

        HomingCube hc = cube.GetComponent<HomingCube>();
        hc.SetTarget(target);
    }


    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance && dist <= attackRange)
            {
                closestDistance = dist;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    public void UseSkill()
    {
        
    }

}
