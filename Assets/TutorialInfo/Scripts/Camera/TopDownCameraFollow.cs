using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -6f); // camera đứng sau và trên nhân vật
    public float followSpeed = 5f;
    public Vector3 rotationOffset = new Vector3(20f, 0f, 0f); // Góc xoay cố định

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera không có target. Giống như bạn lúc thi giữa kỳ.");
            return;
        }

        // Giữ góc nhìn cố định
        Quaternion fixedRotation = Quaternion.Euler(rotationOffset);
        Vector3 desiredPosition = target.position + fixedRotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.rotation = fixedRotation;
    }
}
