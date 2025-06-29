using UnityEngine;
using UnityEngine.UI; // R?t quan tr?ng: c?n th? vi?n UI

public class UIHealthBarBillboard : MonoBehaviour
{
    [Header("UI Health Bar Settings")]
    [Tooltip("Kéo ??i t??ng Image 'HealthBarFill' t? Hierarchy vào ?ây.")]
    public Image fillImage; // Tham chi?u ??n Image component c?a HealthBarFill

    [Tooltip("Kéo ??i t??ng Camera chính c?a b?n vào ?ây, ho?c ?? tr?ng ?? tìm t? ??ng.")]
    public Camera mainCamera; // Camera ?? thanh máu luôn h??ng t?i

    // Các bi?n này không c?n public vì chúng ch? ???c qu?n lý n?i b? qua hàm UpdateHealth
    private float currentHealth;
    private float maxHealth;

    void Awake()
    {
        // Tìm camera chính n?u ch?a ???c gán trong Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (fillImage == null)
        {
            // Debug.LogError s? d?ng game n?u l?i này nghiêm tr?ng.
            // Debug.LogWarning ch? c?nh báo và cho phép game ch?y ti?p.
            Debug.LogWarning("UIHealthBarBillboard: 'fillImage' is not assigned! Health bar visualization will not function correctly.", this);
        }
    }

    void LateUpdate()
    {
        // ??m b?o Canvas World Space luôn h??ng v? phía camera
        if (mainCamera != null)
        {
            // L?y h??ng t? Canvas này ??n Camera
            Vector3 lookDirection = mainCamera.transform.position - transform.position;

            // R?T QUAN TR?NG: B? qua s? chênh l?ch ?? cao (tr?c Y) ?? gi? thanh máu th?ng ??ng.
            // N?u không có dòng này, thanh máu s? nghiêng khi camera nhìn t? trên/d??i.
            lookDirection.y = 0;

            // Ch? xoay n?u có h??ng nhìn h?p l? (tránh l?i khi lookDirection là Vector3.zero,
            // ?i?u này th??ng ch? x?y ra khi camera và canvas ? cùng m?t v? trí).
            if (lookDirection != Vector3.zero)
            {
                // T?o Quaternion ?? m?t ph?ng Canvas nhìn th?ng vào camera.
                // Tr?c forward (Z) c?a Canvas c?n nhìn ng??c chi?u v?i lookDirection (t?c là h??ng v? camera).
                transform.rotation = Quaternion.LookRotation(-lookDirection);
            }
        }
    }

    /// <summary>
    /// C?p nh?t giá tr? máu và hi?n th? thanh máu.
    /// Hàm này s? ???c g?i t? script qu?n lý máu c?a nhân v?t.
    /// </summary>
    /// <param name="newCurrentHealth">Máu hi?n t?i c?a nhân v?t.</param>
    /// <param name="newMaxHealth">Máu t?i ?a c?a nhân v?t.</param>
    public void UpdateHealth(float newCurrentHealth, float newMaxHealth)
    {
        currentHealth = newCurrentHealth;
        maxHealth = newMaxHealth;

        // Tính toán t? l? ph?n tr?m máu (luôn n?m trong kho?ng 0-1)
        float healthRatio = Mathf.Clamp01(currentHealth / maxHealth);

        // C?p nh?t Fill Amount c?a Image
        if (fillImage != null)
        {
            fillImage.fillAmount = healthRatio;
        }

        // Tùy ch?n: ?n/Hi?n toàn b? Canvas khi ??y máu ho?c h?t máu.
        // Ch? hi?n thanh máu khi máu không ??y và không h?t hoàn toàn.
        gameObject.SetActive(currentHealth > 0 && currentHealth < maxHealth);
        // N?u b?n mu?n luôn hi?n thanh máu, hãy b? dòng trên ho?c s?a thành:
        // gameObject.SetActive(currentHealth > 0);
    }
}