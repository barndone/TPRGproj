using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : IBehavior
{
    //  list of each child behavior this node has
    public List<IBehavior> childBehaviors;

    //  constructor
    public SequenceNode()
    {
        childBehaviors = new List<IBehavior>();
    }

    //  Will return a success if ALL of its children return a success (AND)
    public BehaviorResult DoBehavior(Unit agent)
    {
        //  iterate through this node's child behaviors
        foreach(IBehavior child in childBehaviors)
        {
            //  if that node returns a failure
            if (child.DoBehavior(agent) == BehaviorResult.Failure)
            {
                //  return a failure
                return BehaviorResult.Failure;
            }
        }

        //  otherwise, all of them were a success so we return a success
        return BehaviorResult.Success;
    }
}
