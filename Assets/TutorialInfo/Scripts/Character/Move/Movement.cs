using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour, IMovable
{
    public float walkSpeed = 2f;
    public float maxVelocityChange = 10f;

    protected Vector2 input = Vector2.zero;
    private Rigidbody rb;

    private float speedMultiplier = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetInputLeft(Vector2 inputleft)
    {
        input = inputleft.normalized;
    }

    public void Move()
    {
        rb.AddForce(CalculateMovement(walkSpeed * speedMultiplier), ForceMode.VelocityChange);
    }

    public void ApplySpeedMultiplier(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        StartCoroutine(ResetSpeedAfter(duration));
    }

    IEnumerator ResetSpeedAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

    public void ResetSpeed()
    {
        speedMultiplier = 1f;
    }

    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y) * _speed;
        Vector3 velocity = rb.velocity;
        velocity.y = 0;

        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        return velocityChange;
    }
}