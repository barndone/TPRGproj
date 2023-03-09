using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : IBehavior
{
    //  list of child behaviors to this selector node
    public List<IBehavior> childBehaviors;
    public Unit Agent;

    //  default constructor
    public SelectorNode()
    {
        childBehaviors = new List<IBehavior>();
    }

    //  Will return a success if ANY of its children return a success (OR)
    public BehaviorResult DoBehavior(Unit agent)
    {
        //  iterate through each child in the childBehaviors list
        foreach(IBehavior child in childBehaviors)
        {
            //  if any of them return a successful result
            if (child.DoBehavior(agent) == BehaviorResult.Success)
            {
                //  we exit
                return BehaviorResult.Success;
            }
        }
        //  otherwise, none of them were a success so we return Failure
        return BehaviorResult.Failure;
    }
}
