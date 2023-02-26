using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //  flag for whether it is a player or CPU turn
    //  default to true
    public bool playerTurn = true;

    //  reference to the player controller (stores each unit the player has control over)
    [SerializeField] private PlayerController player;

    //  reference to the computer controller (stores each unit the computer has contorl over)
    [SerializeField] private ComputerController cpu;

    private void TurnStart()
    {
        //  if it is the players turn
        if (playerTurn)
        {
            //  for each unit in the player party
            foreach(Unit unit in player.party)
            {
                //  reset the flags on that unit to allow for movement and action
                unit.StartOfTurn();
                unit.selectable = true;
            }
            //  mark that the player has actions to take
            player.noMoreActions = false;

            foreach(Unit unit in cpu.party)
            {
                unit.selectable = false;
            }
        }

        //  otherwise, it's the CPU turn
        else
        {
            //  for each unit in the CPU party
            foreach (Unit unit in cpu.party)
            {
                //  reset the flags on that unit to allow for movement and action
                unit.StartOfTurn();
                unit.selectable = true;
            }
            //  mark that the cpu controller has actions to take
            cpu.noMoreActions = false;

            foreach (Unit unit in player.party)
            {
                unit.selectable = false;
            }
        }
    }

    public void TurnEnd()
    {
        //  flip the value of the bool playerTurn
        playerTurn = !playerTurn;
        TurnStart();
    }

    void LateUpdate()
    {
        //  if the CPU noMoreActions is false and it is NOT the player's turn
        if (!cpu.noMoreActions && !playerTurn)
        {
            //  initialize a counter
            int counter = 0;
            //  check each unit in the list
            foreach (Unit unit in cpu.party)
            {
                //  if that unit is waiting:
                if (unit.waiting)
                {
                    //  increment the counter
                    counter++;
                }
            }

            //  after iteration, if the count is equal to the size of the party
            //  AKA all units are waiting
            if (counter == cpu.party.Count)
            {
                //  we can flag that there are no more actions so we can end the turn
                cpu.noMoreActions = true;
            }
        }

        //  otherwise, if the player has more actions to take and it is currently their turn
        else if (!player.noMoreActions && playerTurn)
        {
            //  initialize a counter
            int counter = 0;
            //  check each unit in the list
            foreach (Unit unit in player.party)
            {
                //  if that unit is waiting:
                if (unit.waiting)
                {
                    //  increment the counter
                    counter++;
                }
            }

            //  if the counter is equal to the size of the party
            //  all units are waiting
            if (counter == player.party.Count)
            {
                //  we can flag that there are no more actions so we can end the turn
                player.noMoreActions = true;
            }
        }
    }

    void Update()
    {
        //  if there are no more actions the CPU could take AND it is NOT currently the player's turn
        if (cpu.noMoreActions && !playerTurn)
        {
            //  time to end it all
            TurnEnd();
        }

        //  otherwise if there are no more actions the player could take and it is currently the player's turn
        else if (player.noMoreActions && playerTurn)
        {
            //  time to end it all
            TurnEnd();
        }
    }
}
