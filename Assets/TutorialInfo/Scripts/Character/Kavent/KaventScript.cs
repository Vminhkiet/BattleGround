using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaventScript : MonoBehaviour, ICharacterSkill
{
    private APlayerInputHandler InputHandler { get; set; }
    [SerializeField] private AttackChargeSystem _attackChargeSystem;
    public GameObject skillIndicatorPrefab;
    public GameObject colliderSlash;
    private GameObject activeIndicator;
    public float ultiRange = 4f;
    private Vector3 currentTargetPosition;
    public float targetingRange = 8f;

    private IEffectPlayer effectPlayer;
    public void SetEffectSkill(IEffectPlayer effectPlayer)
    {
        this.effectPlayer = effectPlayer;
    }

    public void Init()
    {
        InputHandler = GetComponentInParent<KaventInputHandler>();
        colliderSlash?.SetActive(true);
        colliderSlash?.GetComponent<HitBox>().SetCaster(InputHandler.gameObject);
        colliderSlash?.GetComponent<HitBox>().SetHitBoxActive(false);
        colliderSlash?.GetComponent<HitBox>().SetPlayerStats(GetComponent<PlayerStats>());
    }

    public void NormalAttack(Vector2 inputright)
    {
        int atkPhase = InputHandler.GetAttackPhase();
        colliderSlash.GetComponent<HitBox>().SetHitBoxActive(true);
        _attackChargeSystem.ConsumeCharge();
        effectPlayer?.PlayNormalAttackEffect(1, inputright);
        StartCoroutine(DisableSlashColliderAfterDelay(2f));
        
        switch (atkPhase)
        {
            case 1:

                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    private IEnumerator DisableSlashColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        colliderSlash.GetComponent<HitBox>().SetHitBoxActive(false);
    }
    public void UseSkill(Vector2 inputright)
    {
        effectPlayer?.PlayUltiEffect(new Vector2(activeIndicator.transform.position.x, activeIndicator.transform.position.z));
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
    public void UseSpell(Vector2 inputspell)
    {

    }

    public void SetNetworkOwnership(INetworkOwnership ownership)
    {

    }
}

