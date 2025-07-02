using UnityEngine;
using System.Collections.Generic;

public class SlowZone : MonoBehaviour, ISkillEffect
{
    public float slowMultiplier = 0.2f;
    private GameObject caster;

    private Dictionary<IMovable, float> originalSpeeds = new();

    public void SetCaster(GameObject casterObj)
    {
        caster = casterObj;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == caster) return;

        IMovable movable = other.GetComponent<IMovable>();
        if (movable != null)
        {
            movable.ApplySpeedMultiplier(slowMultiplier, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IMovable movable = other.GetComponent<IMovable>();
        if (movable != null)
        {
            movable.ResetSpeed();
        }
    }
}
