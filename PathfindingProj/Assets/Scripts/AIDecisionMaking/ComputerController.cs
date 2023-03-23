using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerController : BaseController
{
    //  contains list of possible enemies for the enemy party
    [SerializeField] GameObject[] possibleEnemies;
    //  size of the enemy party
    [SerializeField] int partySize;

    //  root decisions for each of the party members
    public List<IDecision> decisionRoots;

    void Awake()
    {
        decisionRoots = new List<IDecision>();

        //  initialize start pos to furthest corner of grid from player
        Vector2 startPos = new Vector2(9, 9);
        //  add a random unit to the computer controller's party using the possible enemies list
        for (int i = 0; i < partySize; i++)
        {
            var randomEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Length)];
            GameObject newEnemy = Instantiate(randomEnemy);
            
            newEnemy.gameObject.name = "Enemy " + (i + 1);
            party.Add(newEnemy.GetComponent<Unit>());

            party[i].mapPosition = new Vector2(startPos.x, startPos.y - i);
            party[i].partyManager = this;
            party[i].StartOfTurnPos = party[i].mapPosition;
            party[i].ally = false;
            party[i].selectable = false;


            //  root decision
            CanHealDecision root = new CanHealDecision(party[i]);

            //  healer path
            AllyNeedHealingDecision shouldHealCheck = new AllyNeedHealingDecision();
            AllyInRangeDecision healRangeCheck = new AllyInRangeDecision();

            //  attacking path
            EnemyInRangeDecision attackCheck = new EnemyInRangeDecision();

            //  assign branches to root
            root.trueBranch = shouldHealCheck;
            root.falseBranch = attackCheck;

            //  assign branches to healing check
            shouldHealCheck.trueBranch = healRangeCheck;
            shouldHealCheck.falseBranch = attackCheck;

            //  add this root to the list
            decisionRoots.Add(root);

            //  assign the root to the corresponding unit
            party[i].unitDecisionRef = decisionRoots[i];
        }
    }
    void Update()
    {
        //  if it is NOT the player's turn
        if (!TurnManager.playerTurn)
        {
            //  iterate through this controllers party
            for (int i = 0; i < party.Count; i++)
            {
                if (!party[i].IsDead)
                {
                    //  assign the current decision to the units corresponding decision in the list
                    IDecision curDecision = decisionRoots[i];
                    //  while the decision is not returning null:
                    for (int j = 0; j < 50 && curDecision != null; ++j)
                    {
                        //  make decisions!
                        curDecision = curDecision.MakeDecision(party[i]);

                        if (j == 49)
                        {
                            Debug.LogError("Unit will exceed decision making limit! Next run of this loop will abort.");
                        }
                    }

                    party[i].gridManager.HideAccessibleTiles(party[i], party[i].accessibleTiles);
                    party[i].gridManager.ResetTiles();
                }
            }   
        }
    }
}
