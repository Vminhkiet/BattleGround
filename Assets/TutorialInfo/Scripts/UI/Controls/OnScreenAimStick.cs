using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnScreenAimStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI References")]
    [Tooltip("K�o Image (UI) l�m n?n c?a joystick v�o ?�y trong Inspector.")]
    [SerializeField] private Image joystickBackground;
    [Tooltip("K�o Image (UI) l�m tay c?m c?a joystick v�o ?�y trong Inspector. ?�y ph?i l� con c?a Joystick Background.")]
    [SerializeField] private Image joystickHandle;

    [Header("Stick Settings")]
    [Tooltip("B�n k�nh k�o t?i ?a cho tay c?m t? t�m c?a background (??n v? pixel UI).")]
    [SerializeField] private float movementRange = 50f;
    [Tooltip("Kho?ng c�ch k�o t?i thi?u t? ?i?m ch?m ban ??u ?? k? n?ng ???c k�ch ho?t khi nh? tay.")]
    [SerializeField] private float activationThreshold = 10f;

    [Header("Ability Logic Settings")]
    [Tooltip("Th?i gian h?i chi�u (cooldown) c?a k? n?ng t�nh b?ng gi�y.")]
    [SerializeField] private float cooldownTime = 5f;

    [Header("Cooldown UI (Optional)")]
    [Tooltip("Text UI ?? hi?n th? th?i gian cooldown c�n l?i (v� d?: '3s').")]
    [SerializeField] private Text cooldownText;
    [Tooltip("Image UI (lo?i Filled) ?? hi?n th? hi?u ?ng cooldown (v� d?: m?t v�ng tr�n ?? ??y).")]
    [SerializeField] private Image cooldownOverlay;

    private Vector2 startPos;
    private Vector2 currentDragPos;
    private bool isDragging = false;
    private float currentCooldown = 0f;

    [Tooltip("T�n c?a Input Action (Type: Value, Control Type: Vector2) trong Input Actions Asset. " +
             "?�y l� Action m� PlayerController s? l?ng nghe.")]
    [SerializeField] private string controlPath = "AimDirection";

    void Awake()
    {
        if (joystickBackground == null) joystickBackground = GetComponent<Image>();
        if (joystickHandle == null && transform.childCount > 0) joystickHandle = transform.GetChild(0).GetComponent<Image>();

        if (joystickBackground == null || joystickHandle == null)
        {
            Debug.LogError("OnScreenAimStick: C�c tham chi?u UI (Background ho?c Handle) ch?a ???c thi?t l?p ch�nh x�c. T?t script.");
            enabled = false;
            return;
        }

        ResetJoystickVisuals(); // H�m n�y s? ???c ch?nh s?a ?? kh�ng bi?n m?t ho�n to�n
        UpdateCooldownUI(); // H�m n�y s? ?i?u khi?n tr?ng th�i hi?n th? d?a tr�n cooldown
    }

    void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            UpdateCooldownUI();
        }
        else // Khi kh�ng c�n cooldown
        {
            // ??m b?o UI c?p nh?t v? tr?ng th�i s?n s�ng khi cooldown k?t th�c
            if (joystickBackground.color.a < 0.7f && !isDragging) // Ki?m tra n?u n� ch?a ?? r� v� kh�ng ?ang k�o
            {
                UpdateCooldownUI(); // G?i l?i ?? ??t alpha v? tr?ng th�i s?n s�ng
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
            Debug.Log("K? n?ng ?ang trong th?i gian h?i chi�u!");
            return;
        }

        isDragging = true;
        startPos = eventData.position;

        UpdateHandlePosition(eventData.position, eventData.pressEventCamera);

        // Khi b?t ??u k�o, l�m cho n� r� r�ng h?n
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

        // Reset v? tr� tay c?m v? t�m sau khi nh?
        joystickHandle.rectTransform.anchoredPosition = joystickBackground.rectTransform.anchoredPosition;

        Vector2 finalDragVector = currentDragPos - startPos;

        SendValueToControl(Vector2.zero);

        if (finalDragVector.magnitude >= activationThreshold)
        {
            Debug.Log("?� k�o ?? ng??ng. K? n?ng s? ???c x? l� qua PlayerController.");

            currentCooldown = cooldownTime;
            // G?i UpdateCooldownUI ngay l?p t?c ?? hi?n th? cooldown m?i
            UpdateCooldownUI();
        }
        else
        {
            Debug.Log("Kh�ng ?? l?c k�o ?? k�ch ho?t k? n?ng.");
            // N?u kh�ng ?? ng??ng, ??a v? tr?ng th�i s?n s�ng (m?)
            UpdateCooldownUI(); // ??m b?o UI tr? l?i tr?ng th�i s?n s�ng n?u kh�ng k�ch ho?t
        }
    }

    // --- C�c H�m H? Tr? Visual UI ---

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

    // ?� S?A: H�m n�y s? ??t alpha v? tr?ng th�i m? khi kh�ng t??ng t�c, kh�ng bi?n m?t ho�n to�n.
    private void ResetJoystickVisuals()
    {
        joystickHandle.rectTransform.anchoredPosition = joystickBackground.rectTransform.anchoredPosition;
        // ??t alpha v? m?t gi� tr? nh? h?n 1 (v� d?: 0.3f) ?? n� lu�n hi?n th? m? khi kh�ng ho?t ??ng
        joystickBackground.color = new Color(joystickBackground.color.r, joystickBackground.color.g, joystickBackground.color.b, 0.3f);
        joystickHandle.color = new Color(joystickHandle.color.r, joystickHandle.color.g, joystickHandle.color.b, 0.3f);
    }

    // ?� S?A: ?i?u ch?nh c�ch alpha ???c t�nh to�n ?? x? l� tr?ng th�i s?n s�ng r� r�ng h?n.
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
            // Khi ?ang cooldown, alpha s? gi?m t? 0.7f (ngay sau khi k�ch ho?t) xu?ng 0.3f (k?t th�c cooldown)
            targetAlpha = Mathf.Lerp(0.3f, 0.7f, currentCooldown / cooldownTime);
        }
        else
        {
            // Khi kh�ng c� cooldown, ??t alpha v? tr?ng th�i s?n s�ng (v� d?: 0.5f)
            // ?�y l� tr?ng th�i m?c ??nh khi n� kh�ng ???c k�o v� s?n s�ng d�ng.
            targetAlpha = 0.5f;
        }

        joystickBackground.color = new Color(joystickBackground.color.r, joystickBackground.color.g, joystickBackground.color.b, targetAlpha);
        joystickHandle.color = new Color(joystickHandle.color.r, joystickHandle.color.g, joystickHandle.color.b, targetAlpha);
    }
}