using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnScreenAimStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI References")]
    [Tooltip("Kéo Image (UI) làm n?n c?a joystick vào ?ây trong Inspector.")]
    [SerializeField] private Image joystickBackground;
    [Tooltip("Kéo Image (UI) làm tay c?m c?a joystick vào ?ây trong Inspector. ?ây ph?i là con c?a Joystick Background.")]
    [SerializeField] private Image joystickHandle;

    [Header("Stick Settings")]
    [Tooltip("Bán kính kéo t?i ?a cho tay c?m t? tâm c?a background (??n v? pixel UI).")]
    [SerializeField] private float movementRange = 50f;
    [Tooltip("Kho?ng cách kéo t?i thi?u t? ?i?m ch?m ban ??u ?? k? n?ng ???c kích ho?t khi nh? tay.")]
    [SerializeField] private float activationThreshold = 10f;

    [Header("Ability Logic Settings")]
    [Tooltip("Th?i gian h?i chiêu (cooldown) c?a k? n?ng tính b?ng giây.")]
    [SerializeField] private float cooldownTime = 5f;

    [Header("Cooldown UI (Optional)")]
    [Tooltip("Text UI ?? hi?n th? th?i gian cooldown còn l?i (ví d?: '3s').")]
    [SerializeField] private Text cooldownText;
    [Tooltip("Image UI (lo?i Filled) ?? hi?n th? hi?u ?ng cooldown (ví d?: m?t vòng tròn ?? ??y).")]
    [SerializeField] private Image cooldownOverlay;

    private Vector2 startPos;
    private Vector2 currentDragPos;
    private bool isDragging = false;
    private float currentCooldown = 0f;

    [Tooltip("Tên c?a Input Action (Type: Value, Control Type: Vector2) trong Input Actions Asset. " +
             "?ây là Action mà PlayerController s? l?ng nghe.")]
    [SerializeField] private string controlPath = "AimDirection";

    void Awake()
    {
        if (joystickBackground == null) joystickBackground = GetComponent<Image>();
        if (joystickHandle == null && transform.childCount > 0) joystickHandle = transform.GetChild(0).GetComponent<Image>();

        if (joystickBackground == null || joystickHandle == null)
        {
            Debug.LogError("OnScreenAimStick: Các tham chi?u UI (Background ho?c Handle) ch?a ???c thi?t l?p chính xác. T?t script.");
            enabled = false;
            return;
        }

        ResetJoystickVisuals(); // Hàm này s? ???c ch?nh s?a ?? không bi?n m?t hoàn toàn
        UpdateCooldownUI(); // Hàm này s? ?i?u khi?n tr?ng thái hi?n th? d?a trên cooldown
    }

    void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            UpdateCooldownUI();
        }
        else // Khi không còn cooldown
        {
            // ??m b?o UI c?p nh?t v? tr?ng thái s?n sàng khi cooldown k?t thúc
            if (joystickBackground.color.a < 0.7f && !isDragging) // Ki?m tra n?u nó ch?a ?? rõ và không ?ang kéo
            {
                UpdateCooldownUI(); // G?i l?i ?? ??t alpha v? tr?ng thái s?n sàng
            }
        }
    }

    protected override string controlPathInternal
    {
        get => controlPath;
        set => controlPath = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentCooldown > 0)
        {
            Debug.Log("K? n?ng ?ang trong th?i gian h?i chiêu!");
            return;
        }

        isDragging = true;
        startPos = eventData.position;

        UpdateHandlePosition(eventData.position, eventData.pressEventCamera);

        // Khi b?t ??u kéo, làm cho nó rõ ràng h?n
        joystickBackground.color = new Color(joystickBackground.color.r, joystickBackground.color.g, joystickBackground.color.b, 0.7f);
        joystickHandle.color = new Color(joystickHandle.color.r, joystickHandle.color.g, joystickHandle.color.b, 1f);

        SendValueToControl(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || currentCooldown > 0) return;

        currentDragPos = eventData.position;

        UpdateHandlePosition(currentDragPos, eventData.pressEventCamera);

        Vector2 dragVector = currentDragPos - startPos;
        Vector2 normalizedDrag = dragVector / movementRange;
        normalizedDrag = Vector2.ClampMagnitude(normalizedDrag, 1f);

        SendValueToControl(normalizedDrag);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        // Reset v? trí tay c?m v? tâm sau khi nh?
        joystickHandle.rectTransform.anchoredPosition = joystickBackground.rectTransform.anchoredPosition;

        Vector2 finalDragVector = currentDragPos - startPos;

        SendValueToControl(Vector2.zero);

        if (finalDragVector.magnitude >= activationThreshold)
        {
            Debug.Log("?ã kéo ?? ng??ng. K? n?ng s? ???c x? lý qua PlayerController.");

            currentCooldown = cooldownTime;
            // G?i UpdateCooldownUI ngay l?p t?c ?? hi?n th? cooldown m?i
            UpdateCooldownUI();
        }
        else
        {
            Debug.Log("Không ?? l?c kéo ?? kích ho?t k? n?ng.");
            // N?u không ?? ng??ng, ??a v? tr?ng thái s?n sàng (m?)
            UpdateCooldownUI(); // ??m b?o UI tr? l?i tr?ng thái s?n sàng n?u không kích ho?t
        }
    }

    // --- Các Hàm H? Tr? Visual UI ---

    private void UpdateHandlePosition(Vector2 screenPosition, Camera cam)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.rectTransform,
            screenPosition,
            cam,
            out localPoint
        );

        Vector2 relativePos = localPoint - joystickBackground.rectTransform.anchoredPosition;
        relativePos = Vector2.ClampMagnitude(relativePos, movementRange);

        joystickHandle.rectTransform.anchoredPosition = joystickBackground.rectTransform.anchoredPosition + relativePos;
    }

    // ?Ã S?A: Hàm này s? ??t alpha v? tr?ng thái m? khi không t??ng tác, không bi?n m?t hoàn toàn.
    private void ResetJoystickVisuals()
    {
        joystickHandle.rectTransform.anchoredPosition = joystickBackground.rectTransform.anchoredPosition;
        // ??t alpha v? m?t giá tr? nh? h?n 1 (ví d?: 0.3f) ?? nó luôn hi?n th? m? khi không ho?t ??ng
        joystickBackground.color = new Color(joystickBackground.color.r, joystickBackground.color.g, joystickBackground.color.b, 0.3f);
        joystickHandle.color = new Color(joystickHandle.color.r, joystickHandle.color.g, joystickHandle.color.b, 0.3f);
    }

    // ?Ã S?A: ?i?u ch?nh cách alpha ???c tính toán ?? x? lý tr?ng thái s?n sàng rõ ràng h?n.
    private void UpdateCooldownUI()
    {
        if (cooldownText != null)
        {
            cooldownText.text = currentCooldown > 0 ? Mathf.CeilToInt(currentCooldown).ToString() : "";
            cooldownText.enabled = currentCooldown > 0;
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = currentCooldown > 0 ? currentCooldown / cooldownTime : 0f;
            cooldownOverlay.enabled = currentCooldown > 0;
        }

        float targetAlpha;
        if (currentCooldown > 0)
        {
            // Khi ?ang cooldown, alpha s? gi?m t? 0.7f (ngay sau khi kích ho?t) xu?ng 0.3f (k?t thúc cooldown)
            targetAlpha = Mathf.Lerp(0.3f, 0.7f, currentCooldown / cooldownTime);
        }
        else
        {
            // Khi không có cooldown, ??t alpha v? tr?ng thái s?n sàng (ví d?: 0.5f)
            // ?ây là tr?ng thái m?c ??nh khi nó không ???c kéo và s?n sàng dùng.
            targetAlpha = 0.5f;
        }

        joystickBackground.color = new Color(joystickBackground.color.r, joystickBackground.color.g, joystickBackground.color.b, targetAlpha);
        joystickHandle.color = new Color(joystickHandle.color.r, joystickHandle.color.g, joystickHandle.color.b, targetAlpha);
    }
}