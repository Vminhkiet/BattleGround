using UnityEngine;

public class HexCameraController : MonoBehaviour
{
    public Transform cam; // Assign MainCamera here
    public float rotationSpeed = 100f;
    public float zoomSpeed = 10f;
    public float minZoom = 10f;
    public float maxZoom = 80f;
    public float moveSpeed = 10f;

    void Update()
    {
        HandleZoom();
        HandleRotation();
        HandleMovement();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = cam.forward; // direction the camera is looking

        cam.position += zoomDirection * scroll * zoomSpeed;

        // Option B: move camera up/down (for orthographic)
        // cam.localPosition = new Vector3(cam.localPosition.x, zoomLevel, cam.localPosition.z);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.Rotate(-rotationY, rotationX, 0f);
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, z).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
    }
}