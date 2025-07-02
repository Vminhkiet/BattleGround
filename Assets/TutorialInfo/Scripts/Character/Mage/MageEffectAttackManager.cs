using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MageEffectAttackManager : MonoBehaviour, IEffectAttackManager
{
    public float shootDelay = 0.3f;
    public float targetingRange = 8f;
    public float ultiRange = 4f;
    public float attackRange = 8f;
    public float healAmount = -500;

    private Transform _transform;
    private PhotonView photonView;

    private void Start()
    {
        _transform = GetComponentInChildren<MageScript>().gameObject.transform;
        photonView=GetComponent<PhotonView>();
    }
    public void PlayNormalAttack1()
    {
        StartCoroutine(DelayedShoot());
    }

    public void PlayNormalAttack2(Vector2 direction)
    {
        
    }

    public void PlayNormalAttack3(Vector2 direction)
    {
        
    }
    private IEnumerator DelayedShoot()
    {
        yield return new WaitForSeconds(shootDelay);
        FireProjectile();
    }

    private void FireProjectile()
    {
        if (!photonView.IsMine) return; 

        Transform target = FindClosestEnemy();

        Vector3 spawnPos = new Vector3(_transform.position.x, _transform.position.y + 0.5f, _transform.position.z);
        Quaternion rotation = _transform.rotation;

        ObjectPooler.Instance.SpawnProjectileWithTarget(spawnPos, rotation, target ? target.GetComponent<PhotonView>()?.ViewID ?? -1 : -1);
    }

    Transform FindClosestEnemy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject player in players)
        {
            if (player.transform == transform) continue;

            float dist = Vector3.Distance(_transform.position, player.transform.position);
            if (dist < closestDistance && dist <= attackRange)
            {
                closestDistance = dist;
                closestPlayer = player.transform;
            }
        }
        return closestPlayer;
    }


    public void PlaySpell(Vector2 direction)
    {
        StartCoroutine(SpawnSkillProjectiles(direction));
    }

    public void PlayUlti(Vector2 position)
    {
        
    }

    public void RotateEffect(ParticleSystem ps, Vector2 direction)
    {
        
    }

    public void StopAllEffects()
    {
       
    }

    public void TurnOffUlti()
    {
       
    }

    public void TurnOnUlti()
    {
        
    }

    private IEnumerator SpawnSkillProjectiles(Vector2 input)
    {
        if (!photonView.IsMine) yield break;
        Heal();
      /*  yield return new WaitForSeconds(shootDelay);

        int amount = 8;
        float duration = 2f;
        float interval = duration / amount;

        float inputMagnitude = Mathf.Clamp01(input.magnitude);
        Vector3 dir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 inputPosition = _transform.position + dir * inputMagnitude * targetingRange;

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-ultiRange / 2f, ultiRange / 2f),
                10f,
                Random.Range(-ultiRange / 2f, ultiRange / 2f)
            );

            Vector3 spawnPosition = inputPosition + randomOffset;
            ObjectPooler.Instance.SpawnProjectile(spawnPosition, Quaternion.identity); 

            yield return new WaitForSeconds(interval);
        }*/
    }

    void Heal()
    {
        PhotonView view = GetComponent<PhotonView>();
        if (view != null)
        {
            view.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, healAmount);
        }
    }
}