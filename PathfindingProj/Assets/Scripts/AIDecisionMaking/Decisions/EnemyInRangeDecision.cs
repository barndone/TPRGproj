using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInRangeDecision : IDecision
{
    private List<Unit> enemies;

    public EnemyInRangeDecision() { }

    public IDecision MakeDecision(Unit agent)
    {
        //  calculate the accessible tiles of the agent
        agent.gridManager.ShowAccessibleTiles(agent, out agent.accessibleTiles);


        //  populate the enemies 
        enemies = agent.turnManager.player.party;

        //  iterate through the list of allies
        foreach (Unit unit in enemies)
        {
            //  if there is no target:
            if (agent.target == null)
            {
                agent.target = unit;
            }

            //  otherwise, we have a target:
            else
            {
                //  compare the manhattan distance 
                //  AND compare the targets defence
                //  if it is weaker and closer, make it the target
                if (CoordinateUtils.CalcManhattanDistance(agent.mapPosition, unit.mapPosition)
                    < CoordinateUtils.CalcManhattanDistance(agent.mapPosition, agent.target.mapPosition)
                    && unit.Defence <= agent.target.Defence)
                {
                    agent.target = unit;
                }

                //  otherwise, we do nothing
            }
        }

        //  if the agent can reach its target
        if (agent.accessibleTiles.Contains(agent.target.currentTile))
        {
            //  attack the target
            //  calculate the path to the target
            agent.hasPath = agent.gridManager.CalculateAStarPath(agent.mapPosition, agent.target.mapPosition, out agent.pathToMove);
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
            agent.uiController.moveWish = true;

            //  wait
            //agent.uiController.waitWish = true;

            //  exit the decision loop
            return null;
        }
    }
}
