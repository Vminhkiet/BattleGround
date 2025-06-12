using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;          // The thing to follow (your Player)
    public Vector3 offset = new Vector3(0f, 10f, 0f); // Top-down, so Y is elevated
    public float followSpeed = 5f;    // Tweak for faster/slower follow

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera has no target to follow. Much like your life, it's directionless.");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Lock rotation to top-down (optional)
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
