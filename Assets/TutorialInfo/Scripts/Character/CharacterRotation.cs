using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    public float rotationSpeed = 5f; // Tốc độ xoay

    private Vector3 previousMousePosition;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Khi nhấn chuột trái
        {
            isDragging = true;
            previousMousePosition = Input.mousePosition; // Lưu vị trí chuột ban đầu
        }

        if (Input.GetMouseButtonUp(0)) // Khi thả chuột trái
        {
            isDragging = false;
        }

        if (isDragging) // Nếu đang kéo giữ chuột
        {
            Vector3 deltaMouse = Input.mousePosition - previousMousePosition; // Tính toán sự thay đổi vị trí chuột

            // Xoay chỉ theo chiều ngang (trục Y)
            float horizontalRotation = deltaMouse.x * rotationSpeed * Time.deltaTime;

            // Xoay đối tượng quanh trục Y (xoay trên trục ngang)
            transform.RotateAround(transform.position, Vector3.up, horizontalRotation);

            previousMousePosition = Input.mousePosition; // Cập nhật vị trí chuột hiện tại
        }
    }
}
