using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unit
{
    //  TODO:
    //  implement skill for ranger

    //  implementation for the mouseover tooltip of a ranger
    public override void MOTooltip()
    {
        if (this.curHealth > 0)
        {
            Tooltip.ShowTooltip_Static("Ranger\nBack-line bow using unit with the ability to trap enemies for 1 turn.");
        }
        else
        {
            Tooltip.ShowTooltip_Static("Here lies a dead unit, long may they RIP.");
        }
    }
}
