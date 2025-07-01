using UnityEngine;
using System.Collections.Generic;

public class PullZone : MonoBehaviour, ISkillEffect
{
    public float pullForce = 1000f;
    private GameObject caster;

    private Dictionary<Rigidbody, Vector3> originalVelocities = new Dictionary<Rigidbody, Vector3>();

    public void SetCaster(GameObject caster)
    {
        this.caster = caster;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == caster) return;

        INetworkOwnership owner = other.GetComponent<INetworkOwnership>();
        if (owner != null && !owner.IsLocalPlayer) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !originalVelocities.ContainsKey(rb))
        {
            originalVelocities[rb] = rb.velocity;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == caster) return;

        INetworkOwnership owner = other.GetComponent<INetworkOwnership>();
        if (owner != null && !owner.IsLocalPlayer) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            rb.AddForce(direction * pullForce * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && originalVelocities.ContainsKey(rb))
        {
            rb.velocity = originalVelocities[rb];
            originalVelocities.Remove(rb);
        }
    }
}
