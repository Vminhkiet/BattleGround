using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float maxVelocityChange = 10f;
    public float rotationSpeed = 10f;
    public float attackThreshold = 0.3f;

    private Vector2 input;
    private Vector2 rightStickInput;
    private float lastRightStickMagnitude;
    private Rigidbody rb;
    private bool isAttacking; // Tr?ng thái t?n công (cho combo Attack)
    private bool useSkill;    // Tr?ng thái tung k? n?ng

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        input.Normalize();
        RotateCharacter();
        UpdateAnimationState();
    }

    // Input x? lý di chuy?n
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        input = callbackContext.ReadValue<Vector2>();
    }

    // Input x? lý k? n?ng
    public void OnSkill(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && !isAttacking) // Ch? tung k? n?ng khi không trong combo t?n công
        {
            useSkill = true;
            TriggerSkill(); // X? lý logic k? n?ng
        }
        else
        {
            useSkill = false;
        }
    }

    // Input x? lý t?n công (cho combo Attack)
    public void OnAttack(InputAction.CallbackContext context)
    {
        rightStickInput = context.ReadValue<Vector2>();
        if (context.performed)
        {
            lastRightStickMagnitude = rightStickInput.magnitude;
        }
        else if (context.canceled) 
        {
            if (lastRightStickMagnitude > attackThreshold && !isAttacking)
            {
                isAttacking = true;
                UpdateAnimationState();
            }
            rightStickInput = Vector2.zero;
        }
    }

    void RotateCharacter()
    {
        if (input.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(CalculateMovement(walkSpeed), ForceMode.VelocityChange);
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

    private void TriggerSkill()
    {
        // Logic x? lý k? n?ng: ví d? gây sát th??ng, spawn VFX
        Debug.Log("Tung k? n?ng ??c bi?t!");
        Collider[] enemies = Physics.OverlapSphere(transform.position, 5f); // Bán kính 5
        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Gây sát th??ng cho: " + enemy.name);
            }
        }
    }

    private void UpdateAnimationState()
    {
        PlayerAnimationController animationController = GetComponent<PlayerAnimationController>();
        if (animationController != null)
        {
            animationController.SetMovementState(input.magnitude > 0.1f);
            animationController.SetAttackState(isAttacking);
            animationController.SetSkillState(useSkill);
        }

        isAttacking = false;
    }
}