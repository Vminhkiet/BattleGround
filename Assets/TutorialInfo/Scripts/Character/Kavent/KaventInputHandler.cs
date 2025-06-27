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
            rotationEffect.RotateEffectSlash(lastValidRightStickInput);
        }
        else if (context.canceled)
        {
            predictSlash.SetActive(false);
        }
        base.OnAttack(context);
    }

}

