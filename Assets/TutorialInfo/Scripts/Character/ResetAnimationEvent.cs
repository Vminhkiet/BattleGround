using System;
using UnityEngine;
public class ResetAnimationEvent : MonoBehaviour
{
    public event Action OnIsAttackingChanged;
    public event Action OnIsUltiChanged;

    public void ResetIsAttacking()
    {
        OnIsAttackingChanged?.Invoke();
    }
    public void ResetIsUlti()
    {
        OnIsUltiChanged?.Invoke();
    }
}
