using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class HitBox : MonoBehaviour
{
    private PlayerStats _pStats;
    private GameObject caster;
    private PhotonView ownerView;

    public float attackInterval = 1f;
    public string[] targetTags;

    private Dictionary<GameObject, float> lastDamageTime = new();
    private bool canDealDamage = false;

    public UnityEvent<float> onDamageDealt;

    void Awake()
    {
        if (onDamageDealt == null)
            onDamageDealt = new UnityEvent<float>();

        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            Debug.LogWarning("Collider should be set to IsTrigger", this);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogWarning("Rigidbody is missing. Hit detection may not work properly.", this);
    }

    public void SetCaster(GameObject casterObj)
    {
        caster = casterObj;
        ownerView = casterObj.GetComponent<PhotonView>();
    }

    public void SetPlayerStats(PlayerStats stats)
    {
        _pStats = stats;
    }

    public void SetHitBoxActive(bool active)
    {
        canDealDamage = active;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canDealDamage) return;

        if (other.transform.root.gameObject == caster) return;

        PhotonView targetView = other.GetComponent<PhotonView>();
        if (targetView != null && ownerView != null)
        {
            if (targetView.OwnerActorNr == ownerView.OwnerActorNr)
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
        if (!isTargetValid) return;

        float currentTime = Time.time;
        if (lastDamageTime.TryGetValue(other.gameObject, out float lastHitTime))
        {
            if (currentTime < lastHitTime + attackInterval)
                return;
        }

        if (other.TryGetComponent(out PlayerHealthUI targetHealth))
        {
            float dmg = _pStats.GetDamage();
            PhotonView view = targetHealth.GetComponent<PhotonView>();
            if (view != null)
            {
                view.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, dmg);
            }
            lastDamageTime[other.gameObject] = currentTime;

            onDamageDealt.Invoke(dmg);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (lastDamageTime.ContainsKey(other.gameObject))
            lastDamageTime.Remove(other.gameObject);
    }
}
