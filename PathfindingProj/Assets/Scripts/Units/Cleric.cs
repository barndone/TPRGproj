using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleric : Unit
{
    [SerializeField] public int maxHeals = 3;
    [SerializeField] public int healCount = 0;

    //  Cleric skill -- heal the target ally by an amount equal to this units attack
    public override void Skill(Tile targetTile)
    {
        //  check if it is a valid tile to attack
        if (ValidTile(targetTile.MapPos))
        {
            //  does that tile actually have a unit on it?
            //  and are those units allies
            if (targetTile.occupyingUnit != null
                && targetTile.occupyingUnit.ally == this.ally)
            {
                //  check if this unit has enough uses of the skill left
                if (healCount < maxHeals)
                {
                    //  if so, cache the target unit
                    Unit target = targetTile.occupyingUnit;

                    //  heal by an amount equal to this unit's attack stat
                    target.CurHealth += (this.Attack);
                    target.healingTaken_val += (this.Attack);
                    this.healingDone_val += (this.Attack);
                    //  check to see if we have surpassed the max health of the target
                    if (target.CurHealth > target.MaxHealth)
                    {
                        //  if so, assign current health to max health
                        target.CurHealth = target.MaxHealth;
                    }

                    healCount++;

                    if (healCount >= maxHeals)
                    {
                        this.canUseSkill = false;
                    }
                }

                else
                {
                    this.hasAction = false;
                    this.hasActed = false;
                    Debug.Log("This Cleric is out of Heal uses");
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
