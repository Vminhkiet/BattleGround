using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // M�u t?i ?a c?a nh�n v?t
    private float currentHealth; // M�u hi?n t?i c?a nh�n v?t

    [Header("UI References")]
    [Tooltip("K�o ??i t??ng Canvas 'HealthBarCanvas' (n?i c� script UIHealthBarBillboard) v�o ?�y.")]
    public UIHealthBarBillboard healthBarUIBillboard; // Tham chi?u ??n script UIHealthBarBillboard tr�n Canvas

    void Start()
    {
        currentHealth = maxHealth; // Kh?i t?o m�u ban ??u

        // Ki?m tra xem tham chi?u ??n thanh m�u UI c� t?n t?i kh�ng
        if (healthBarUIBillboard != null)
        {
            // C?p nh?t thanh m�u l?n ??u khi game b?t ??u
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError("PlayerHealthUI: 'healthBarUIBillboard' reference is not set! Health bar will not be updated.", this);
        }
    }

    /// <summary>
    /// G�y s�t th??ng cho nh�n v?t v� c?p nh?t thanh m�u.
    /// </summary>
    /// <param name="damage">L??ng s�t th??ng ph?i ch?u.</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Debug.Log(gameObject.name + " died!");
            // TODO: Th�m logic khi nh�n v?t ch?t ? ?�y (v� d?: Game Over, Respawn)
        }
        // C?p nh?t thanh m�u tr�n ??u nh�n v?t
        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    /// <summary>
    /// H?i m�u cho nh�n v?t v� c?p nh?t thanh m�u.
    /// </summary>
    /// <param name="amount">L??ng m�u h?i ph?c.</param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        // C?p nh?t thanh m�u tr�n ??u nh�n v?t
        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // --- TESTING (T�y ch?n) ---
    // S? d?ng c�c n�t n�y ?? ki?m tra trong Editor ho?c tr�n b?n Build ??n gi?n
    void OnGUI()
    {
        // ?i?u ch?nh v? tr� n�t n?u b? ch?ng l�n nhau
        float buttonWidth = 120;
        float buttonHeight = 30;
        float padding = 10;

        // N�t G�y s�t th??ng
        if (GUI.Button(new Rect(padding, padding, buttonWidth, buttonHeight), "Take Damage (10)"))
        {
            TakeDamage(10);
        }

        // N�t H?i m�u
        if (GUI.Button(new Rect(padding, padding * 2 + buttonHeight, buttonWidth, buttonHeight), "Heal (5)"))
        {
            Heal(5);
        }
    }
}