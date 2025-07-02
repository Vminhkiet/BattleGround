using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MageScript : MonoBehaviour, ICharacterSkill
{
    public GameObject cubePrefab;
    public float attackRange = 15f;
    public float shootDelay = 0.3f;
    public GameObject skillIndicatorPrefab;
    private GameObject activeIndicator;
    public float ultiRange = 4f;
    private Vector3 currentTargetPosition;
    public float targetingRange = 8f;

    [SerializeField] private AttackChargeSystem _attackChargeSystem;
    private INetworkOwnership _owner;
    private IEffectPlayer effectPlayer;
    public void Init() {
        ObjectPooler.Instance.SetDamageForcubePool(GetComponent<PlayerStats>().GetDamage());
        ObjectPooler.Instance.SetCasterForcubePool(GetComponentInParent<APlayerInputHandler>().gameObject);
    }
    public void SetEffectSkill(IEffectPlayer effectPlayer)
    {
        this.effectPlayer = effectPlayer;
    }
    public void SetNetworkOwnership(INetworkOwnership ownership)
    {
        this._owner = ownership;
    }
    public void NormalAttack(Vector2 inputright)
    {
        int attackphase = 1;
        _attackChargeSystem.ConsumeCharge();
        effectPlayer?.PlayNormalAttackEffect(attackphase, inputright);
    }

    


    public void DrawUltiPosition(Vector2 input)
    {
        if (input.sqrMagnitude > 0.01f)
        {
            float inputMagnitude = Mathf.Clamp01(input.magnitude);
            Vector3 dir = new Vector3(input.x, 0, input.y).normalized;
            Vector3 targetPos = transform.position + dir * inputMagnitude * targetingRange;

            if (activeIndicator == null)
            {
                activeIndicator = Instantiate(skillIndicatorPrefab);
                activeIndicator.transform.rotation = Quaternion.Euler(90, 0, 0);
                activeIndicator.transform.localScale = Vector3.one * ultiRange * 2f;
            }

            activeIndicator.SetActive(true);
            activeIndicator.transform.position = targetPos;

            currentTargetPosition = targetPos;
        }
        else
        {
            if (activeIndicator != null)
                activeIndicator.SetActive(false);
        }
    }

    public void UseSkill(Vector2 input)
    {
        effectPlayer?.PlaySpellEffect(input);
    }

    public void UseSpell(Vector2 input)
    {

    }
}
