using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;
public class KaventInputHandler : APlayerInputHandler
{

    private ICharacterSkill characterSkill;
    private Movement movementComponent;
    private Vector2 lastValidRightStickInput;
    private bool nextAttack = false;
    private Vector2 nextAttackinput;

    private void Awake()
    {
        characterSkill = GetComponent<ICharacterSkill>();
        movementComponent = GetComponent<KaventMovement>();
    }

    private void Update()
    {
        if (nextAttack)
        {
            bool canNewAttack = !GetIsAttacking();

            if (canNewAttack)
            {
                SetAttackPhase(GetAttackPhase() % 3 + 1);

                SetIsAttacking(true);


                if (characterSkill != null)
                {
                    characterSkill.NormalAttack(GetInputRight());
                }

            }
            nextAttackinput = Vector2.zero;
            nextAttack = !nextAttack;
        }
    }

    public override void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 currentMoveInputValue = callbackContext.ReadValue<Vector2>();

        SetMoveInputInternal(currentMoveInputValue);


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
            lastValidRightStickInput = GetInputRight();
            lastRightStickMagnitude = lastValidRightStickInput.magnitude;
        }
        else if (context.canceled)
        {
            bool canNewAttack = !GetIsAttacking() && (lastRightStickMagnitude - attackThreshold >= 0);

            if(canNewAttack)
            {
                SetAttackPhase(GetAttackPhase() % 3 + 1);

                SetIsAttacking(true);


                if (characterSkill != null)
                {
                    characterSkill.NormalAttack(GetInputRight());
                }

            }
            else if (GetIsAttacking() && (lastRightStickMagnitude - attackThreshold >= 0))
            {
                nextAttackinput = lastValidRightStickInput;
                nextAttack = true;
            }

            SetRightStickInputInternal(Vector2.zero);
            lastRightStickMagnitude = 0f;
            lastValidRightStickInput = Vector2.zero;

        }
    }

    public void ResetAttackState()
    {
        SetIsAttacking(false);
    }
}

