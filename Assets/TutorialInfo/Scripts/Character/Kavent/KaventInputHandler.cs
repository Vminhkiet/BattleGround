using System;
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
    private EffectSlashManager effectSlashManager;
    protected override void Awake()
    {
        base.Awake();
        rotationEffect = predictSlash.GetComponent<RotationEffect>();
        effectSlashManager = GetComponent<EffectSlashManager>();

    }

    public override void OnAttack(InputAction.CallbackContext context)
    {
        base.OnAttack(context);
        if (context.canceled)
            ResetInputRight();
    }

    

}

