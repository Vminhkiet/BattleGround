using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    [Tooltip("Kéo ??i t??ng Canvas 'HealthBarCanvas' (n?i có script UIHealthBarBillboard) vào ?ây.")]
    public UIHealthBarBillboard healthBarUIBillboard;

    [Header("Optional Events")]
    public UnityEvent onDeath;

    [Header("Debug")]
    public bool showDebugButtons = false;

    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("PlayerHealthUI: 'healthBarUIBillboard' reference is not set!", this);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }

        if (IsDead)
        {
            Debug.Log($"{gameObject.name} died.");
            onDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthBarUIBillboard != null)
        {
            healthBarUIBillboard.UpdateHealth(currentHealth, maxHealth);
        }
    }

    [PunRPC]
    public void TakeDamageNetwork(float damage)
    {
        TakeDamage(damage);
    }

    void OnGUI()
    {
        if (!showDebugButtons) return;

        float buttonWidth = 140;
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

        if (GUI.Button(new Rect(padding, padding * 3 + buttonHeight * 2, buttonWidth, buttonHeight), "Reset Health"))
        {
            ResetHealth();
        }
    }
}
