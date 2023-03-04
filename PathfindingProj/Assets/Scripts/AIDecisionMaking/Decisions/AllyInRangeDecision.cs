using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyInRangeDecision : IDecision
{
    public AllyInRangeDecision()
    {
        
    }

    public IDecision MakeDecision(Unit agent)
    {
        //  calculate the accessible tiles of the agent
        agent.gridManager.ShowAccessibleTiles(agent, out agent.accessibleTiles);
        
        //  if the agent can reach its target
        if (agent.accessibleTiles.Contains(agent.target.currentTile))
        {
            //  heal the target
            //  calculate the path to the target
            agent.hasPath = agent.gridManager.CalculateAStarPath(agent.mapPosition, agent.target.mapPosition, out agent.pathToMove);
            agent.uiController.moveWish = true;

            if (agent.hasMoved)
            {
                //  heal the target (tile)
                agent.Skill(agent.target.currentTile);
            }

            //  exit the decision loop
            return null;
        }

        //  otherwise it cannot reach its target
        else
        {
            //  move towards the target:

            //  calculate the path to the target
            agent.hasPath = agent.gridManager.CalculateAStarPath(agent.mapPosition, agent.target.mapPosition, out agent.pathToMove);
            agent.uiController.moveWish = true;

            //  wait
            if (agent.hasMoved)
            {
                agent.uiController.waitWish = true;
            }

            //  exit the decision loop
            return null;
        }
    }
}
