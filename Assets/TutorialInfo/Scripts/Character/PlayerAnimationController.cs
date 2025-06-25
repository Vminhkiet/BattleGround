using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private float stayTime;
    private bool isAttacking;
    private bool isMoving;
    private bool useSkill;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("UseSkill", useSkill);

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        bool isInAttackState = currentState.IsName("Attack");

        if (isInAttackState)
        {
            stayTime += Time.deltaTime;
            if (stayTime >= 3f && isAttacking)
            {
                animator.SetBool("StayTimeExceeded", true);
            }
            if (currentState.normalizedTime >= 1.0f)
            {
                isAttacking = false;
                stayTime = 0f;
                animator.SetBool("StayTimeExceeded", false);
            }
        }
        else
        {
            stayTime = 0f;
            if (isAttacking)
            {
                isAttacking = false;
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpecialSkill") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            useSkill = false;
        }
    }

    public void SetMovementState(bool moving) => isMoving = moving;
    public void SetAttackState(bool attacking) => isAttacking = attacking;
    public void SetSkillState(bool skill) => useSkill = skill;
}
