using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Unit
{
    [SerializeField] public int maxStuns = 2;
    [SerializeField] public int stunCount = 0;

    //  Rogue skill -- Stun the target enemy for one turn
    public override void Skill(Tile targetTile)
    {
        //  check if it is a valid tile to attack
        if (ValidTile(targetTile.MapPos))
        {
            //  does that tile actually have a unit on it?
            //  and is that unit an enemy
            if (targetTile.occupyingUnit != null
                && targetTile.occupyingUnit.ally != this.ally)
            {
                //  check if this unit has enough uses of the skill left
                if (stunCount < maxStuns)
                {
                    //  if so, cache the target unit
                    Unit target = targetTile.occupyingUnit;

                    //  flag the target as stunned
                    target.stunned = true;
                    Debug.Log(target + ": stunned for one turn");

                    //  increment the stun counter!
                    stunCount++;

                    if (stunCount >= maxStuns)
                    {
                        this.canUseSkill = false;
                    }
                }

                else
                {
                    this.hasAction = false;
                    this.hasActed = false;
                    Debug.Log("This Rogue is out of Stun uses");
                }
            }

            else
            {
                this.hasAction = false;
                this.hasActed = false;
                Debug.Log("Target is not valid");
            }
        }
    }
}
