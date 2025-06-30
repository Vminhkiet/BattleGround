using UnityEngine;
using Photon.Pun;

public class PhotonEffectManagerAdapter : IEffectPlayer
{
    private EffectAttackManager _actualEffectManager;
    private PhotonView _photonView;
    private EffectAttackSynchronizer _synchronizer;

    public PhotonEffectManagerAdapter(EffectAttackManager _actualEffectManager, PhotonView photonView)
    {
        _actualEffectManager = _actualEffectManager;
        _photonView = photonView;
        if (_actualEffectManager == null) Debug.LogError("PhotonEffectManagerAdapter: EffectAttackManager is null!");
        if (_photonView == null) Debug.LogError("PhotonEffectManagerAdapter: PhotonView is null!");

        _synchronizer = _photonView.gameObject.GetComponent<EffectAttackSynchronizer>();
        if (_synchronizer == null)
        {
            Debug.LogError("PhotonEffectManagerAdapter: EffectAttackSynchronizer not found on PhotonView's GameObject! RPCs will not be sent.", _photonView.gameObject);
        }

    }

    public void PlayNormalAttackEffect(int attackPhase, Vector2 direction)
    {
        if (_actualEffectManager != null)
        {
            switch (attackPhase)
            {
                case 1: _actualEffectManager.PlayNormalAttack1(); break;
                case 2: _actualEffectManager.PlayNormalAttack2(direction); break;
                case 3: _actualEffectManager.PlayNormalAttack3(direction); break;
                default: Debug.LogWarning($"PhotonEffectManagerAdapter: Invalid attack phase {attackPhase}"); break;
            }
        }

        if (_synchronizer != null && _photonView != null && _photonView.IsMine)
        {
            switch (attackPhase)
            {
                case 1: _synchronizer.RPC_PlayNormalAttack1(); break;
                case 2: _synchronizer.RPC_PlayNormalAttack2(direction.x, direction.y); break;
                case 3: _synchronizer.RPC_PlayNormalAttack3(direction.x, direction.y); break;
            }
        }
    }

    public void PlayUltiEffect(Vector2 position)
    {
        if (_actualEffectManager != null) _actualEffectManager.PlayUlti(position);
        if (_synchronizer != null && _photonView != null && _photonView.IsMine)
        {
            _synchronizer.RPC_PlayUlti(position.x, position.y);
        }
    }

    public void PlaySpellEffect(Vector2 direction)
    {
        if (_actualEffectManager != null) _actualEffectManager.PlaySpell(direction);
        if (_synchronizer != null && _photonView != null && _photonView.IsMine)
        {
            _synchronizer.RPC_PlaySpell(direction.x, direction.y);
        }
    }

    public void StopAllEffects()
    {
        if (_actualEffectManager != null) _actualEffectManager.StopAllEffects();
        if (_synchronizer != null && _photonView != null && _photonView.IsMine)
        {
            _synchronizer.RPC_StopAllEffects();
        }
    }
}