using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;

    private Animator animator;
    private Camera mainCamera;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        if (animator == null)
            Debug.LogWarning("Animator không tồn tại. Chắc bạn muốn nhân vật nhảy moonwalk.");
    }

    void FixedUpdate()
    {
        // Lấy hướng input từ joystick
        Vector3 input = new Vector3(variableJoystick.Horizontal, 0f, variableJoystick.Vertical);

        // Tính hướng theo camera
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.z + camRight * input.x;
        moveDir = moveDir.normalized * speed;

        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        bool isMoving = input.magnitude > 0.1f;
        if (animator != null)
            animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }
}
