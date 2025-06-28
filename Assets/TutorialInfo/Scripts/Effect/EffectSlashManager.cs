using UnityEngine;

public class EffectSlashManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _normalAttack1;
    [SerializeField] private ParticleSystem _normalAttack2;
    [SerializeField] private ParticleSystem _normalAttack3;
    [SerializeField] private ParticleSystem _ulti;
    [SerializeField] private ParticleSystem _spell;

    public void PlayNormalAttack1() => _normalAttack1?.Play();

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

    public void PlayUlti(Vector2 direction)
    {
        RotateEffect(_ulti, direction);
        _ulti?.Play();
    }

    public void PlaySpell(Vector2 direction)
    {
        RotateEffect(_spell, direction);
        _spell?.Play();
    }

    private void RotateEffect(ParticleSystem ps, Vector2 direction)
    {
        if (ps == null || direction == Vector2.zero) return;

        Vector3 direction3D = new Vector3(direction.x, 0f, direction.y).normalized;

        ps.transform.rotation = Quaternion.LookRotation(direction3D);
    }

    public void StopAllEffects()
    {
        _normalAttack1?.Stop();
        _normalAttack2?.Stop();
        _normalAttack3?.Stop();
        _ulti?.Stop();
        _spell?.Stop();
    }
}
