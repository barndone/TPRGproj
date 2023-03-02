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
        Tooltip.ShowTooltip_Static("Rogue\nMelee unit with high mobility that does increased damage based off of positioning.");
    }
}
