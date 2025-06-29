using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AttackChargeSystem : MonoBehaviour
{
    // Class nh? ?? nhóm Image Background và Image Fill cho m?i slot
    [System.Serializable] // Cho phép Unity hi?n th? trong Inspector
    public class ChargeSlotUI
    {
        public Image background; // N?n c?a c?c (màu xám khi r?ng)
        public Image fill;       // Ph?n fill c?a c?c (màu cam khi ??y)
    }

    [Header("UI References")]
    [Tooltip("Kéo T?T C? các ChargeSlot_X (ho?c các GameObject cha c?a ChargeBG/ChargeFill) vào ?ây.")]
    public List<ChargeSlotUI> chargeSlots; // Danh sách các c?u trúc ch?a Image BG và Fill

    [Header("Attack Charge Settings")]
    public int maxCharges = 3;
    public float rechargeTimePerCharge = 2f; // Th?i gian ?? h?i ph?c 1 ?òn (giây)

    [Header("Charge State Visuals")]
    public Color fullChargeColor = Color.cyan;   // Màu c?a fill khi ??y ?? (ví d?: cam)
    public Color emptyChargeColor = Color.gray; // Màu c?a background khi r?ng (ví d?: xám)

    private float currentRechargeProgress; // Ti?n ?? h?i ph?c cho ?òn ti?p theo
    private int currentChargesCount;       // S? ?òn ?ánh hi?n có (s? nguyên)

    public int CurrentCharges { get { return currentChargesCount; } }

    void Awake()
    {
        if (chargeSlots.Count != maxCharges)
        {
            Debug.LogWarning("AttackChargeSystem: Number of charge slots (" + chargeSlots.Count + ") does not match maxCharges (" + maxCharges + ")! Adjusting maxCharges.", this);
            maxCharges = chargeSlots.Count;
        }

        currentChargesCount = maxCharges; // B?t ??u v?i t?t c? ?òn ??y
        currentRechargeProgress = 0f;
        UpdateUI(); // C?p nh?t UI ban ??u
    }

    void Update()
    {
        if (currentChargesCount < maxCharges) // N?u ch?a có ?? s? ?òn t?i ?a
        {
            currentRechargeProgress += Time.deltaTime; // T?ng ti?n ?? h?i ph?c

            if (currentRechargeProgress >= rechargeTimePerCharge)
            {
                currentChargesCount++; // H?i ph?c 1 ?òn
                currentChargesCount = Mathf.Min(currentChargesCount, maxCharges); // ??m b?o không v??t quá maxCharges
                currentRechargeProgress = 0f; // Reset ti?n ?? cho ?òn ti?p theo

                UpdateUI(); // C?p nh?t toàn b? UI khi m?t ?òn m?i ???c h?i ph?c hoàn toàn
            }
            else
            {
                // N?u ch?a ?? th?i gian ?? h?i m?t ?òn ??y ??,
                // c?p nh?t UI cho bi?u t??ng ?ang h?i ph?c ?? hi?n th? ti?n ?? fill
                UpdatePartialChargeUI();
            }
        }
    }

    public bool ConsumeCharge()
    {
        if (currentChargesCount >= 1)
        {
            currentChargesCount--; // Gi?m 1 ?òn
            currentRechargeProgress = 0f; // B?t ??u quá trình h?i ph?c cho ?òn v?a m?t
            UpdateUI(); // C?p nh?t tr?ng thái các icon ngay l?p t?c
            return true;
        }
        return false;
    }

    /// <summary>
    /// C?p nh?t tr?ng thái "??y" ho?c "r?ng" cho t?t c? các icon.
    /// ??i v?i các ?òn ?ã có: fill ??y, màu ??y.
    /// ??i v?i các ?òn ?ã dùng: fill 0, màu n?n r?ng.
    /// </summary>
    void UpdateUI()
    {
        for (int i = 0; i < maxCharges; i++)
        {
            if (i < currentChargesCount)
            {
                // ?òn ?ánh ?ã s?n sàng: fill ??y, màu ??y
                chargeSlots[i].fill.fillAmount = 1f;
                chargeSlots[i].fill.color = fullChargeColor;
                chargeSlots[i].background.color = fullChargeColor; // N?n c?ng có th? theo màu ??y n?u mu?n
            }
            else
            {
                // ?òn ?ánh ?ã s? d?ng ho?c ?ang ch? h?i ph?c: fill 0, màu n?n r?ng
                chargeSlots[i].fill.fillAmount = 0f;
                chargeSlots[i].fill.color = fullChargeColor; // Màu fill v?n là màu ??y, ch? là nó ?ang 0%
                chargeSlots[i].background.color = emptyChargeColor;
            }
        }
    }

    /// <summary>
    /// C?p nh?t hi?n th? ti?n ?? h?i ph?c c?a ?òn ?ánh ti?p theo (n?u có).
    /// Icon này s? fill d?n t? 0 ??n 1.
    /// </summary>
    void UpdatePartialChargeUI()
    {
        // Ch? x? lý n?u ?ang có m?t ?òn b? thi?u và ?ang trong quá trình h?i ph?c
        if (currentChargesCount < maxCharges)
        {
            // L?y slot UI c?a ?òn ?ang h?i ph?c (ví d?: n?u có 1 ?òn, ?ang h?i ?òn th? 2, thì index là 1)
            ChargeSlotUI slotToRecharge = chargeSlots[currentChargesCount];

            // Tính toán t? l? ti?n ?? h?i ph?c (t? 0 ??n 1)
            float progressRatio = currentRechargeProgress / rechargeTimePerCharge;

            // C?p nh?t fillAmount cho Image fill c?a slot ?ó
            slotToRecharge.fill.fillAmount = progressRatio;

            // B?n có th? tùy ch?n thay ??i màu n?n c?a slot ?ó d?n d?n khi nó h?i ph?c
            // Color interpolatedBGColor = Color.Lerp(emptyChargeColor, fullChargeColor, progressRatio);
            // slotToRecharge.background.color = interpolatedBGColor;
        }
    }
}