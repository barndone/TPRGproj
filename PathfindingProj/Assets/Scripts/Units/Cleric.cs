using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleric : Unit
{
    //  TODO:
    //  implement skill for cleric

    //  implementation for the mouseover tooltip of a cleric
    public override void MOTooltip()
    {
        if (this.curHealth > 0)
        {
            Tooltip.ShowTooltip_Static("Cleric\nMelee support unit with the ability to heal their allies.");
        }
        else
        {
            Tooltip.ShowTooltip_Static("Here lies a dead unit, long may they RIP.");
        }
    }
}
