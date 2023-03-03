using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInRangeDecision : IDecision
{
    public EnemyInRangeDecision() { }

    public IDecision MakeDecision(Unit agent)
    {
        //  calculate the accessible tiles of the agent
        agent.gridManager.ShowAccessibleTiles(agent, out agent.accessibleTiles);

        //  if the agent can reach its target
        if (agent.accessibleTiles.Contains(agent.target.currentTile))
        {
            //  attack the target
            //  calculate the path to the target
            agent.hasPath = agent.gridManager.CalculateAStarPath(agent.mapPosition, agent.target.mapPosition, out agent.pathToMove);
            agent.isSelected = true;
            agent.uiController.moveWish = true;

            //  attack the target (tile)
            agent.Attacking(agent.target.currentTile);

            //  exit the decision loop
            return null;
        }

        //  otherwise it cannot reach its target
        else
        {
            //  move towards the target:
            //  calculate the path to the target
            agent.hasPath = agent.gridManager.CalculateAStarPath(agent.mapPosition, agent.target.mapPosition, out agent.pathToMove);
            agent.isSelected = true;
            agent.uiController.moveWish = true;

            //  wait
            agent.uiController.waitWish = true;

            //  exit the decision loop
            return null;
        }
    }
}
