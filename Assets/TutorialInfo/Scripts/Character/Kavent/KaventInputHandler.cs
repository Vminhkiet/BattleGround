﻿using System;
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
    private MeshRenderer meshRenderer;
    
    protected void Awake()
    {
        rotationEffect = predictSlash.GetComponent<RotationEffect>();
        meshRenderer = predictSlash.GetComponent<MeshRenderer>();
        predictSlash.SetActive(true);
        meshRenderer.enabled = false;
    }

    public override void OnAttack(InputAction.CallbackContext context)
    {
        base.OnAttack(context);
        if (context.canceled)
            ResetInputRight();
    }
    protected override void OnSkillPerformed(Vector2 input)
    {
        base.OnSkillPerformed(input);

        if (input == Vector2.zero)
        {
            meshRenderer.enabled = false;
            return;
        }
        if (!meshRenderer.enabled)
        {
            meshRenderer.enabled = true;
        }
        
        rotationEffect.RotateEffectSlash(input);
    }

}

