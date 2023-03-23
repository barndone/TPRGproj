using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unit
{
    private void Awake()
    {
        canAttackAdjacent = false;
    }
    //  Ranger skill -- Fire a more powerful shot (2x)
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
                //  check if this unit has NOT moved
                if (!this.hasMoved)
                {
                    //  if so, cache the target unit
                    Unit target = targetTile.occupyingUnit;


                    int damage = (this.Attack - target.Defence) * 2;

                    //  time to attack!
                    target.CurHealth -= damage;
                    this.dmgDone_val += damage;
                    target.dmgTaken_val += damage;

                    //  play the skill sound
                    audioSource.PlayOneShot(skillSound);
                    //  find a random damage taken sound
                    var randomDamageSound = target.dmgTaken[Random.Range(0, dmgTaken.Length)];
                    //  play that random sound
                    target.audioSource.PlayOneShot(randomDamageSound);

                    this.hasAction = false;
                    this.hasActed = true;
                    this.hasMoved = true;

                    this.EndOfUnitActions();

                    Debug.Log("Power Shot fired at:  " + target + ", current health: " + target.CurHealth);
                }

                else
                {
                    this.hasAction = false;
                    this.hasActed = false;
                    Debug.Log("This Ranger cannot use power shot since it has moved!");
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

    private void LateUpdate()
    {
        switch (this.hasMoved)
        {
            case false:
                this.canUseSkill = true;
                break;
            case true:
                this.canUseSkill = false;
                break;
        }


    }
}
