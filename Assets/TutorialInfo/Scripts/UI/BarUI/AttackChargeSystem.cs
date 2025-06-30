using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AttackChargeSystem : MonoBehaviour
{
    [System.Serializable]
    public class ChargeSlotUI
    {
        public Image background;
        public Image fill;
    }

    [Header("UI References")]
    [Tooltip("Kéo T?T C? các ChargeSlot_X (ho?c các GameObject cha c?a ChargeBG/ChargeFill) vào ?ây.")]
    public List<ChargeSlotUI> chargeSlots;

    [Header("Attack Charge Settings")]
    public int maxCharges = 3;
    public float rechargeTimePerCharge = 2f;

    [Header("Charge State Visuals")]
    public Color fullChargeColor = Color.cyan;
    public Color emptyChargeColor = Color.gray;

    private float currentRechargeProgress;
    private int currentChargesCount;

    public int CurrentCharges { get { return currentChargesCount; } }

    void Awake()
    {
        if (chargeSlots.Count != maxCharges)
        {
            Debug.LogWarning("AttackChargeSystem: Number of charge slots (" + chargeSlots.Count + ") does not match maxCharges (" + maxCharges + ")! Adjusting maxCharges.", this);
            maxCharges = chargeSlots.Count;
        }

        currentChargesCount = maxCharges;
        currentRechargeProgress = 0f;
        UpdateUI();
    }

    void Update()
    {
        if (currentChargesCount < maxCharges)
        {
            currentRechargeProgress += Time.deltaTime;

            if (currentRechargeProgress >= rechargeTimePerCharge)
            {
                currentChargesCount++;
                currentChargesCount = Mathf.Min(currentChargesCount, maxCharges);
                currentRechargeProgress = 0f;

                UpdateUI();
            }
            else
            {
                UpdatePartialChargeUI();
            }
        }
    }

    public bool ConsumeCharge()
    {
        if (currentChargesCount >= 1)
        {
            currentChargesCount--;
            currentRechargeProgress = 0f; 
            UpdateUI(); 
            return true;
        }
        return false;
    }


    void UpdateUI()
    {
        for (int i = 0; i < maxCharges; i++)
        {
            if (i < currentChargesCount)
            {
                chargeSlots[i].fill.fillAmount = 1f;
                chargeSlots[i].fill.color = fullChargeColor;
                chargeSlots[i].background.color = fullChargeColor; 
            }
            else
            {
                chargeSlots[i].fill.fillAmount = 0f;
                chargeSlots[i].fill.color = fullChargeColor;
                chargeSlots[i].background.color = emptyChargeColor;
            }
        }
    }

    void UpdatePartialChargeUI()
    {
        if (currentChargesCount < maxCharges)
        {
            ChargeSlotUI slotToRecharge = chargeSlots[currentChargesCount];

            float progressRatio = currentRechargeProgress / rechargeTimePerCharge;

            slotToRecharge.fill.fillAmount = progressRatio;

        }
    }
}