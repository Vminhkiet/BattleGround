﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    protected Vector2 lastValidRightStickInput;
    protected Vector2 lastValidUltiStickInput;
    protected Vector2 lastValidSpellStickInput;
    protected bool nextAttack = false;
    protected Vector2 nextAttackinput;

    protected ICharacterSkill characterSkill;
    protected IMovable movementComponent;
    protected PlayerStats playerStats;
    protected CharacterMeshRotation cRotation;
    protected IEffectPlayer effectPlayer;
    protected INetworkOwnership _networkOwnership;
    protected INetworkTransform _networkTransform;
    protected ResetAnimationEvent resetAnimationEvent;
    [SerializeField] private AttackChargeSystem attackChargeSystem;
    protected UltiChargeManager _ultiChargeManager;

    public event Action<float> OnMoveInputChanged;
    public event Action<bool> OnAttackStateChanged;
    public event Action<int> OnAttackPhaseChanged;
    public event Action OnSpellChanged;
    public event Action OnUltiChanged;
    public static event Action<Vector2> OnCharacterRotateChanged;

    private bool isStun = false;
    private bool isSlow = false;

    public void Initialize(IEffectPlayer effectPlayerInstance, 
                            ICharacterSkill characterSkillInstance,
                            IMovable movementComponentInstance, 
                            PlayerStats playerStatsInstance, 
                            CharacterMeshRotation cRotationInstance,
                            INetworkOwnership networkOwnership,
                            INetworkTransform networkTransform,
                            UltiChargeManager _ultiChargeManager,
                            ResetAnimationEvent resetAnimationEvent
                            )
    {
        this.effectPlayer = effectPlayerInstance;
        this.characterSkill = characterSkillInstance;
        this.movementComponent = movementComponentInstance;
        this.playerStats = playerStatsInstance;
        this.cRotation = cRotationInstance;
        this._networkOwnership = networkOwnership;
        this._networkTransform = networkTransform;
        this._ultiChargeManager = _ultiChargeManager;
        this.resetAnimationEvent = resetAnimationEvent;

        this._ultiChargeManager.Init();
        this.characterSkill.Init();
        this.characterSkill.SetEffectSkill(this.effectPlayer);
        this.characterSkill.SetNetworkOwnership(this._networkOwnership);
        if (this.resetAnimationEvent != null)
        {
            this.resetAnimationEvent.OnIsAttackingChanged -= ResetAttackState;
            this.resetAnimationEvent.OnIsUltiChanged -= ResetUltiState;
            this.resetAnimationEvent.OnIsAttackingChanged += ResetAttackState;
            this.resetAnimationEvent.OnIsUltiChanged += ResetUltiState;
        }
    }

    private void OnDisable()
    {
        if (this.resetAnimationEvent != null)
        {
            this.resetAnimationEvent.OnIsAttackingChanged -= ResetAttackState;
            this.resetAnimationEvent.OnIsUltiChanged -= ResetUltiState;
        }
    }


    protected virtual void Update()
    {
        if (_networkOwnership == null || !_networkOwnership.IsLocalPlayer)
        {
            return;
        }
        characterSkill.DrawUltiPosition(GetInputUlti());
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

    private void FixedUpdate()
    {
        if (_networkOwnership == null || !_networkOwnership.IsLocalPlayer)
        {
            return;
        }
        movementComponent.Move();
    }

    public virtual void OnMove(InputAction.CallbackContext callbackContext)
    {
        if (_networkOwnership == null || !_networkOwnership.IsLocalPlayer)
        {
            return;
        }

        Vector2 currentMoveInputValue = callbackContext.ReadValue<Vector2>();

        SetMoveInputInternal(currentMoveInputValue);

        if (callbackContext.performed)
        {
            if (movementComponent == null) return;
            if(!GetIsAttacking())
                cRotation.RotateTowardsDirection(GetInputLeft());
            movementComponent.SetInputLeft(GetInputLeft());
        }
        else if (callbackContext.canceled)
        {
            SetMoveInputInternal(Vector2.zero);
            movementComponent.SetInputLeft(GetInputLeft());
        }
    }

    public virtual void OnSkill(InputAction.CallbackContext context)
    {
        if (_networkOwnership == null || !_networkOwnership.IsLocalPlayer)
        {
            return;
        }

        SetUltiInputInternal(context.ReadValue<Vector2>());
        if (context.performed)
        {
            lastValidUltiStickInput = GetInputUlti();
            lastUltiStickMagnitude = lastValidUltiStickInput.magnitude;
        }
        else if (context.canceled)
        {
            bool canUlti = !GetIsAttacking() && lastUltiStickMagnitude - attackThreshold > 0 && _ultiChargeManager.IsUltiFull();

            if (canUlti)
            {
                SetIsUlti(true);

                if (characterSkill != null)
                    characterSkill.UseSkill(lastValidUltiStickInput);
                _ultiChargeManager.ResetUlti();
            }

            ResetInputUlti();

        }
    }

    public virtual void OnSpell(InputAction.CallbackContext context)
    {
        if (_networkOwnership == null || !_networkOwnership.IsLocalPlayer)
        {
            return;
        }

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


    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if (_networkOwnership == null || !_networkOwnership.IsLocalPlayer)
        {
            return;
        }

        SetRightStickInputInternal(context.ReadValue<Vector2>());
        if (context.performed)
        {
            lastValidRightStickInput = GetInputRight();
            lastRightStickMagnitude = lastValidRightStickInput.magnitude;
            OnSkillPerformed(lastValidRightStickInput);
        }
        else if (context.canceled)
        {
            bool canNewAttack = !GetIsAttacking() && (lastRightStickMagnitude - attackThreshold >= 0) && !GetIsUlti() ;
            OnSkillPerformed(Vector2.zero);

            if (canNewAttack)
            {
                SetAttackPhase(GetAttackPhase() % 3 + 1);

                SetIsAttacking(true);


                if (characterSkill != null)
                {
                    cRotation.RotateTowardsDirection(lastValidRightStickInput);
                    characterSkill.NormalAttack(lastValidRightStickInput);
                }

            }
            else if (GetIsAttacking() && (lastRightStickMagnitude - attackThreshold >= 0))
            {
                nextAttackinput = lastValidRightStickInput;
                nextAttack = true;
            }

        }
    }


    public void invokeRotationCharacter(Vector2 input)
    {
        OnCharacterRotateChanged?.Invoke(input);
    }

    public void SetIsSlow(bool isSlow)
    {
        this.isSlow = isSlow;
    }



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

    public bool GetIsStun()
    {
        return isStun;
    }

    public bool GetIsSlow()
    {
        return isSlow;
    }

    public virtual void ResetAttackState()
    {
        SetIsAttacking(false);
    }

    public virtual void ResetUltiState()
    {
        SetIsUlti(false);
    }

    public virtual void ResetSpellState()
    {
        SetIsSpell(false);
    }

    public void ResetInputRight()
    {
        SetRightStickInputInternal(Vector2.zero);
        lastRightStickMagnitude = 0f;
        lastValidRightStickInput = Vector2.zero;
    }

    public void ResetInputUlti()
    {
        SetUltiInputInternal(Vector2.zero);
        lastUltiStickMagnitude = 0f;
        lastValidUltiStickInput = Vector2.zero;
    }

    protected virtual void OnSkillPerformed(Vector2 input)
    {
        // Debug.Log("Skill performed input received.");
        // Lớp con có thể phát âm thanh "charge" hoặc bắt đầu hiệu ứng charging
    }

}
