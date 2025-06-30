using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimationController : MonoBehaviour
{
    protected PhotonView parentPhotonView;
    protected Animator animator;
    protected float speed = 0;

    protected virtual void Awake()
    {

        animator = GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("DONT HAVE ANIMATOR");
        }

        parentPhotonView = GetComponentInParent<PhotonView>();
        if (parentPhotonView == null)
        {
            Debug.LogError("PlayerAnimationController: PhotonView not found in parent!", this);
            this.enabled = false;
        }
    }

    public void SetIsAttacking(bool isAttackingState)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
        animator.SetBool("IsAttacking", isAttackingState);
    }
    public void SetStopIsAttacking()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
        animator.SetBool("IsAttacking", false);
        animator.SetInteger("AttackPhase", 1);
    }

    public void SetAttackPhase(int attackPhaseValue)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
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

    public void SetUtil()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
        animator.SetTrigger("Ulti");
    }
    public void SetSpell()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null when setting parameters in PlayerAnimationController!");
            return;
        }
        animator.SetTrigger("Spell");
    }
}
