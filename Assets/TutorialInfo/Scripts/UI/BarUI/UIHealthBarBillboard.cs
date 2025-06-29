using UnityEngine;
using UnityEngine.UI; // R?t quan tr?ng: c?n th? vi?n UI

public class UIHealthBarBillboard : MonoBehaviour
{
    [Header("UI Health Bar Settings")]
    [Tooltip("K�o ??i t??ng Image 'HealthBarFill' t? Hierarchy v�o ?�y.")]
    public Image fillImage; // Tham chi?u ??n Image component c?a HealthBarFill

    [Tooltip("K�o ??i t??ng Camera ch�nh c?a b?n v�o ?�y, ho?c ?? tr?ng ?? t�m t? ??ng.")]
    public Camera mainCamera; // Camera ?? thanh m�u lu�n h??ng t?i

    // C�c bi?n n�y kh�ng c?n public v� ch�ng ch? ???c qu?n l� n?i b? qua h�m UpdateHealth
    private float currentHealth;
    private float maxHealth;

    void Awake()
    {
        // T�m camera ch�nh n?u ch?a ???c g�n trong Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (fillImage == null)
        {
            // Debug.LogError s? d?ng game n?u l?i n�y nghi�m tr?ng.
            // Debug.LogWarning ch? c?nh b�o v� cho ph�p game ch?y ti?p.
            Debug.LogWarning("UIHealthBarBillboard: 'fillImage' is not assigned! Health bar visualization will not function correctly.", this);
        }
    }

    void LateUpdate()
    {
        // ??m b?o Canvas World Space lu�n h??ng v? ph�a camera
        if (mainCamera != null)
        {
            // L?y h??ng t? Canvas n�y ??n Camera
            Vector3 lookDirection = mainCamera.transform.position - transform.position;

            // R?T QUAN TR?NG: B? qua s? ch�nh l?ch ?? cao (tr?c Y) ?? gi? thanh m�u th?ng ??ng.
            // N?u kh�ng c� d�ng n�y, thanh m�u s? nghi�ng khi camera nh�n t? tr�n/d??i.
            lookDirection.y = 0;

            // Ch? xoay n?u c� h??ng nh�n h?p l? (tr�nh l?i khi lookDirection l� Vector3.zero,
            // ?i?u n�y th??ng ch? x?y ra khi camera v� canvas ? c�ng m?t v? tr�).
            if (lookDirection != Vector3.zero)
            {
                // T?o Quaternion ?? m?t ph?ng Canvas nh�n th?ng v�o camera.
                // Tr?c forward (Z) c?a Canvas c?n nh�n ng??c chi?u v?i lookDirection (t?c l� h??ng v? camera).
                transform.rotation = Quaternion.LookRotation(-lookDirection);
            }
        }
    }

    /// <summary>
    /// C?p nh?t gi� tr? m�u v� hi?n th? thanh m�u.
    /// H�m n�y s? ???c g?i t? script qu?n l� m�u c?a nh�n v?t.
    /// </summary>
    /// <param name="newCurrentHealth">M�u hi?n t?i c?a nh�n v?t.</param>
    /// <param name="newMaxHealth">M�u t?i ?a c?a nh�n v?t.</param>
    public void UpdateHealth(float newCurrentHealth, float newMaxHealth)
    {
        currentHealth = newCurrentHealth;
        maxHealth = newMaxHealth;

        // T�nh to�n t? l? ph?n tr?m m�u (lu�n n?m trong kho?ng 0-1)
        float healthRatio = Mathf.Clamp01(currentHealth / maxHealth);

        // C?p nh?t Fill Amount c?a Image
        if (fillImage != null)
        {
            fillImage.fillAmount = healthRatio;
        }

        // T�y ch?n: ?n/Hi?n to�n b? Canvas khi ??y m�u ho?c h?t m�u.
        // Ch? hi?n thanh m�u khi m�u kh�ng ??y v� kh�ng h?t ho�n to�n.
        gameObject.SetActive(currentHealth > 0 && currentHealth < maxHealth);
        // N?u b?n mu?n lu�n hi?n thanh m�u, h�y b? d�ng tr�n ho?c s?a th�nh:
        // gameObject.SetActive(currentHealth > 0);
    }
}