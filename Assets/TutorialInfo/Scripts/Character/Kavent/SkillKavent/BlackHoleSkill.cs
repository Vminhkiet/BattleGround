using UnityEngine;

public class BlackHoleSkill : MonoBehaviour
{
    private GameObject caster;
    private ISkillEffect[] effects;

    void Start()
    {
        effects = GetComponentsInChildren<ISkillEffect>();
        if(caster != null)
            foreach (var effect in effects)
            {
                effect.SetCaster(caster);
            }
    }
    public void SetCaster(GameObject casterObj)
    {
        caster = casterObj;
        foreach (var effect in effects)
        {
            effect.SetCaster(caster);
        }
    }
}
