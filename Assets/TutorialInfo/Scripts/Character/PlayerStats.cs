using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    private int maxEnergy = 100;
    private int health = 100;
    private int energy = 100;
    private int spellCoolDown = 0;

    public void TakeDamage(int damage)
    {
        health-=damage;
    }

    public bool isEnergyFull()
    {
        return energy==maxEnergy;
    }

    public bool isSpellFull()
    {
        return spellCoolDown==0;
    }
}
