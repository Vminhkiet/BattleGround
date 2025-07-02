using DG.Tweening;
using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HomingCube : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public float homingDelay = 0.2f;

    private Transform target;
    private float lifeTimer;
    private float delayTimer;
    public bool isHoming = true;
    public Transform player;
    private GameObject caster;

    public UnityEvent<float> onDamageDealt;

    private float damage;
    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }
    public void SetCaster(GameObject caster)
    {
        this.caster = caster;
    }
    public void SetPlayerStatsDamage(float damage)
    {
        this.damage = damage;
    }

    void OnEnable()
    {
        if (onDamageDealt == null)
            onDamageDealt = new UnityEvent<float>();
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
            transform.position += Vector3.down * speed* 2 * Time.deltaTime;
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
            direction.y = 0;
            direction.Normalize();
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
            if ( other.CompareTag("DecorationObject") || other.CompareTag("Tile"))
            {
                ExplosionPooler.Instance.SpawnExplosion(transform.position);
                ReturnToPool();
            }
            if (other.transform == target)
            {
                ExplosionPooler.Instance.SpawnExplosion(transform.position);       
                if (other.TryGetComponent(out PlayerHealthUI targetHealth))
                {
                    PhotonView view = targetHealth.GetComponent<PhotonView>();
                    if (view != null)
                    {
                        view.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, this.damage);
                    }
                    UltiChargeManager ultiCharge = caster.GetComponent<UltiChargeManager>();
                    if (ultiCharge != null)
                    {
                        ultiCharge.AddUltiPoint(this.damage);
                    }

                    onDamageDealt.Invoke(this.damage);
                }
                ReturnToPool();
                return;
            }

        /*    if(other.tag.Equals("Player") && !other.gameObject.Equals(caster))
            {
                ExplosionPooler.Instance.SpawnExplosion(transform.position);
                if (other.TryGetComponent(out PlayerHealthUI targetHealth))
                {
                    PhotonView view = targetHealth.GetComponent<PhotonView>();
                    if (view != null)
                    {
                        view.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, this.damage);
                    }
                    UltiChargeManager ultiCharge = caster.GetComponent<UltiChargeManager>();
                    if (ultiCharge != null)
                    {
                        ultiCharge.AddUltiPoint(this.damage);
                    }

                    onDamageDealt.Invoke(this.damage);
                }
                ReturnToPool();
                return;
            }*/
    }

    void ReturnToPool()
    {
        isHoming = true;
        if(ObjectPooler.Instance==null)
        {
            Debug.Log("pool null");
        }    
        else
            ObjectPooler.Instance.DespawnProjectile(gameObject);
    }
}
