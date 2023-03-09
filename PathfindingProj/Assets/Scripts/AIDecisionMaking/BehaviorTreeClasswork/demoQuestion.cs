using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoQuestion : IBehavior
{
    //  constructor
    public demoQuestion()
    {

    }

    //  for testing purposes check to see if this unit can move
    public BehaviorResult DoBehavior(Unit agent)
    {
        //  if it can move
        if (!agent.hasMoved)
        {
            //  wahoo we did it
            return BehaviorResult.Success;
        }
        //  damn we can't move :(
        else return BehaviorResult.Failure;
    }
}
