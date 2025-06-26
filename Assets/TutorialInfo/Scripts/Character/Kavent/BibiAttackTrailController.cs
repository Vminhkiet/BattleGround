using UnityEngine;
using System.Collections;

public class BibiAttackTrailController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    // THAY ĐỔI NÀY: 'characterRootTransform' giờ sẽ là KAVENT GameObject
    // KAVENT là nơi mà các script di chuyển/xoay chính đang nằm.
    public Transform characterRootTransform; // Kéo GameObject KAVENT vào đây trong Inspector

    public float attackRadius = 3f;
    public float attackAngle = 90f;
    public int segments = 30;

    public float indicatorAlpha = 0.3f;
    public float actualTrailAlpha = 1.0f;
    public float actualTrailDuration = 0.2f;

    private Coroutine currentTrailCoroutine;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;

        // THÊM: Đảm bảo characterRootTransform được gán
        // Nếu script này là con của KAVENT, bạn có thể tự động lấy KAVENT bằng GetComponetInParent
        if (characterRootTransform == null)
        {
            // Tìm KAVENT (đối tượng cha gần nhất có Animator, hoặc đơn giản là cha của cha nếu nó là root)
            // Hoặc bạn có thể kéo trực tiếp KAVENT vào ô này trong Inspector.
            // Để đơn giản, nếu script này nằm dưới một khớp xương và khớp xương nằm dưới KAVENT, thì KAVENT là cha của cha.
            characterRootTransform = transform.root; // Lấy root GameObject của hệ thống phân cấp
                                                     // Hoặc một cách khác: characterRootTransform = GetComponentInParent<Animator>().transform;
            if (characterRootTransform == null)
            {
                Debug.LogError("BibiAttackTrailController: Không tìm thấy Character Root Transform (KAVENT)! Vui lòng gán thủ công hoặc kiểm tra cấu trúc.", this);
            }
        }
    }

    // Hàm BẬT chỉ báo phạm vi tấn công (khi joystick đang kéo)
    // joystickDirection: hướng kéo của joystick (Vector2)
    public void EnableRangeIndicator(Vector2 joystickDirection)
    {
        if (characterRootTransform == null) return;

        if (currentTrailCoroutine != null)
        {
            StopCoroutine(currentTrailCoroutine);
        }
        lineRenderer.enabled = true;
        lineRenderer.positionCount = segments + 1;

        // Lấy góc xoay Y từ hướng joystick
        Vector3 worldAttackDirection = new Vector3(joystickDirection.x, 0f, joystickDirection.y).normalized;
        float joystickAngle = Mathf.Atan2(worldAttackDirection.x, worldAttackDirection.z) * Mathf.Rad2Deg;

        DrawArc(characterRootTransform.position, joystickAngle, attackRadius, attackAngle, indicatorAlpha);
    }

    // Hàm CẬP NHẬT chỉ báo phạm vi tấn công (khi joystick vẫn đang kéo và đổi hướng)
    public void UpdateRangeIndicator(Vector2 joystickDirection)
    {
        if (characterRootTransform == null) return;

        if (lineRenderer.enabled)
        {
            Vector3 worldAttackDirection = new Vector3(joystickDirection.x, 0f, joystickDirection.y).normalized;
            float joystickAngle = Mathf.Atan2(worldAttackDirection.x, worldAttackDirection.z) * Mathf.Rad2Deg;
            DrawArc(characterRootTransform.position, joystickAngle, attackRadius, attackAngle, indicatorAlpha);
        }
    }

    // Hàm được gọi từ Animation Event để tạo vệt tấn công THỰC TẾ
    public void ActivateActualAttackTrail()
    {
        if (characterRootTransform == null) return;

        if (currentTrailCoroutine != null)
        {
            StopCoroutine(currentTrailCoroutine);
        }
        // Lấy góc xoay Y hiện tại của KAVENT (vì KAVENT giờ là gốc xoay)
        float actualRotationY = characterRootTransform.rotation.eulerAngles.y;
        currentTrailCoroutine = StartCoroutine(DrawAndFadeArcTrail(actualRotationY, actualTrailDuration, actualTrailAlpha));
    }

    // Hàm TẮT hoàn toàn hiển thị vệt/chỉ báo
    public void DisableTrailVisual()
    {
        if (currentTrailCoroutine != null)
        {
            StopCoroutine(currentTrailCoroutine);
        }
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }

    // Hàm vẽ cung tròn (hàm nội bộ)
    private void DrawArc(Vector3 center, float centralAngleY, float radius, float angleExtent, float alpha)
    {
        // Điều chỉnh alpha của Line Renderer Material
        // Giữ nguyên logic này
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        startColor.a = alpha;
        endColor.a = alpha * 0.1f;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;

        float startAngleRad = (centralAngleY - angleExtent / 2f) * Mathf.Deg2Rad;
        float endAngleRad = (centralAngleY + angleExtent / 2f) * Mathf.Deg2Rad;
        float angleStep = (endAngleRad - startAngleRad) / segments;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngleRad + i * angleStep;
            float x = center.x + radius * Mathf.Sin(currentAngle);
            float z = center.z + radius * Mathf.Cos(currentAngle);
            lineRenderer.SetPosition(i, new Vector3(x, center.y + 0.1f, z));
        }
    }

    // Coroutine để vẽ và làm mờ vệt tấn công thực tế
    IEnumerator DrawAndFadeArcTrail(float characterRotationY, float duration, float startAlpha)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = segments + 1;

        DrawArc(characterRootTransform.position, characterRotationY, attackRadius, attackAngle, startAlpha);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, 0f, timer / duration);
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;
            startColor.a = currentAlpha;
            endColor.a = currentAlpha * 0.1f;
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
            yield return null;
        }

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }
}