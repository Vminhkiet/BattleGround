using UnityEngine;

public interface IEffectPlayer
{
    void PlayNormalAttackEffect(int attackPhase, Vector2 direction);
    void PlayUltiEffect(Vector2 position);
    void PlaySpellEffect(Vector2 direction);
    void StopAllEffects();
}