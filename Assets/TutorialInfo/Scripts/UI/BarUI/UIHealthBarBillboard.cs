using UnityEngine;
using UnityEngine.UI;

public class UIHealthBarBillboard : MonoBehaviour
{
    [Header("UI Health Bar Settings")]
    [Tooltip("Kéo ??i t??ng Image 'HealthBarFill' t? Hierarchy vào ?ây.")]
    public Image fillImage;

    [Tooltip("Kéo ??i t??ng Camera chính c?a b?n vào ?ây, ho?c ?? tr?ng ?? tìm t? ??ng.")]
    public Camera mainCamera;

    private float currentHealth;
    private float maxHealth;
    private Vector3 lookDirection;
    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (fillImage == null)
        {
            Debug.LogWarning("UIHealthBarBillboard: 'fillImage' is not assigned! Health bar visualization will not function correctly.", this);
        }
    }
    private void Start()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 healthBarPosition = transform.position;

        lookDirection = new Vector3(
            cameraPosition.x - healthBarPosition.x,
            0f,
            cameraPosition.z - healthBarPosition.z
        ).normalized;
    }
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }


    public void UpdateHealth(float newCurrentHealth, float newMaxHealth)
    {
        currentHealth = newCurrentHealth;
        maxHealth = newMaxHealth;

        float healthRatio = Mathf.Clamp01(currentHealth / maxHealth);

        if (fillImage != null)
        {
            fillImage.fillAmount = healthRatio;
        }

        gameObject.SetActive(currentHealth >= 0 && currentHealth < maxHealth);
    }
}