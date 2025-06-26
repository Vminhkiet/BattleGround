using UnityEngine;
using System.Collections;

public class BibiAttackTrailController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform characterPivot; // V? trí trung tâm c?a nhân v?t, n?i cung tròn s? xoay quanh
    public float attackRadius = 3f; // Bán kính c?a cung tròn
    public float attackAngle = 90f; // Góc c?a cung tròn (ví d? 90 ?? cho 1/4 vòng tròn)
    public float trailDuration = 0.2f; // Th?i gian v?t t?n t?i
    public int segments = 30; // S? l??ng ?o?n th?ng ?? t?o cung tròn (càng nhi?u càng m?n)

    private Coroutine drawTrailCoroutine;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // ??m b?o không có ?i?m nào ban ??u
        lineRenderer.enabled = false; // T?t Line Renderer ban ??u
    }

    // Hàm này s? ???c g?i t? Animation Event
    public void ActivateAttackTrail(float characterRotationY)
    {
        if (drawTrailCoroutine != null)
        {
            StopCoroutine(drawTrailCoroutine); // D?ng coroutine c? n?u có
        }
        drawTrailCoroutine = StartCoroutine(DrawArcTrail(characterRotationY));
    }

    IEnumerator DrawArcTrail(float characterRotationY)
    {
        lineRenderer.enabled = true; // B?t Line Renderer
        lineRenderer.positionCount = segments + 1; // S? ?i?m = s? ?o?n + 1

        // Tính toán góc b?t ??u và k?t thúc c?a cung tròn d?a trên h??ng c?a nhân v?t
        // characterRotationY là góc xoay Y c?a nhân v?t (ví d?: 0 ?? là phía tr??c, 90 ?? là bên ph?i)
        float startAngleRad = (characterRotationY - attackAngle / 2f) * Mathf.Deg2Rad;
        float endAngleRad = (characterRotationY + attackAngle / 2f) * Mathf.Deg2Rad;
        float angleStep = (endAngleRad - startAngleRad) / segments;

        // V? cung tròn
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngleRad + i * angleStep;
            float x = characterPivot.position.x + attackRadius * Mathf.Sin(currentAngle);
            float z = characterPivot.position.z + attackRadius * Mathf.Cos(currentAngle);
            // Gi? s? game nhìn t? trên xu?ng (top-down), Y là chi?u cao c? ??nh
            lineRenderer.SetPosition(i, new Vector3(x, characterPivot.position.y, z));
        }

        // ??i m?t kho?ng th?i gian (trailDuration) ?? v?t m? d?n
        yield return new WaitForSeconds(trailDuration);

        // Sau khi h?t th?i gian, t?t Line Renderer
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0; // Xóa các ?i?m ?? không còn v?t
    }
}