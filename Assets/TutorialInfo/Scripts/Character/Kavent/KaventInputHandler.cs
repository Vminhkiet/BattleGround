using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
public class KaventInputHandler : APlayerInputHandler
{

    private ICharacterSkill characterSkill;
    private Movement movementComponent;

    [Header("Attack Combo Settings")]
    [SerializeField] private float comboResetTime = 2.0f;
    private Coroutine comboResetCoroutine;

    private void Awake()
    {
        //characterSkill
        base.Awake();
        characterSkill = GetComponent<KaventScript>();
        movementComponent = GetComponent<KaventMovement>();
    }

    public override void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 currentMoveInputValue = callbackContext.ReadValue<Vector2>();
        if (GetIsAttacking())
        {
            SetMoveInputInternal(Vector2.zero);
        }
        else
        {
            SetMoveInputInternal(currentMoveInputValue);
        }

        if (movementComponent != null)
        {
            movementComponent.SetInputLeft(GetInputLeft());
        }

    }

    public override void OnAttack(InputAction.CallbackContext context)
    {
        SetRightStickInputInternal(context.ReadValue<Vector2>());
        if (context.performed)
        {
            lastRightStickMagnitude = GetInputRight().magnitude;
        }
        else if (context.canceled)
        {
            bool canNewAttack = !GetIsAttacking() && (lastRightStickMagnitude - attackThreshold >= 0);
            bool canContinueCombo = GetIsAttacking() && GetAttackPhase() < 3 && (Time.time - _lastAttackActivatedTime <= comboResetTime);

            if(canNewAttack || canContinueCombo)
            {
                if (canNewAttack)
                {
                    SetAttackPhase(1);
                }
                else if (GetAttackPhase() == 3 || (Time.time - _lastAttackActivatedTime > comboResetTime && GetIsAttacking()))
                {
                    SetAttackPhase(1);
                }
                else
                {
                    SetAttackPhase(GetAttackPhase() + 1);
                }

                SetIsAttacking(true);
                _lastAttackActivatedTime = Time.time;

                if(comboResetCoroutine != null)
                {
                    StopCoroutine(comboResetCoroutine);
                }

                comboResetCoroutine = StartCoroutine(ComboResetTimer());
            }

            if (characterSkill != null)
            {
                characterSkill.NormalAttack(GetInputRight());
            }

        }
    }

    private IEnumerator ComboResetTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        ResetAttackState();
    }

    public void ResetAttackState()
    {
        SetIsAttacking(false);
        SetAttackPhase(0);

        if(comboResetCoroutine != null)
        {
            StopCoroutine(comboResetCoroutine);
            comboResetCoroutine = null;
        }
    }
}

