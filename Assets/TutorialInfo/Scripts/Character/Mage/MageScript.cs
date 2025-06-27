using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MageScript : MonoBehaviour, ICharacterSkill
{
    public GameObject cubePrefab;
    public float attackRange = 15f;
    public float shootDelay = 0.3f;
    public GameObject skillIndicatorPrefab;
    private GameObject activeIndicator;
    public float ultiRange = 4f;


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

    void ICharacterSkill.DrawUltiPosition(Vector2 input)
    {
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 dir = new Vector3(input.x, 0, input.y).normalized;
            Vector3 targetPos = transform.position + dir * ultiRange;

            if (activeIndicator == null)
            {
                activeIndicator = Instantiate(skillIndicatorPrefab);
            }

            activeIndicator.SetActive(true);
            activeIndicator.transform.localScale = new Vector3(1, 1, 1 )*ultiRange;
            activeIndicator.transform.rotation = Quaternion.Euler(90, 0, 0);
            activeIndicator.transform.position = targetPos;
        }
        else
        {
           /* if (activeIndicator != null)
                activeIndicator.SetActive(false);*/
        }
    }

    public void UseSpell(Vector2 input)
    {

    }
    public void UseSkill(Vector2 inputright)
    {
        StartCoroutine(SpawnSkillProjectiles(inputright));
    }

    Vector3 GetUltiTargetPosition(Vector2 inputDir)
    {
        if (inputDir.sqrMagnitude < 0.01f)
            return transform.position; 

        Vector3 inputDirection3D = new Vector3(inputDir.x, 0, inputDir.y).normalized;

        float skillDistance = 5f;

        Vector3 targetPos = transform.position + inputDirection3D * skillDistance;

        return targetPos;
    }

    private IEnumerator SpawnSkillProjectiles(Vector2 inputDir)
    {
        yield return new WaitForSeconds(shootDelay);
        int amount = 8;
        float duration = 2f;
        float interval = duration / amount;
        Vector3 inputPosition = GetUltiTargetPosition(inputDir);

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-2f, 2f),
                 5f,
                Random.Range(-2f, 2f)
            );

            Vector3 spawnPosition = inputPosition + randomOffset;

            GameObject cube = ObjectPooler.Instance.GetCube();
            cube.transform.position = spawnPosition;
            cube.transform.rotation = Quaternion.identity;

            HomingCube hc = cube.GetComponent<HomingCube>();
            hc.isHoming = false;  
            hc.SetTarget(null);   

            yield return new WaitForSeconds(interval);
        }
    }
}
