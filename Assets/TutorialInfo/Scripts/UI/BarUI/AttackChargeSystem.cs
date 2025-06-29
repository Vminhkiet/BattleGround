using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AttackChargeSystem : MonoBehaviour
{
    // Class nh? ?? nh�m Image Background v� Image Fill cho m?i slot
    [System.Serializable] // Cho ph�p Unity hi?n th? trong Inspector
    public class ChargeSlotUI
    {
        public Image background; // N?n c?a c?c (m�u x�m khi r?ng)
        public Image fill;       // Ph?n fill c?a c?c (m�u cam khi ??y)
    }

    [Header("UI References")]
    [Tooltip("K�o T?T C? c�c ChargeSlot_X (ho?c c�c GameObject cha c?a ChargeBG/ChargeFill) v�o ?�y.")]
    public List<ChargeSlotUI> chargeSlots; // Danh s�ch c�c c?u tr�c ch?a Image BG v� Fill

    [Header("Attack Charge Settings")]
    public int maxCharges = 3;
    public float rechargeTimePerCharge = 2f; // Th?i gian ?? h?i ph?c 1 ?�n (gi�y)

    [Header("Charge State Visuals")]
    public Color fullChargeColor = Color.cyan;   // M�u c?a fill khi ??y ?? (v� d?: cam)
    public Color emptyChargeColor = Color.gray; // M�u c?a background khi r?ng (v� d?: x�m)

    private float currentRechargeProgress; // Ti?n ?? h?i ph?c cho ?�n ti?p theo
    private int currentChargesCount;       // S? ?�n ?�nh hi?n c� (s? nguy�n)

    public int CurrentCharges { get { return currentChargesCount; } }

    void Awake()
    {
        if (chargeSlots.Count != maxCharges)
        {
            Debug.LogWarning("AttackChargeSystem: Number of charge slots (" + chargeSlots.Count + ") does not match maxCharges (" + maxCharges + ")! Adjusting maxCharges.", this);
            maxCharges = chargeSlots.Count;
        }

        currentChargesCount = maxCharges; // B?t ??u v?i t?t c? ?�n ??y
        currentRechargeProgress = 0f;
        UpdateUI(); // C?p nh?t UI ban ??u
    }

    void Update()
    {
        if (currentChargesCount < maxCharges) // N?u ch?a c� ?? s? ?�n t?i ?a
        {
            currentRechargeProgress += Time.deltaTime; // T?ng ti?n ?? h?i ph?c

            if (currentRechargeProgress >= rechargeTimePerCharge)
            {
                currentChargesCount++; // H?i ph?c 1 ?�n
                currentChargesCount = Mathf.Min(currentChargesCount, maxCharges); // ??m b?o kh�ng v??t qu� maxCharges
                currentRechargeProgress = 0f; // Reset ti?n ?? cho ?�n ti?p theo

                UpdateUI(); // C?p nh?t to�n b? UI khi m?t ?�n m?i ???c h?i ph?c ho�n to�n
            }
            else
            {
                // N?u ch?a ?? th?i gian ?? h?i m?t ?�n ??y ??,
                // c?p nh?t UI cho bi?u t??ng ?ang h?i ph?c ?? hi?n th? ti?n ?? fill
                UpdatePartialChargeUI();
            }
        }
    }

    public bool ConsumeCharge()
    {
        if (currentChargesCount >= 1)
        {
            currentChargesCount--; // Gi?m 1 ?�n
            currentRechargeProgress = 0f; // B?t ??u qu� tr�nh h?i ph?c cho ?�n v?a m?t
            UpdateUI(); // C?p nh?t tr?ng th�i c�c icon ngay l?p t?c
            return true;
        }
        return false;
    }

    /// <summary>
    /// C?p nh?t tr?ng th�i "??y" ho?c "r?ng" cho t?t c? c�c icon.
    /// ??i v?i c�c ?�n ?� c�: fill ??y, m�u ??y.
    /// ??i v?i c�c ?�n ?� d�ng: fill 0, m�u n?n r?ng.
    /// </summary>
    void UpdateUI()
    {
        for (int i = 0; i < maxCharges; i++)
        {
            if (i < currentChargesCount)
            {
                // ?�n ?�nh ?� s?n s�ng: fill ??y, m�u ??y
                chargeSlots[i].fill.fillAmount = 1f;
                chargeSlots[i].fill.color = fullChargeColor;
                chargeSlots[i].background.color = fullChargeColor; // N?n c?ng c� th? theo m�u ??y n?u mu?n
            }
            else
            {
                // ?�n ?�nh ?� s? d?ng ho?c ?ang ch? h?i ph?c: fill 0, m�u n?n r?ng
                chargeSlots[i].fill.fillAmount = 0f;
                chargeSlots[i].fill.color = fullChargeColor; // M�u fill v?n l� m�u ??y, ch? l� n� ?ang 0%
                chargeSlots[i].background.color = emptyChargeColor;
            }
        }
    }

    /// <summary>
    /// C?p nh?t hi?n th? ti?n ?? h?i ph?c c?a ?�n ?�nh ti?p theo (n?u c�).
    /// Icon n�y s? fill d?n t? 0 ??n 1.
    /// </summary>
    void UpdatePartialChargeUI()
    {
        // Ch? x? l� n?u ?ang c� m?t ?�n b? thi?u v� ?ang trong qu� tr�nh h?i ph?c
        if (currentChargesCount < maxCharges)
        {
            // L?y slot UI c?a ?�n ?ang h?i ph?c (v� d?: n?u c� 1 ?�n, ?ang h?i ?�n th? 2, th� index l� 1)
            ChargeSlotUI slotToRecharge = chargeSlots[currentChargesCount];

            // T�nh to�n t? l? ti?n ?? h?i ph?c (t? 0 ??n 1)
            float progressRatio = currentRechargeProgress / rechargeTimePerCharge;

            // C?p nh?t fillAmount cho Image fill c?a slot ?�
            slotToRecharge.fill.fillAmount = progressRatio;

            // B?n c� th? t�y ch?n thay ??i m�u n?n c?a slot ?� d?n d?n khi n� h?i ph?c
            // Color interpolatedBGColor = Color.Lerp(emptyChargeColor, fullChargeColor, progressRatio);
            // slotToRecharge.background.color = interpolatedBGColor;
        }
    }
}