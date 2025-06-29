using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
class MageInputHandler : APlayerInputHandler
    {
        public override void OnAttack(InputAction.CallbackContext context)
        {
            base.OnAttack(context);
            if (context.canceled)
                ResetInputRight();
        }
    }
