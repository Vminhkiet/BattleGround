using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaventScript : MonoBehaviour, ICharacterSkill
{
    private APlayerInputHandler InputHandler { get; set; }

    void Start()
    {
        InputHandler = GetComponent<KaventInputHandler>();
    }

    public void NormalAttack(Vector2 inputright)
    {
        int atkPhase = InputHandler.GetAttackPhase();

        switch (atkPhase)
        {
            case 1:

                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    public void UseSkill()
    {

    }
}

