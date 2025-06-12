using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;
    public Joystick joystick; // drag your joystick here in the Inspector
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector2 input = joystick.Direction;
        Vector3 move = new Vector3(input.x, 0, input.y);

        rb.velocity = move * speed + new Vector3(0, rb.velocity.y, 0);

        RotateCharacter(move);
    }

    void RotateCharacter(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }
}
