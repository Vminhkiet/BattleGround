using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // Máu t?i ?a c?a nhân v?t
    private float currentHealth; // Máu hi?n t?i c?a nhân v?t

    [Header("UI References")]
    [Tooltip("Kéo ??i t??ng Canvas 'HealthBarCanvas' (n?i có script UIHealthBarBillboard) vào ?ây.")]
    public UIHealthBarBillboard healthBarUIBillboard; // Tham chi?u ??n script UIHealthBarBillboard trên Canvas

    void Start()
    {
        currentHealth = maxHealth; // Kh?i t?o máu ban ??u

        // Ki?m tra xem tham chi?u ??n thanh máu UI có t?n t?i không
        if (healthBarUIBillboard != null)
        {
            // C?p nh?t thanh máu l?n ??u khi game b?t ??u
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError("PlayerHealthUI: 'healthBarUIBillboard' reference is not set! Health bar will not be updated.", this);
        }
    }

    /// <summary>
    /// Gây sát th??ng cho nhân v?t và c?p nh?t thanh máu.
    /// </summary>
    /// <param name="damage">L??ng sát th??ng ph?i ch?u.</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Debug.Log(gameObject.name + " died!");
            // TODO: Thêm logic khi nhân v?t ch?t ? ?ây (ví d?: Game Over, Respawn)
        }
        // C?p nh?t thanh máu trên ??u nhân v?t
        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    /// <summary>
    /// H?i máu cho nhân v?t và c?p nh?t thanh máu.
    /// </summary>
    /// <param name="amount">L??ng máu h?i ph?c.</param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        // C?p nh?t thanh máu trên ??u nhân v?t
        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // --- TESTING (Tùy ch?n) ---
    // S? d?ng các nút này ?? ki?m tra trong Editor ho?c trên b?n Build ??n gi?n
    void OnGUI()
    {
        // ?i?u ch?nh v? trí nút n?u b? ch?ng lên nhau
        float buttonWidth = 120;
        float buttonHeight = 30;
        float padding = 10;

        // Nút Gây sát th??ng
        if (GUI.Button(new Rect(padding, padding, buttonWidth, buttonHeight), "Take Damage (10)"))
        {
            TakeDamage(10);
        }

        // Nút H?i máu
        if (GUI.Button(new Rect(padding, padding * 2 + buttonHeight, buttonWidth, buttonHeight), "Heal (5)"))
        {
            Heal(5);
        }
    }
}