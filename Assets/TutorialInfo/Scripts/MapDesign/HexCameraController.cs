using UnityEngine;

/// <summary>
/// A camera controller that mimics the behavior of the Unity Scene View camera.
/// It supports orbiting, panning, zooming, and flying.
/// This script can be placed on a dedicated "CameraController" object in your scene.
/// </summary>
public class HexCameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The object to orbit around. If null, will use a dynamic pivot point.")]
    public Transform target; 

    [Header("Speeds")]
    public float panSpeed = 1f;
    public float orbitSpeed = 150f;
    public float flySpeed = 10f;
    public float zoomSpeed = 10f;

    [Header("Zoom Settings")]
    public float minDistance = 0.1f;
    public float maxDistance = 100f;

    [Header("Smoothing")]
    [Tooltip("How quickly the camera snaps to its target position and rotation.")]
    public float orbitDampening = 10f;

    // Private state
    private Vector3 offset;
    private Vector3 pivotPoint;
    private float distance;
    private Transform _cameraTransform;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        
        // Initialize the pivot and offset based on an optional target or the camera's current view
        if (target != null)
        {
            pivotPoint = target.position;
            offset = _cameraTransform.position - pivotPoint;
        }
        else
        {
            // If no target is set, create a pivot point in front of the camera to start
            pivotPoint = _cameraTransform.position + _cameraTransform.forward * 10f;
            offset = _cameraTransform.position - pivotPoint;
        }
        distance = offset.magnitude;
    }

    void LateUpdate()
    {
        // Switch between Fly and Orbit modes
        if (Input.GetMouseButton(1))
        {
            HandleFlyCamera();
        }
        else
        {
            HandleOrbitCamera();
        }
    }

    /// <summary>
    /// Handles first-person flying behavior.
    /// </summary>
    void HandleFlyCamera()
    {
        // Rotation (Look around)
        float mouseX = Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;
        
        // Rotate yaw based on world up axis, and pitch based on camera's local right axis
        _cameraTransform.Rotate(Vector3.up, mouseX, Space.World);
        _cameraTransform.Rotate(Vector3.right, -mouseY, Space.Self);
        
        // Movement (WASDQE)
        float forward = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        float right = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float up = (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);

        Vector3 moveDir = new Vector3(right, up, forward).normalized;
        _cameraTransform.position += _cameraTransform.TransformDirection(moveDir) * flySpeed * Time.deltaTime;

        // Update the pivot point for a seamless transition back to orbit mode
        pivotPoint = _cameraTransform.position + _cameraTransform.forward * distance;
        offset = _cameraTransform.position - pivotPoint;
    }

    /// <summary>
    /// Handles orbiting, panning, and zooming around a pivot point.
    /// </summary>
    void HandleOrbitCamera()
    {
        // Orbit (Alt + Left Mouse Button)
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.X))
        {
            float mouseX = Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;

            // Rotate the offset vector around the pivot
            Quaternion rotation = Quaternion.Euler(0, mouseX, 0);
            offset = rotation * offset;
            
            rotation = Quaternion.AngleAxis(-mouseY, _cameraTransform.right);
            offset = rotation * offset;
        }

        // Pan (Middle Mouse Button)
        if (Input.GetMouseButton(2))
        {
            float mouseX = -Input.GetAxis("Mouse X") * panSpeed * 0.05f;
            float mouseY = -Input.GetAxis("Mouse Y") * panSpeed * 0.05f;

            // Scale pan speed with distance for more intuitive movement
            mouseX *= Mathf.Sqrt(distance);
            mouseY *= Mathf.Sqrt(distance);

            Vector3 pan = _cameraTransform.right * mouseX + _cameraTransform.up * mouseY;
            pivotPoint += pan;
        }

        // Zoom (Mouse Scroll Wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
        }
        
        // Zoom (Alt + Right Mouse Button)
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
        {
            float delta = Input.GetAxis("Mouse Y"); // Use vertical mouse movement for smooth zoom
            distance -= delta * zoomSpeed * 0.1f;
        }

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Apply final position and rotation with smoothing
        Vector3 targetPosition = pivotPoint + offset.normalized * distance;
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, Time.deltaTime * orbitDampening);
        
        Quaternion targetRotation = Quaternion.LookRotation(pivotPoint - _cameraTransform.position);
        _cameraTransform.rotation = Quaternion.Slerp(_cameraTransform.rotation, targetRotation, Time.deltaTime * orbitDampening);
    }
}