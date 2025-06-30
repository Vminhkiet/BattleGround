using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour
{
    private PlayerStats _pStats;

    public float attackInterval = 1f;

    public string[] targetTags;

    private System.Collections.Generic.Dictionary<GameObject, float> lastDamageTime =
        new System.Collections.Generic.Dictionary<GameObject, float>();

    private bool canDealDamage = false;

    public UnityEvent<float> onDamageDealt;

    void Awake()
    {
        if (onDamageDealt == null)
        {
            onDamageDealt = new UnityEvent<float>();
        }

        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("Collider on HitBox (" + gameObject.name + ") is not set to 'Is Trigger'. OnTriggerStay will not work correctly.", this);
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody is missing on HitBox (" + gameObject.name + "). Collision detection might not work correctly.", this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!canDealDamage)
        {
            return;
        }

        bool isTargetValid = false;
        foreach (string tag in targetTags)
        {
            if (other.CompareTag(tag))
            {
                isTargetValid = true;
                break;
            }
        }

        if (!isTargetValid)
        {
            return;
        }

        float currentTime = Time.time;
        float lastTimeHitThisTarget;

        if (lastDamageTime.TryGetValue(other.gameObject, out lastTimeHitThisTarget))
        {
            if (currentTime < lastTimeHitThisTarget + attackInterval)
            {
                return;
            }
        }
        else
        {
            lastTimeHitThisTarget = -attackInterval;
        }

        if (other.TryGetComponent(out PlayerHealthUI targetHealth))
        {

            targetHealth.TakeDamage(_pStats.GetDamage());

            lastDamageTime[other.gameObject] = currentTime;

            onDamageDealt.Invoke(_pStats.GetDamage());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (lastDamageTime.ContainsKey(other.gameObject))
        {
            lastDamageTime.Remove(other.gameObject);
        }
    }
    public void SetHitBoxActive(bool active)
    {
        canDealDamage = active;
    }

}
