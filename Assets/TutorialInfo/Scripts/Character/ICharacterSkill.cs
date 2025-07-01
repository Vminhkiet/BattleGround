using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterSkill
{
    void NormalAttack(Vector2 inputright);
    void UseSkill(Vector2 inputright);

    void UseSpell(Vector2 inputright);

    void DrawUltiPosition(Vector2 input);

    void SetEffectSkill(IEffectPlayer effectPlayer);
    void SetNetworkOwnership(INetworkOwnership ownership);
}
