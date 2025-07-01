using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
class MageInputHandler : APlayerInputHandler
{
    private LineRenderer lineRenderer;

    protected void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.enabled = false;
    }

    public override void OnAttack(InputAction.CallbackContext context)
    {     
            base.OnAttack(context);
            if (context.canceled)
                ResetInputRight();
    }

    protected override void OnSkillPerformed(Vector2 input)
    {
        if (input != Vector2.zero && !lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }
        else if(input == Vector2.zero && lineRenderer.enabled) {
            lineRenderer.enabled = false;
        }  
    }
}