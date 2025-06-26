using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public abstract class APlayerInputHandler : MonoBehaviour
{
    protected Vector2 input;
    protected Vector2 rightStickInput;
    protected Vector2 ultiInput;
    protected Vector2 spellInput;


    protected bool isAttacking = false;
    protected bool isUlti = false;
    protected bool isSpell = false;
    protected int attackPhase = 0;
    protected float lastRightStickMagnitude;
    protected float lastUltiStickMagnitude;
    protected float lastSpellStickMagnitude;
    protected float attackThreshold = 0.05f;

    public event Action<bool> OnAttackStateChanged;
    public event Action<int> OnAttackPhaseChanged;
    public event Action<float> OnMoveInputChanged;
    public event Action OnSpellChanged;
    public event Action OnUltiChanged;
    

    public abstract void OnMove(InputAction.CallbackContext callbackContext);
    public abstract void OnAttack(InputAction.CallbackContext context);
    public abstract void OnSkill(InputAction.CallbackContext context);

    public abstract void OnSpell(InputAction.CallbackContext context);

    public void SetIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
        OnAttackStateChanged?.Invoke(this.isAttacking);
    }

    public void SetIsUlti(bool isUlti)
    {
        this.isUlti = isUlti;
        if (this.isUlti)
        {
            OnUltiChanged?.Invoke();
        }
    }

    public void SetIsSpell(bool isSpell)
    {
        this.isSpell = isSpell;
        if (this.isSpell)
        {
            OnSpellChanged?.Invoke();
        }
    }

    public void SetAttackPhase(int attackPhase)
    {
        this.attackPhase = attackPhase;
        Debug.Log(attackPhase);
        OnAttackPhaseChanged?.Invoke(this.attackPhase);
    }

    protected void SetMoveInputInternal(Vector2 value)
    {
        input = value;
        OnMoveInputChanged?.Invoke(input.magnitude);
    }
    protected void SetRightStickInputInternal(Vector2 value)
    {
        rightStickInput = value;
    }

    protected void SetUltiInputInternal(Vector2 value)
    {
        ultiInput = value;
    }

    protected void SetSpellInputInternal(Vector2 value)
    {
        spellInput = value;
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    public int GetAttackPhase()
    {
        return attackPhase;
    }

    public bool GetIsUlti()
    {
        return isUlti;
    }


    public Vector2 GetInputLeft()
    {
        return input;
    }

    public Vector2 GetInputRight()
    {
        return rightStickInput;
    }

    public Vector2 GetInputUlti()
    {
        return ultiInput;
    }

    public Vector2 GetInputSpell()
    {
        return spellInput;
    }

}
