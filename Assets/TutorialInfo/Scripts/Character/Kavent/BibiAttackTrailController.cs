using UnityEngine;
using System.Collections;

public class BibiAttackTrailController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform characterPivot; // V? tr� trung t�m c?a nh�n v?t, n?i cung tr�n s? xoay quanh
    public float attackRadius = 3f; // B�n k�nh c?a cung tr�n
    public float attackAngle = 90f; // G�c c?a cung tr�n (v� d? 90 ?? cho 1/4 v�ng tr�n)
    public float trailDuration = 0.2f; // Th?i gian v?t t?n t?i
    public int segments = 30; // S? l??ng ?o?n th?ng ?? t?o cung tr�n (c�ng nhi?u c�ng m?n)

    private Coroutine drawTrailCoroutine;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // ??m b?o kh�ng c� ?i?m n�o ban ??u
        lineRenderer.enabled = false; // T?t Line Renderer ban ??u
    }

    // H�m n�y s? ???c g?i t? Animation Event
    public void ActivateAttackTrail(float characterRotationY)
    {
        if (drawTrailCoroutine != null)
        {
            StopCoroutine(drawTrailCoroutine); // D?ng coroutine c? n?u c�
        }
        drawTrailCoroutine = StartCoroutine(DrawArcTrail(characterRotationY));
    }

    IEnumerator DrawArcTrail(float characterRotationY)
    {
        lineRenderer.enabled = true; // B?t Line Renderer
        lineRenderer.positionCount = segments + 1; // S? ?i?m = s? ?o?n + 1

        // T�nh to�n g�c b?t ??u v� k?t th�c c?a cung tr�n d?a tr�n h??ng c?a nh�n v?t
        // characterRotationY l� g�c xoay Y c?a nh�n v?t (v� d?: 0 ?? l� ph�a tr??c, 90 ?? l� b�n ph?i)
        float startAngleRad = (characterRotationY - attackAngle / 2f) * Mathf.Deg2Rad;
        float endAngleRad = (characterRotationY + attackAngle / 2f) * Mathf.Deg2Rad;
        float angleStep = (endAngleRad - startAngleRad) / segments;

        // V? cung tr�n
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngleRad + i * angleStep;
            float x = characterPivot.position.x + attackRadius * Mathf.Sin(currentAngle);
            float z = characterPivot.position.z + attackRadius * Mathf.Cos(currentAngle);
            // Gi? s? game nh�n t? tr�n xu?ng (top-down), Y l� chi?u cao c? ??nh
            lineRenderer.SetPosition(i, new Vector3(x, characterPivot.position.y, z));
        }

        // ??i m?t kho?ng th?i gian (trailDuration) ?? v?t m? d?n
        yield return new WaitForSeconds(trailDuration);

        // Sau khi h?t th?i gian, t?t Line Renderer
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0; // X�a c�c ?i?m ?? kh�ng c�n v?t
    }
}