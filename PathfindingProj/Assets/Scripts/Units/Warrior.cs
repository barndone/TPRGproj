using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{
    //  implementation for the warriors skill
    public override void Skill(Tile targetTile)
    {
        //  check if it is a valid tile to attack
        if (ValidTile(targetTile.MapPos))
        {
            //  does that tile actually have a unit on it?
            if (targetTile.occupyingUnit != null)
            {
                //  reference to the other unit
                Unit other = targetTile.occupyingUnit;
                //  cache the initial position of the other unit
                Vector2 initialPos = other.mapPosition;

                //  mark the occupying unit of the target tile as null
                targetTile.occupyingUnit = null;
                //  mark the tile as no longer occupied
                targetTile.Occupied = false;

                //  Move the other unit one tile away from the current unit
                other.mapPosition += (other.mapPosition - this.mapPosition);

                //  check if that tile is out of bounds OR an obstacle OR occupied
                if ((other.mapPosition.x < 0 || other.mapPosition.x >= gridManager.Rows)
                    || (other.mapPosition.y < 0 || other.mapPosition.y >= gridManager.Columns)
                    || gridManager.map[other.mapPosition].Occupied 
                    || gridManager.map[other.mapPosition].Obstacle)
                {
                    if ((other.mapPosition.x < 0 || other.mapPosition.x >= gridManager.Rows)
                    || (other.mapPosition.y < 0 || other.mapPosition.y >= gridManager.Columns))
                    {
                        //  return the other unit to its original position
                        other.mapPosition = initialPos;
                        //  reduce the other unit's health by 1
                        other.CurHealth--;
                        this.dmgDone_val++;
                        other.dmgTaken_val--;
                    }
                    //  if the tile is occupied:
                    else if (gridManager.map[other.mapPosition].Occupied)
                    {
                        //  cache the collided with unit!
                        Unit collided = gridManager.map[other.mapPosition].occupyingUnit;
                        //  reduce the health of the collided with unit by 1
                        collided.CurHealth--;
                        this.dmgDone_val++;
                        collided.dmgTaken_val--;

                        //  return the other unit to its original position
                        other.mapPosition = initialPos;
                        //  reduce the other unit's health by 1
                        other.CurHealth--;
                        this.dmgDone_val++;
                        other.dmgTaken_val--;
                    }
                    else if (gridManager.map[other.mapPosition].Obstacle)
                    {
                        //  return the other unit to its original position
                        other.mapPosition = initialPos;
                        //  reduce the other unit's health by 1
                        other.CurHealth--;
                        this.dmgDone_val++;
                        other.dmgTaken_val--;
                    }

                    //  find a random damage taken sound
                    var randomDamageSound = other.dmgTaken[Random.Range(0, dmgTaken.Length)];
                    //  play that random sound
                    other.audioSource.PlayOneShot(randomDamageSound);
                }

                //  assign the other unit as the occupyingUnit of the tile it landed in
                gridManager.map[other.mapPosition].occupyingUnit = other;
                //  mark the tile the other unit landed in as occupied
                gridManager.map[other.mapPosition].Occupied = true;

                //  mark the current tile of the pushed unit
                other.currentTile = gridManager.map[other.mapPosition];
                //  transform the position of the pushed unit to the position of their new tile
                other.transform.position = other.currentTile.gameObj.transform.position;
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
