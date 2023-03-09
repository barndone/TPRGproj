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

        //  if we have a target:
        if (agent.target != null)
        {
            //  let's check if the target is dead
            if (agent.target.IsDead)
            {
                //  if so, they are no longer a valid target!
                agent.target = null;
            }
            //  otherwise, do nothing
        }

        //  not an else statement so that if we reset the target, this is still called
        //  if there is not a target:
        if (agent.target == null)
        {
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
        }


        //  if the agent can reach its target
        if (agent.accessibleTiles.Contains(agent.target.currentTile))
        {
            //  attack the target

            //  check if this unit even has to move
            //  slightly janky way to show only the attacking range using my accessible tiles algorithm
            agent.hasMoved = true;
            //  calculate the accessible tiles of the agent
            agent.gridManager.ShowAccessibleTiles(agent, out agent.accessibleTiles);

            //  if the accessibleTiles list contains the currentTile of the target:
            if (agent.accessibleTiles.Contains(agent.target.currentTile))
            {
                //  unit can attack from where it is standing
            }

            //  otherwise, we will have to move
            else
            {
                //  swap the hasMoved parameter to account for movement (since we actually haven't moved yet)
                agent.hasMoved = false;
                //  calculate the accessible tiles of the agent
                agent.gridManager.ShowAccessibleTiles(agent, out agent.accessibleTiles);
                //  calculate the path to the target
                agent.hasPath = agent.gridManager.CalculateAStarPath(agent.mapPosition, agent.target.mapPosition, out agent.pathToMove);
                //  remove the tile that the target is standing in from the pathToMove
                agent.pathToMove.Remove(agent.target.mapPosition);
                //  set move-wish to true to start moving!
                agent.uiController.moveWish = true;

            }
            //  after we move, we attack!
            if (agent.hasMoved)
            {
                //  attack the target (tile)
                agent.Attacking(agent.target.currentTile);
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
