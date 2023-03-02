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
        Tooltip.ShowTooltip_Static("Cleric\nMelee support unit with the ability to heal their allies.");
    }
}
