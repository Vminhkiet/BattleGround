using UnityEngine;

public class UltiChargeManager : MonoBehaviour
{
    private UltiUI _ultiUI;

    [Header("Settings")]
    public float maxUltiPoint = 100f;
    public float currentUltiPoint = 0f;

    public void Init()
    {
        _ultiUI = FindObjectOfType<UltiUI>();
        if (_ultiUI != null)
        {
            _ultiUI.UpdateMaxPointUlti(maxUltiPoint);
            _ultiUI.UpdateHealth(currentUltiPoint);
        }
    }

    public void AddUltiPoint(float amount)
    {
        currentUltiPoint += amount;
        currentUltiPoint = Mathf.Min(currentUltiPoint, maxUltiPoint);

        if (_ultiUI != null)
            _ultiUI.UpdateHealth(currentUltiPoint);
    }

    public bool IsUltiFull()
    {
        return currentUltiPoint >= maxUltiPoint;
    }

    public void ResetUlti()
    {
        currentUltiPoint = 0;
        _ultiUI?.UpdateHealth(currentUltiPoint);
    }
}
