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

    private ICharacterSkill characterSkill;
    private Movement movementComponent;
    private RotationEffect rotationEffect;
    private Vector2 lastValidRightStickInput;
    private Vector2 lastValidUltiStickInput;
    private Vector2 lastValidSpellStickInput;
    private bool nextAttack = false;
    private Vector2 nextAttackinput;
    private PlayerStats playerStats;

    private void Awake()
    {
        rotationEffect = predictSlash.GetComponent<RotationEffect>();
        playerStats = GetComponent<PlayerStats>();
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
                    characterSkill.NormalAttack(nextAttackinput);
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

    public override void OnSkill(InputAction.CallbackContext context)
    {
        if (!playerStats.isEnergyFull())
            return;

        SetUltiInputInternal(context.ReadValue<Vector2>());
        if (context.performed)
        {
            lastValidUltiStickInput = GetInputUlti();
            lastUltiStickMagnitude = lastValidUltiStickInput.magnitude;
        }
        else if (context.canceled)
        {
            bool canUlti = !GetIsAttacking() && lastUltiStickMagnitude - attackThreshold > 0;

            if(canUlti)
            {
                SetIsUlti(true);

                if(characterSkill != null)
                    characterSkill.UseSkill(lastValidUltiStickInput);
            }

            SetUltiInputInternal(Vector2.zero);
            lastUltiStickMagnitude = 0f;
            lastValidUltiStickInput = Vector2.zero;

        }
    }

    public override void OnSpell(InputAction.CallbackContext context)
    {
        if (!playerStats.isSpellFull())
            return;

        SetSpellInputInternal(context.ReadValue<Vector2>());
        if (context.performed)
        {
            lastValidSpellStickInput = GetInputSpell();
            lastSpellStickMagnitude = lastValidSpellStickInput.magnitude;
        }
        else if (context.canceled)
        {
            bool canSpell = lastUltiStickMagnitude - attackThreshold > 0;

            if (canSpell)
            {
                SetIsSpell(true);
                if (characterSkill != null)
                    characterSkill.UseSpell(lastValidSpellStickInput);
            }

            SetSpellInputInternal(Vector2.zero);
            lastSpellStickMagnitude = 0f;
            lastValidSpellStickInput = Vector2.zero;

        }
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

    public void ResetAttackState()
    {
        SetIsAttacking(false);
    }

    public void ResetUltiState()
    {
        SetIsUlti(false);
    }

    public void ResetSpellState()
    {
        SetIsSpell(false);
    }
}

