using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Unit
{
    //  TODO:
    //  implement skill

    //  implementation for the mouseover tooltip of a rogue
    public override void MOTooltip()
    {
        
        if (this.curHealth > 0)
        {
            Tooltip.ShowTooltip_Static("Rogue\nMelee unit with high mobility that does increased damage based off of positioning.");
        }
        else
        {
            Tooltip.ShowTooltip_Static("Here lies a dead unit, long may they RIP.");
        }
    }
}
