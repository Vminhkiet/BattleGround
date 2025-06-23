using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaventAnimationController : PlayerAnimationController
{
    private APlayerInputHandler _inputhandler;

    private void Awake()
    {
        base.Awake();
        _inputhandler = GetComponent<KaventInputHandler>();

        _inputhandler.OnAttackPhaseChanged += HandleAttackPhaseChanged;
        _inputhandler.OnAttackStateChanged += HandleAttackStateChanged;
        _inputhandler.OnMoveInputChanged += HanldeMovementChanged;
    }

    private void Update()
    {
        if (animator != null)
        {
            if (speed < 0.5) return;
            float sinInput = Time.time * 0.5f * Mathf.PI * 4f;
            float rawSinValue = Mathf.Sin(sinInput);
            animator.SetFloat("typerun", rawSinValue);
        }
    }

    private void HandleAttackPhaseChanged(int newPhase)
    {
        SetAnimatorParameters(_inputhandler.GetIsAttacking(), newPhase);
    }

    private void HandleAttackStateChanged(bool newIsAttackingState)
    {
        SetAnimatorParameters(newIsAttackingState, _inputhandler.GetAttackPhase());
    }

    private void HanldeMovementChanged(float newSpeed)
    {
        SetAnimatorMovement(newSpeed);
    }
}

