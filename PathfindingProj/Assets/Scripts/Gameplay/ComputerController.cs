using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerController : BaseController
{
    //  contains list of possible enemies for the enemy party
    [SerializeField] GameObject[] possibleEnemies;
    //  size of the enemy party
    [SerializeField] int partySize;

    private void Awake()
    {
        //  initialize start pos to furthest corner of grid from player
        Vector2 startPos = new Vector2(9, 9);
        //  add a random unit to the computer controller's party using the possible enemies list
        for (int i = 0; i < partySize; i++)
        {
            var randomEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Length)];
            GameObject newEnemy = Instantiate(randomEnemy);
            party.Add(newEnemy.GetComponent<Unit>());

            party[i].mapPosition = new Vector2(startPos.x, startPos.y - i);
            party[i].partyManager = this;
            party[i].StartOfTurnPos = party[i].mapPosition;
            party[i].ally = false;
            party[i].selectable = false;
        }
    }
}
