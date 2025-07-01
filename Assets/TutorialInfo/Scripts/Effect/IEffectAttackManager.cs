using UnityEngine;

public interface IEffectAttackManager
{
    public void PlayNormalAttack1();

    public void PlayNormalAttack2(Vector2 direction);
    public void PlayNormalAttack3(Vector2 direction);

    public void PlayUlti(Vector2 position);

    public void PlaySpell(Vector2 direction);

    public void RotateEffect(ParticleSystem ps, Vector2 direction);
    public void TurnOnUlti();
    public void TurnOffUlti();
    public void StopAllEffects();
}
