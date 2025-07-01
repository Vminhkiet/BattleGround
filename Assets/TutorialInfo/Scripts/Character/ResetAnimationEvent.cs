using System;
using UnityEngine;
public class ResetAnimationEvent : MonoBehaviour
{
    public event Action OnIsAttackingChanged;

    public void ResetIsAttacking()
    {
        OnIsAttackingChanged?.Invoke();
    }
}
