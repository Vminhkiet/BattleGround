using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KaventEffectManager : MonoBehaviour, IEffectAttackManager
{
    [SerializeField] private ParticleSystem _normalAttack1;
    [SerializeField] private ParticleSystem _normalAttack2;
    [SerializeField] private ParticleSystem _normalAttack3;
    [SerializeField] private GameObject _ultiPrefab;
    private GameObject _ulti;
    private ParticleSystem _pUlti;
    [SerializeField] private ParticleSystem _spell;

    private bool isTurnOnUlti = false;
    private void Start()
    {
        _normalAttack1?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _normalAttack1?.Clear(true);
        _ulti = Instantiate(_ultiPrefab);
        _pUlti = _ulti.GetComponentInChildren<ParticleSystem>();
        BlackHoleSkill blackHoleSkill = _ulti.GetComponentInChildren<BlackHoleSkill>();
        blackHoleSkill.SetCaster(this.gameObject);
        TurnOffUlti();
    }

    private void Update()
    {
        if (_pUlti == null) return;
        if (!_pUlti.isStopped)
        {
            return;
        }
        if (isTurnOnUlti)
        {
            TurnOffUlti();
        }
    }

    public void PlayNormalAttack1()
    {
        _normalAttack1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _normalAttack1.Clear(true);
        _normalAttack1?.Play();
    }

    public void PlayNormalAttack2(Vector2 direction)
    {
        RotateEffect(_normalAttack2, direction);
        _normalAttack2?.Play();
    }

    public void PlayNormalAttack3(Vector2 direction)
    {
        RotateEffect(_normalAttack3, direction);
        _normalAttack3?.Play();
    }

    public void PlayUlti(Vector2 position)
    {
        _ulti.transform.position = new Vector3(position.x, transform.position.y, position.y);
        TurnOnUlti();
    }

    public void PlaySpell(Vector2 direction)
    {
        RotateEffect(_spell, direction);
        _spell?.Play();
    }

    public void RotateEffect(ParticleSystem ps, Vector2 direction)
    {
        if (ps == null || direction == Vector2.zero) return;

        Vector3 direction3D = new Vector3(direction.x, 0f, direction.y).normalized;

        ps.transform.rotation = Quaternion.LookRotation(direction3D);
    }
    public void TurnOnUlti()
    {
        _ulti?.SetActive(true);
        _pUlti?.Play();
        isTurnOnUlti = true;
    }
    public void TurnOffUlti()
    {
        _pUlti.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _pUlti?.Clear(true);
        _ulti?.SetActive(false);
        isTurnOnUlti = false;

    }
    public void StopAllEffects()
    {
        _normalAttack1?.Stop();
        _normalAttack2?.Stop();
        _normalAttack3?.Stop();
        _spell?.Stop();
    }
}