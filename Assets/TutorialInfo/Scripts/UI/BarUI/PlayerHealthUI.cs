using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    [Tooltip("Kéo ??i t??ng Canvas 'HealthBarCanvas' (n?i có script UIHealthBarBillboard) vào ?ây.")]
    public UIHealthBarBillboard healthBarUIBillboard;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError("PlayerHealthUI: 'healthBarUIBillboard' reference is not set! Health bar will not be updated.", this);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Debug.Log(gameObject.name + " died!");
        }
        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    void OnGUI()
    {
        float buttonWidth = 120;
        float buttonHeight = 30;
        float padding = 10;

        if (GUI.Button(new Rect(padding, padding, buttonWidth, buttonHeight), "Take Damage (10)"))
        {
            TakeDamage(10);
        }

        if (GUI.Button(new Rect(padding, padding * 2 + buttonHeight, buttonWidth, buttonHeight), "Heal (5)"))
        {
            Heal(5);
        }
    }
}