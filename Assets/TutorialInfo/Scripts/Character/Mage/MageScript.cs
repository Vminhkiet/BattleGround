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
    private Vector3 currentTargetPosition;
    public float targetingRange = 8f;
    public void SetEffectSkill(IEffectPlayer effectPlayer)
    {

    }
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
        cube.transform.position = new Vector3(transform.position.x,transform.position.y+0.5f, transform.position.z) ;
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


    public void DrawUltiPosition(Vector2 input)
    {
        if (input.sqrMagnitude > 0.01f)
        {
            float inputMagnitude = Mathf.Clamp01(input.magnitude);
            Vector3 dir = new Vector3(input.x, 0, input.y).normalized;
            Vector3 targetPos = transform.position + dir * inputMagnitude * targetingRange;

            if (activeIndicator == null)
            {
                activeIndicator = Instantiate(skillIndicatorPrefab);
                activeIndicator.transform.rotation = Quaternion.Euler(90, 0, 0);
                activeIndicator.transform.localScale = Vector3.one * ultiRange * 2f;
            }

            activeIndicator.SetActive(true);
            activeIndicator.transform.position = targetPos;

            currentTargetPosition = targetPos;
        }
        else
        {
            if (activeIndicator != null)
                activeIndicator.SetActive(false);
        }
    }

    public void UseSkill(Vector2 inputright)
    {
        StartCoroutine(SpawnSkillProjectiles(inputright));
    }


    private IEnumerator SpawnSkillProjectiles(Vector2 input)
    {
        yield return new WaitForSeconds(shootDelay);

        int amount = 8;
        float duration = 2f;
        float interval = duration / amount;

        float inputMagnitude = Mathf.Clamp01(input.magnitude);
        Vector3 dir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 inputPosition = transform.position + dir * inputMagnitude * targetingRange;

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-ultiRange / 2f, ultiRange / 2f),
                10f,
                Random.Range(-ultiRange / 2f, ultiRange / 2f)
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
    public void UseSpell(Vector2 input)
    {

    }
}
