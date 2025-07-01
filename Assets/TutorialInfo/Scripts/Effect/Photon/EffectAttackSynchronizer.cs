using UnityEngine;
using Photon.Pun;

public class EffectAttackSynchronizer : MonoBehaviourPunCallbacks
{
    private IEffectAttackManager _attackManager;

    private void Awake()
    {
        _attackManager = GetComponent<IEffectAttackManager>();
        if (_attackManager == null)
        {
            Debug.LogError("EffectAttackManager not found on " + gameObject.name);
        }
    }

    [PunRPC]
    public void RPC_PlayNormalAttack1()
    {
        _attackManager?.PlayNormalAttack1();
    }

    [PunRPC]
    public void RPC_PlayNormalAttack2(float dirX, float dirY) 
    {
        _attackManager?.PlayNormalAttack2(new Vector2(dirX, dirY));
    }

    [PunRPC]
    public void RPC_PlayNormalAttack3(float dirX, float dirY)
    {
        _attackManager?.PlayNormalAttack3(new Vector2(dirX, dirY));
    }

    [PunRPC]
    public void RPC_PlayUlti(float dirX, float dirY)
    {
        _attackManager?.PlayUlti(new Vector2(dirX, dirY));
    }

    [PunRPC]
    public void RPC_PlaySpell(float dirX, float dirY)
    {
        _attackManager?.PlaySpell(new Vector2(dirX, dirY));
    }

    [PunRPC]
    public void RPC_StopAllEffects()
    {
        _attackManager?.StopAllEffects();
    }
}
