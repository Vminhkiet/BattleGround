using UnityEngine;
using Photon.Pun;

public class PhotonEffectManagerAdapter : IEffectPlayer
{
    private EffectAttackManager _actualEffectManager;
    private PhotonView _photonView;
    private EffectAttackSynchronizer _synchronizer;

    public PhotonEffectManagerAdapter(EffectAttackManager _actualEffectManager, PhotonView photonView)
    {
        this._actualEffectManager = _actualEffectManager;
        this._photonView = photonView;
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
        switch (attackPhase)
        {
            case 1: _actualEffectManager?.PlayNormalAttack1(); break;
            case 2: _actualEffectManager?.PlayNormalAttack2(direction); break;
            case 3: _actualEffectManager?.PlayNormalAttack3(direction); break;
            default: Debug.LogWarning($"Invalid attack phase: {attackPhase}"); break;
        }

        if (_photonView != null && _photonView.IsMine)
        {
            switch (attackPhase)
            {
                case 1:
                    _photonView.RPC(nameof(EffectAttackSynchronizer.RPC_PlayNormalAttack1), RpcTarget.Others);
                    break;
                case 2:
                    _photonView.RPC(nameof(EffectAttackSynchronizer.RPC_PlayNormalAttack2), RpcTarget.Others, direction.x, direction.y);
                    break;
                case 3:
                    _photonView.RPC(nameof(EffectAttackSynchronizer.RPC_PlayNormalAttack3), RpcTarget.Others, direction.x, direction.y);
                    break;
            }
        }
    }

    public void PlayUltiEffect(Vector2 position)
    {
        _actualEffectManager?.PlayUlti(position);

        if (_photonView != null && _photonView.IsMine)
        {
            _photonView.RPC(nameof(EffectAttackSynchronizer.RPC_PlayUlti), RpcTarget.Others, position.x, position.y);
        }
    }

    public void PlaySpellEffect(Vector2 direction)
    {
        _actualEffectManager?.PlaySpell(direction);

        if (_photonView != null && _photonView.IsMine)
        {
            _photonView.RPC(nameof(EffectAttackSynchronizer.RPC_PlaySpell), RpcTarget.Others, direction.x, direction.y);
        }
    }

    public void StopAllEffects()
    {
        _actualEffectManager?.StopAllEffects();

        if (_photonView != null && _photonView.IsMine)
        {
            _photonView.RPC(nameof(EffectAttackSynchronizer.RPC_StopAllEffects), RpcTarget.Others);
        }
    }
}