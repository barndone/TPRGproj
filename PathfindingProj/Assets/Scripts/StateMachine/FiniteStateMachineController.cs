using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachineController : MonoBehaviour
{
    public Vector2[] patrolLocations;
    public int currentPatrolIndex;

    public GridManager gridManager;
    public TurnManager turnManager;

    public Unit thisUnit;
    public Unit enemyUnit;

    public List<Tile> accessibleTiles;

    public enum States
    { 
        Patrol,
        Seek,
        Attack,
        Skill
    }

    [SerializeField] private States currentState;

    private void Start()
    {
        accessibleTiles = new List<Tile>();
    }

    private void Update()
    {
        if (!turnManager.playerTurn)
        {
            if (thisUnit.isSelected)
            {
                switch (currentState)
                {
                    case States.Patrol:
                        Patrol();
                        break;
                    case States.Seek:
                        Seek();
                        break;
                    case States.Attack:
                        Attack();
                        break;
                    case States.Skill:
                        Skill();
                        break;
                    default:
                        Debug.LogError("Invalid State!");
                        break;
                }

                if (thisUnit.hasActed && thisUnit.hasMoved)
                {
                    thisUnit.EndOfUnitActions();
                }

                //  if the unit has NOT acted
                if (!thisUnit.hasActed)
                {
                    //  check if the unit could attack or use its skill on the target
                    CheckActions();
                }

                //  if the unit has NOT moved
                else if (!thisUnit.hasMoved)
                {
                    accessibleTiles.Clear();
                    //  flag that the unit would wish to move
                    thisUnit.uiController.moveWish = true;
                    //  check the accessible tiles of this unit
                    gridManager.ShowAccessibleTiles(thisUnit, out accessibleTiles);

                    //  check if the enemy is within range of this unit
                    if (accessibleTiles.Contains(gridManager.map[enemyUnit.mapPosition]))
                    {
                        //  if true, move to the enemy
                        currentState = States.Seek;
                        thisUnit.hasMoved = true;
                    }

                    //  otherwise, patrolin' time
                    else
                    {
                        //  if the current state is NOT patrol
                        if (currentState != States.Patrol)
                        {
                            //  flag the state as patrolling
                            currentState = States.Patrol;
                            thisUnit.hasMoved = true;
                        }
                        else
                        {
                            thisUnit.hasMoved = true;
                        }
                    }

                    thisUnit.uiController.moveWish = false;
                }
            }
        }
    }

    //
    //  Logic for implementating states
    //  TODO
    //

    void Patrol() 
    {
        Debug.Log($"Patrol + {Time.frameCount}");
    }
    void Seek() 
    {
        Debug.Log($"Move + {Time.frameCount}");
    }
    void Attack() 
    {
        Debug.Log($"Attack + {Time.frameCount}");
    }
    void Skill()
    {
        Debug.Log($"Skill + {Time.frameCount}");
    }

    void CheckActions()
    {
        //  flag that this unit would like to attack
        thisUnit.uiController.attackWish = true;
        //  check the accessible tiles of this unit
        gridManager.ShowAccessibleTiles(thisUnit, out accessibleTiles);

        //  check if this unit could attack the unit
        if (accessibleTiles.Contains(gridManager.map[enemyUnit.mapPosition]))
        {
            //  do the thing
            currentState = States.Attack;
            accessibleTiles.Clear();
            thisUnit.hasActed = true;
        }

        //  it is not in range, so check if a skill would hit it
        else
        {
            accessibleTiles.Clear();
            //  flag that this unit would not want to attack
            thisUnit.uiController.attackWish = false;
            //  flag that this unit would like to use a skill
            thisUnit.uiController.skillWish = true;
            //  check the accessible tiles of this unit
            gridManager.ShowAccessibleTiles(thisUnit, out accessibleTiles);

            //  check if this unit could use its skill on the enemy
            if (accessibleTiles.Contains(gridManager.map[enemyUnit.mapPosition]))
            {
                //  do the thing
                currentState = States.Skill;
                accessibleTiles.Clear();
                thisUnit.hasActed = true;
            }

            else
            {
                accessibleTiles.Clear();
                //  flag that it's acted because there is no possible action
                thisUnit.hasActed = true;
            }
            //  flag that this unit would not want to use its skill
            thisUnit.uiController.skillWish = false;
        }
    }

}
