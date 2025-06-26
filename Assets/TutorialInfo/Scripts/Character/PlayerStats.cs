using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PlayerStats
{
    private int maxEnergy;
    private int health;
    private int energy;
    private int spellCoolDown;

    public void TakeDamage(int damage)
    {
        health-=damage;
    }

    public bool isEnergyFull()
    {
        return energy==maxEnergy;
    }
}
