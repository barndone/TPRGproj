using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoAction : IBehavior
{
    //  constructor
    public demoAction()
    {
        
    }

    public BehaviorResult DoBehavior(Unit agent)
    {
        Debug.Log(BehaviorResult.Success);
        return BehaviorResult.Success;
    }
}
