using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class KnightScript : MonoBehaviour, ICharacterSkill
{
    public GameObject skillIndicatorPrefab;
    public GameObject UltiEffect;
    private GameObject activeIndicator;
    public float ultiRange = 4f;
    public float targetingRange = 8f;
    public float UltiDuration=3;

    public void Init() {
    }
    public void SetEffectSkill(IEffectPlayer effectPlayer)
    {

    }
    void Start()
    {
    
    }

    public void NormalAttack(Vector2 inputright)
    {

    }

    public void UseSkill(Vector2 inputright)
    {
        StartCoroutine(BuffPower());
    }

    private IEnumerator BuffPower()
    {
        UltiEffect.SetActive(true);
        yield return new WaitForSeconds(UltiDuration);
        UltiEffect.SetActive(false);
    }

    public void DrawUltiPosition(Vector2 input)
    {
        if (input.sqrMagnitude > 0.01f)
        {
            if (activeIndicator == null)
            {
                activeIndicator = Instantiate(skillIndicatorPrefab);
                activeIndicator.transform.rotation = Quaternion.Euler(90, 0, 0);
                activeIndicator.transform.localScale = Vector3.one * ultiRange * 2f;
            }

            activeIndicator.SetActive(true);
            activeIndicator.transform.position = transform.position;

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
