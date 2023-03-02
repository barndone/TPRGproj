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
        Tooltip.ShowTooltip_Static("Ranger\nBack-line bow using unit with the ability to trap enemies for 1 turn.");
    }
}
