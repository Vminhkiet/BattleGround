using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;
public class KaventInputHandler : APlayerInputHandler
{
    [SerializeField]
    private GameObject predictSlash;
    private RotationEffect rotationEffect;

    protected override void Awake()
    {
        base.Awake();
        rotationEffect = predictSlash.GetComponent<RotationEffect>();
    }

    public override void OnAttack(InputAction.CallbackContext context)
    {
        SetRightStickInputInternal(context.ReadValue<Vector2>());
        if (context.performed)
        {
            if (predictSlash != null && !predictSlash.activeSelf)
            {
                predictSlash.SetActive(true);
            }
            lastValidRightStickInput = GetInputRight();
            lastRightStickMagnitude = lastValidRightStickInput.magnitude;
            rotationEffect.RotateEffectSlash(lastValidRightStickInput);
        }
        else if (context.canceled)
        {
            predictSlash.SetActive(false);

            bool canNewAttack = !GetIsAttacking() && (lastRightStickMagnitude - attackThreshold >= 0) && !GetIsUlti();

            if(canNewAttack)
            {
                SetAttackPhase(GetAttackPhase() % 3 + 1);

                SetIsAttacking(true);


                if (characterSkill != null)
                {
                    characterSkill.NormalAttack(lastValidRightStickInput);
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

}

