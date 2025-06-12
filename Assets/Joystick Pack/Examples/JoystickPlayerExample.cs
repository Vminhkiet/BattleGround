using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed = 5f;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;

    void FixedUpdate()
    {
        Vector3 input = new Vector3(variableJoystick.Horizontal, 0f, variableJoystick.Vertical);
        Vector3 movement = input.normalized * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z); // preserve Y velocity (gravity, jumping)
    }
}
