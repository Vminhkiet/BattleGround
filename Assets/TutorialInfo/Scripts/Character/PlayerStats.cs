using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private string name;
    [SerializeField]
    private int maxEnergy = 100;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private int energy = 100;
    [SerializeField]
    private int spellCoolDown = 0;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    private Character _character;


    private void Awake()
    {
        GetCharacter();
    }
    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
    }

    public bool isEnergyFull()
    {
        return energy==maxEnergy;
    }

    public bool isSpellFull()
    {
        return spellCoolDown==0;
    }

    public void GetCharacter()
    {
        InfoPlayerDatabase db = Resources.Load<InfoPlayerDatabase>("Database/InfoPlayer");
        _character = db.GetCharacter();
        maxHealth = _character.health;
        speed = _character.speed;
        damage = _character.power;
    }
}
