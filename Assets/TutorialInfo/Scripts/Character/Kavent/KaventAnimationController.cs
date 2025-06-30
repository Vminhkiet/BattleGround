using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Photon.Pun;

public class KaventAnimationController : PlayerAnimationController
{
    private APlayerInputHandler _inputhandler;
    
    private void OnEnable()
    {
        _inputhandler = GetComponentInParent<APlayerInputHandler>();
        if (_inputhandler == null)
        {
            this.enabled = false;
            return;
        }

        _inputhandler.OnAttackPhaseChanged += HandleAttackPhaseChanged;
        _inputhandler.OnAttackStateChanged += HandleAttackStateChanged;
        _inputhandler.OnMoveInputChanged += HanldeMovementChanged;
        _inputhandler.OnUltiChanged += HanldeUltiChanged;
        _inputhandler.OnSpellChanged += HanldSpellChanged;
    }
    private void OnDisable()
    {
        if (_inputhandler != null)
        {
            _inputhandler.OnAttackPhaseChanged -= HandleAttackPhaseChanged;
            _inputhandler.OnAttackStateChanged -= HandleAttackStateChanged;
            _inputhandler.OnMoveInputChanged -= HanldeMovementChanged;
            _inputhandler.OnUltiChanged -= HanldeUltiChanged;
            _inputhandler.OnSpellChanged -= HanldSpellChanged;
        }
    }

    private void Update()
    {
        if (parentPhotonView != null && parentPhotonView.IsMine)
        {
            if (animator != null)
            {
                if (speed < 0.5) return;
                float sinInput = Time.time * 0.5f * Mathf.PI * 4f;
                float rawSinValue = Mathf.Sin(sinInput);
                animator.SetFloat("typerun", rawSinValue);
            }
        }
    }


    private void HandleAttackPhaseChanged(int newPhase)
    {
        if (parentPhotonView != null && parentPhotonView.IsMine)
        {
            SetAttackPhase(newPhase);
        }
    }

    private void HandleAttackStateChanged(bool newIsAttackingState)
    {
        if (parentPhotonView != null && parentPhotonView.IsMine)
        {
            SetIsAttacking(newIsAttackingState);
        }
    }

    private void HanldeMovementChanged(float newSpeed)
    {
        if (parentPhotonView != null && parentPhotonView.IsMine)
        {
            SetAnimatorMovement(newSpeed);
        }
    }

    private void HanldeUltiChanged()
    {
        if (parentPhotonView != null && parentPhotonView.IsMine)
        {
            SetUtil();
        }
    }

    private void HanldSpellChanged()
    {
        if (parentPhotonView != null && parentPhotonView.IsMine)
        {
            SetSpell();
        }
    }
}

