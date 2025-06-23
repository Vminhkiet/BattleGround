using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimationController : MonoBehaviour
{
    protected Animator animator;
    protected float speed = 0;

    protected virtual void Awake()
    {

        animator = GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("DONT HAVE ANIMATOR");
        }
    }

    public void SetAnimatorParameters(bool isAttackingState, int attackPhaseValue)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
        animator.SetBool("IsAttacking", isAttackingState);
        animator.SetInteger("AttackPhase", attackPhaseValue);
    }

    public void SetAnimatorMovement(float speed)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
        animator.SetFloat("speed", speed);
        this.speed = speed;
    }
}
