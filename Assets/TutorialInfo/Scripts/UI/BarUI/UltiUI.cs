using UnityEngine;
using UnityEngine.UI;

public class UltiUI : MonoBehaviour
{
    [Header("UI Ulti Settings")]
    public Image fillImage;


    private float pointUlti;
    private float maxPointUlti;
    private Vector3 lookDirection;

    private PlayerStats playerStats;
        
    void Awake()
    {
        if (fillImage == null)
        {
            Debug.LogWarning("UIHealthBarBillboard: 'fillImage' is not assigned! Health bar visualization will not function correctly.", this);
        }
    }

    public void UpdateMaxPointUlti(float newMaxPointUlti)
    {
        maxPointUlti = newMaxPointUlti;
    }
    public void UpdateHealth(float newCurrent)
    {
        pointUlti = newCurrent;
            

        float healthRatio = Mathf.Clamp01(pointUlti / maxPointUlti);

        if (fillImage != null)
        {
            fillImage.fillAmount = healthRatio;
        }

        gameObject.SetActive(pointUlti >= 0 && pointUlti <= maxPointUlti);
    }
}