using System;
using System.Collections.Generic;
using UnityEngine;


public class RotationEffect : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public void RotateEffectSlash(Vector2 input)
    {
        if (input.magnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
